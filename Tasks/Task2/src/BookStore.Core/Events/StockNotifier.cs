namespace BookStore.Core.Events;

public sealed class StockNotifier
{
    public event EventHandler<StockEventArgs>? BookOutOfStock;

    public void Publish(object? sender, StockEventArgs args) => BookOutOfStock?.Invoke(sender, args);
}
