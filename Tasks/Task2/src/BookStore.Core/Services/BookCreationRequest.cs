namespace BookStore.Core.Services;

public sealed class BookCreationRequest
{
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required string Category { get; init; }
    public required string Isbn { get; init; }
    public required decimal Price { get; init; }
    public required int Stock { get; init; }

    public Guid? Id { get; init; }

    public IReadOnlyDictionary<string, string> ExtraFields { get; init; } = new Dictionary<string, string>();

    public string GetExtra(string key, string fallback = "") =>
        ExtraFields.TryGetValue(key, out var value) ? value : fallback;
}
