namespace BookStore.Core.Entities;

public sealed class Customer : IEntity
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    public Customer(string name, string email, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        Email = email;
    }

    public void UpdateContactInfo(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public override string ToString() => $"{Name} <{Email}>";
}
