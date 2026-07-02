using BookStore.Core.Entities;

namespace BookStore.Core.Events;

public sealed class StockEventArgs : EventArgs
{
    public BookBase Book { get; }
    public int RemainingStock { get; }

    public StockEventArgs(BookBase book, int remainingStock)
    {
        Book = book;
        RemainingStock = remainingStock;
    }
}
