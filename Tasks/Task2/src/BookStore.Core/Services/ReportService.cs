using BookStore.Core.Entities;
using BookStore.Core.Repositories;

namespace BookStore.Core.Services;

public sealed record BestSellerReport(BookBase Book, int UnitsSold);
public sealed record TopCustomerReport(Customer Customer, decimal TotalSpent);

public sealed class ReportService
{
    private readonly IRepository<BookBase> _books;
    private readonly IRepository<Customer> _customers;
    private readonly IRepository<Purchase> _purchases;

    public ReportService(IRepository<BookBase> books, IRepository<Customer> customers, IRepository<Purchase> purchases)
    {
        _books = books;
        _customers = customers;
        _purchases = purchases;
    }

    public decimal TotalRevenue() => _purchases.GetAll().Sum(p => p.Total);

    public BestSellerReport? BestSellingBook()
    {
        var unitsByBook = _purchases.GetAll()
            .SelectMany(p => p.Lines)
            .GroupBy(l => l.BookId)
            .Select(g => new { BookId = g.Key, Units = g.Sum(l => l.Quantity) })
            .OrderByDescending(x => x.Units)
            .FirstOrDefault();

        if (unitsByBook is null) return null;

        var book = _books.GetById(unitsByBook.BookId);
        return book is null ? null : new BestSellerReport(book, unitsByBook.Units);
    }

    public TopCustomerReport? TopCustomer()
    {
        var spendByCustomer = _purchases.GetAll()
            .GroupBy(p => p.CustomerId)
            .Select(g => new { CustomerId = g.Key, Spent = g.Sum(p => p.Total) })
            .OrderByDescending(x => x.Spent)
            .FirstOrDefault();

        if (spendByCustomer is null) return null;

        var customer = _customers.GetById(spendByCustomer.CustomerId);
        return customer is null ? null : new TopCustomerReport(customer, spendByCustomer.Spent);
    }
}
