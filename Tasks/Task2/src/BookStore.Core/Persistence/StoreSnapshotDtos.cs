namespace BookStore.Core.Persistence;

public sealed class BookDto
{
    public Guid Id { get; set; }
    public string Format { get; set; } = "";
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Category { get; set; } = "";
    public string Isbn { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Dictionary<string, string> ExtraFields { get; set; } = new();
}

public sealed class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

public sealed class PurchaseLineDto
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPriceAtSale { get; set; }
}

public sealed class PurchaseDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime PurchasedAtUtc { get; set; }
    public List<PurchaseLineDto> Lines { get; set; } = new();
}

public sealed class StoreSnapshot
{
    public List<BookDto> Books { get; set; } = new();
    public List<CustomerDto> Customers { get; set; } = new();
    public List<PurchaseDto> Purchases { get; set; } = new();
}
