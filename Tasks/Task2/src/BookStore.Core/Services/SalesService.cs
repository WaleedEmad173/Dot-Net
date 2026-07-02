using BookStore.Core.Entities;
using BookStore.Core.Repositories;
using BookStore.Core.Validation;

namespace BookStore.Core.Services;

public sealed record PurchaseRequestLine(Guid BookId, int Quantity);

public sealed class SalesService
{
    private readonly IRepository<BookBase> _books;
    private readonly IRepository<Customer> _customers;
    private readonly IRepository<Purchase> _purchases;
    private readonly SemaphoreSlim _purchaseGate = new(1, 1);

    public SalesService(IRepository<BookBase> books, IRepository<Customer> customers, IRepository<Purchase> purchases)
    {
        _books = books;
        _customers = customers;
        _purchases = purchases;
    }

    public async Task<Purchase> RecordPurchaseAsync(Guid customerId, IReadOnlyList<PurchaseRequestLine> requestedLines)
    {
        if (requestedLines is null || requestedLines.Count == 0)
            throw new ValidationException("A purchase must contain at least one book.");

        var customer = _customers.GetById(customerId)
            ?? throw new ValidationException("Customer not found.");

        var merged = requestedLines
            .GroupBy(l => l.BookId)
            .Select(g => new PurchaseRequestLine(g.Key, g.Sum(l => l.Quantity)))
            .ToList();

        foreach (var line in merged)
            Validator.RequirePositive(line.Quantity, "Quantity");

        await _purchaseGate.WaitAsync();
        var decremented = new List<(BookBase Book, int Quantity)>();
        try
        {
            var resolvedLines = new List<PurchaseLine>();

            foreach (var line in merged)
            {
                var book = _books.GetById(line.BookId)
                    ?? throw new ValidationException("One of the selected books no longer exists.");

                if (!book.TryDecreaseStock(line.Quantity))
                    throw new ValidationException(
                        $"Not enough stock for '{book.Title}'. Requested {line.Quantity}, only {book.Stock} available.");

                decremented.Add((book, line.Quantity));
                resolvedLines.Add(new PurchaseLine(book.Id, book.Title, line.Quantity, book.Price));
            }

            var purchase = new Purchase(customer.Id, resolvedLines);
            return _purchases.Add(purchase);
        }
        catch
        {
            foreach (var (book, quantity) in decremented)
                book.Restock(quantity);
            throw;
        }
        finally
        {
            _purchaseGate.Release();
        }
    }

    public IReadOnlyList<Purchase> ListAll() => _purchases.GetAll();

    public Purchase AddExisting(Purchase purchase) => _purchases.Add(purchase);
}
