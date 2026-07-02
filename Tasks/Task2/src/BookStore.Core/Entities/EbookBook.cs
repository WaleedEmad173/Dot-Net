namespace BookStore.Core.Entities;

public sealed class EbookBook : BookBase
{
    public double FileSizeMb { get; }
    public string FileType { get; }

    public EbookBook(string title, string author, string category, string isbn, decimal price, int stock, double fileSizeMb, string fileType, Guid? id = null)
        : base(title, author, category, isbn, price, stock, id)
    {
        FileSizeMb = fileSizeMb;
        FileType = fileType;
    }

    public override string Format => "Ebook";
    public override string FormatDetails => $"{FileSizeMb:0.#} MB - {FileType}";
}
