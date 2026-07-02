using BookStore.Core.Entities;
using BookStore.Core.Repositories;
using BookStore.Core.Validation;

namespace BookStore.Core.Services;

public sealed class CustomerService
{
    private readonly IRepository<Customer> _customers;

    public CustomerService(IRepository<Customer> customers)
    {
        _customers = customers;
    }

    public Customer Register(string name, string email)
    {
        Validator.RequireNonEmpty(name, "Name");
        Validator.RequireValidEmail(email);

        if (_customers.Find(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).Any())
            throw new ValidationException($"A customer with email {email} is already registered.");

        return _customers.Add(new Customer(name, email));
    }

    public Customer AddExisting(Customer customer) => _customers.Add(customer);

    public Customer? FindById(Guid customerId) => _customers.GetById(customerId);

    public IReadOnlyList<Customer> ListAll() => _customers.GetAll();
}
