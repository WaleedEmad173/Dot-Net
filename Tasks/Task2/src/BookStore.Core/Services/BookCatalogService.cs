using BookStore.Core.Entities;
using BookStore.Core.Events;
using BookStore.Core.Extensions;
using BookStore.Core.Repositories;
using BookStore.Core.Validation;

namespace BookStore.Core.Services;

public sealed class BookCatalogService
{
    private readonly IRepository<BookBase> _books;
    private readonly StockNotifier _stockNotifier;

    public BookCatalogService(IRepository<BookBase> books, StockNotifier stockNotifier)
    {
        _books = books;
        _stockNotifier = stockNotifier;
    }

    public BookBase AddBook(string format, BookCreationRequest request)
    {
        Validator.RequireNonEmpty(request.Title, "Title");
        Validator.RequireNonEmpty(request.Author, "Author");
        Validator.RequireNonEmpty(request.Category, "Category");
        Validator.RequireValidIsbn(request.Isbn);
        Validator.RequirePositive(request.Price, "Price");
        Validator.RequireNonNegative(request.Stock, "Stock");

        if (_books.Find(b => b.Isbn == request.Isbn).Any())
            throw new ValidationException($"A book with ISBN {request.Isbn} already exists.");

        var book = BookFactory.Create(format, request);
        book.OutOfStock += (sender, args) => _stockNotifier.Publish(sender, args);

        return _books.Add(book);
    }

    public BookBase AddExisting(BookBase book)
    {
        book.OutOfStock += (sender, args) => _stockNotifier.Publish(sender, args);
        return _books.Add(book);
    }

    public bool RemoveBook(Guid bookId) => _books.Remove(bookId);

    public BookBase? FindById(Guid bookId) => _books.GetById(bookId);

    public IEnumerable<BookBase> SearchByTitleOrAuthor(string term)
    {
        Validator.RequireNonEmpty(term, "Search term");
        return _books.Find(b =>
            b.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            b.Author.Contains(term, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<BookBase> ListAll() => _books.GetAll();

    public IEnumerable<BookBase> FilterByCategory(string category) => ListAll().ByCategory(category);

    public IEnumerable<BookBase> FilterByAuthor(string author) => ListAll().ByAuthor(author);

    public IEnumerable<BookBase> FilterByPriceRange(decimal min, decimal max)
    {
        if (min < 0 || max < min)
            throw new ValidationException("Invalid price range.");
        return ListAll().InPriceRange(min, max);
    }
}
