namespace BookStore.Core.Entities;

public sealed class AudiobookBook : BookBase
{
    public TimeSpan Duration { get; }
    public string Narrator { get; }

    public AudiobookBook(string title, string author, string category, string isbn, decimal price, int stock, TimeSpan duration, string narrator, Guid? id = null)
        : base(title, author, category, isbn, price, stock, id)
    {
        Duration = duration;
        Narrator = narrator;
    }

    public override string Format => "Audiobook";
    public override string FormatDetails => $"{Duration:h\\hmm\\m} - narrated by {Narrator}";
}
