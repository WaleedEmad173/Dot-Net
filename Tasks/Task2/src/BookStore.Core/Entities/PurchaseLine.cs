namespace BookStore.Core.Entities;

public sealed class PurchaseLine
{
    public Guid BookId { get; }
    public string BookTitle { get; }
    public int Quantity { get; }
    public decimal UnitPriceAtSale { get; }

    public decimal LineTotal => Quantity * UnitPriceAtSale;

    public PurchaseLine(Guid bookId, string bookTitle, int quantity, decimal unitPriceAtSale)
    {
        BookId = bookId;
        BookTitle = bookTitle;
        Quantity = quantity;
        UnitPriceAtSale = unitPriceAtSale;
    }
}
