using BookStore.Core.Entities;
using BookStore.Core.Validation;
using System.Globalization;

namespace BookStore.Core.Services;

public static class BookFactory
{
    private static readonly Dictionary<string, Func<BookCreationRequest, BookBase>> Creators =
        new(StringComparer.OrdinalIgnoreCase);

    static BookFactory()
    {
        RegisterFormat("Paperback", req => new PaperbackBook(
            req.Title, req.Author, req.Category, req.Isbn, req.Price, req.Stock,
            pageCount: int.TryParse(req.GetExtra("pageCount"), out var pages) ? pages : 0,
            id: req.Id));

        RegisterFormat("Ebook", req => new EbookBook(
            req.Title, req.Author, req.Category, req.Isbn, req.Price, req.Stock,
            fileSizeMb: double.TryParse(req.GetExtra("fileSizeMb"), NumberStyles.Any, CultureInfo.InvariantCulture, out var size) ? size : 0,
            fileType: req.GetExtra("fileType", "PDF"),
            id: req.Id));

        RegisterFormat("Audiobook", req => new AudiobookBook(
            req.Title, req.Author, req.Category, req.Isbn, req.Price, req.Stock,
            duration: TimeSpan.TryParse(req.GetExtra("duration"), out var duration) ? duration : TimeSpan.Zero,
            narrator: req.GetExtra("narrator", "Unknown"),
            id: req.Id));
    }

    public static void RegisterFormat(string formatName, Func<BookCreationRequest, BookBase> creator)
    {
        Validator.RequireNonEmpty(formatName, "Format name");
        ArgumentNullException.ThrowIfNull(creator);
        Creators[formatName] = creator;
    }

    public static IReadOnlyCollection<string> AvailableFormats => Creators.Keys.ToList().AsReadOnly();

    public static BookBase Create(string formatName, BookCreationRequest request)
    {
        if (!Creators.TryGetValue(formatName, out var creator))
            throw new ValidationException(
                $"Unknown book format '{formatName}'. Available formats: {string.Join(", ", AvailableFormats)}.");

        return creator(request);
    }
}
