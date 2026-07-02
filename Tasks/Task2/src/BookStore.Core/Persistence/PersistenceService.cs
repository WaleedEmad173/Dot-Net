using BookStore.Core.Entities;
using BookStore.Core.Services;
using System.Text.Json;

namespace BookStore.Core.Persistence;

public sealed class PersistenceService
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public PersistenceService(string filePath)
    {
        _filePath = filePath;
    }

    public async Task SaveAsync(BookCatalogService catalog, CustomerService customers, SalesService sales)
    {
        var snapshot = new StoreSnapshot
        {
            Books = catalog.ListAll().Select(ToDto).ToList(),
            Customers = customers.ListAll().Select(c => new CustomerDto { Id = c.Id, Name = c.Name, Email = c.Email }).ToList(),
            Purchases = sales.ListAll().Select(p => new PurchaseDto
            {
                Id = p.Id,
                CustomerId = p.CustomerId,
                PurchasedAtUtc = p.PurchasedAtUtc,
                Lines = p.Lines.Select(l => new PurchaseLineDto
                {
                    BookId = l.BookId,
                    BookTitle = l.BookTitle,
                    Quantity = l.Quantity,
                    UnitPriceAtSale = l.UnitPriceAtSale
                }).ToList()
            }).ToList()
        };

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, snapshot, JsonOptions);
    }

    public async Task<bool> LoadAsync(BookCatalogService catalog, CustomerService customers, SalesService sales)
    {
        if (!File.Exists(_filePath))
            return false;

        await using var stream = File.OpenRead(_filePath);
        var snapshot = await JsonSerializer.DeserializeAsync<StoreSnapshot>(stream, JsonOptions);
        if (snapshot is null) return false;

        foreach (var dto in snapshot.Books)
            catalog.AddExisting(FromDto(dto));

        foreach (var dto in snapshot.Customers)
            customers.AddExisting(new Customer(dto.Name, dto.Email, dto.Id));

        foreach (var dto in snapshot.Purchases)
        {
            var lines = dto.Lines.Select(l => new PurchaseLine(l.BookId, l.BookTitle, l.Quantity, l.UnitPriceAtSale));
            sales.AddExisting(new Purchase(dto.CustomerId, lines, dto.Id, dto.PurchasedAtUtc));
        }

        return true;
    }

    private static BookDto ToDto(BookBase book)
    {
        var extra = book switch
        {
            PaperbackBook p => new Dictionary<string, string> { ["pageCount"] = p.PageCount.ToString() },
            EbookBook e => new Dictionary<string, string> { ["fileSizeMb"] = e.FileSizeMb.ToString(), ["fileType"] = e.FileType },
            AudiobookBook a => new Dictionary<string, string> { ["duration"] = a.Duration.ToString(), ["narrator"] = a.Narrator },
            _ => new Dictionary<string, string>()
        };

        return new BookDto
        {
            Id = book.Id,
            Format = book.Format,
            Title = book.Title,
            Author = book.Author,
            Category = book.Category,
            Isbn = book.Isbn,
            Price = book.Price,
            Stock = book.Stock,
            ExtraFields = extra
        };
    }

    private static BookBase FromDto(BookDto dto)
    {
        var request = new BookCreationRequest
        {
            Title = dto.Title,
            Author = dto.Author,
            Category = dto.Category,
            Isbn = dto.Isbn,
            Price = dto.Price,
            Stock = dto.Stock,
            ExtraFields = dto.ExtraFields,
            Id = dto.Id
        };
        return BookFactory.Create(dto.Format, request);
    }
}
