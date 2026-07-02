using BookStore.Core.Entities;

namespace BookStore.Core.Extensions;

public static class BookExtensions
{
    public static IEnumerable<BookBase> ByCategory(this IEnumerable<BookBase> books, string category) =>
        books.Where(b => string.Equals(b.Category, category, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<BookBase> ByAuthor(this IEnumerable<BookBase> books, string author) =>
        books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<BookBase> InPriceRange(this IEnumerable<BookBase> books, decimal min, decimal max) =>
        books.Where(b => b.Price >= min && b.Price <= max);

    public static IEnumerable<BookBase> InStock(this IEnumerable<BookBase> books) =>
        books.Where(b => b.Stock > 0);

    public static void ApplyRule(this IEnumerable<BookBase> books, Action<BookBase> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        foreach (var book in books)
            rule(book);
    }

    public static void ApplyRule(this IEnumerable<BookBase> books, Func<BookBase, bool> when, Action<BookBase> rule) =>
        books.Where(when).ApplyRule(rule);
}
