namespace BookStore.Core.Entities;

public sealed class PaperbackBook : BookBase
{
    public int PageCount { get; }

    public PaperbackBook(string title, string author, string category, string isbn, decimal price, int stock, int pageCount, Guid? id = null)
        : base(title, author, category, isbn, price, stock, id)
    {
        PageCount = pageCount;
    }

    public override string Format => "Paperback";
    public override string FormatDetails => $"{PageCount} pages";
}
