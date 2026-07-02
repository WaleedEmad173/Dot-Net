using BookStore.Core.Events;

namespace BookStore.Core.Entities;

public abstract class BookBase : IEntity
{
    public Guid Id { get; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string Category { get; private set; }
    public string Isbn { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    private readonly object _stockLock = new();

    public event EventHandler<StockEventArgs>? OutOfStock;

    public abstract string Format { get; }

    public abstract string FormatDetails { get; }

    protected BookBase(string title, string author, string category, string isbn, decimal price, int stock, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Title = title;
        Author = author;
        Category = category;
        Isbn = isbn;
        Price = price;
        Stock = stock;
    }

    public void Rename(string title) => Title = title;

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentOutOfRangeException(nameof(newPrice), "Price cannot be negative.");
        Price = newPrice;
    }

    public void Restock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Restock quantity must be positive.");
        lock (_stockLock)
        {
            Stock += quantity;
        }
    }


    public bool TryDecreaseStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");

        lock (_stockLock)
        {
            if (Stock < quantity)
                return false;

            Stock -= quantity;

            if (Stock == 0)
                OutOfStock?.Invoke(this, new StockEventArgs(this, Stock));

            return true;
        }
    }

    public override string ToString() =>
        $"{Title} by {Author} [{Format}] - {Category} - {Price:C} - Stock: {Stock} - ISBN: {Isbn}";
}
