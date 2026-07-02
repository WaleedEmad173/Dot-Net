namespace BookStore.Core.Entities;


public sealed class Purchase : IEntity
{
    public Guid Id { get; }
    public Guid CustomerId { get; }
    public DateTime PurchasedAtUtc { get; }
    public IReadOnlyList<PurchaseLine> Lines { get; }

    public decimal Total => Lines.Sum(l => l.LineTotal);

    public Purchase(Guid customerId, IEnumerable<PurchaseLine> lines, Guid? id = null, DateTime? purchasedAtUtc = null)
    {
        Id = id ?? Guid.NewGuid();
        CustomerId = customerId;
        PurchasedAtUtc = purchasedAtUtc ?? DateTime.UtcNow;
        Lines = lines.ToList().AsReadOnly();
    }
}
