using BookStore.Core.Entities;
using BookStore.Core.Extensions;
using BookStore.Core.Services;
using BookStore.Core.Validation;

namespace BookStore.App.Menu;

internal sealed class MainMenu
{
    private readonly BookCatalogService _catalog;
    private readonly CustomerService _customers;
    private readonly SalesService _sales;
    private readonly ReportService _reports;

    public MainMenu(BookCatalogService catalog, CustomerService customers, SalesService sales, ReportService reports)
    {
        _catalog = catalog;
        _customers = customers;
        _sales = sales;
        _reports = reports;
    }

    public async Task RunAsync()
    {
        var running = true;
        while (running)
        {
            Print.Header("BookStore - Main Menu");
            var options = new[]
            {
                "Manage books",
                "Manage customers",
                "Record a purchase",
                "Reports (revenue, best seller, top customer)",
                "Apply a price rule to books",
                "Save & exit"
            };
            var choice = ConsoleInput.ReadChoiceIndex("What would you like to do?", options);

            try
            {
                switch (choice)
                {
                    case 0: await BookMenuAsync(); break;
                    case 1: CustomerMenu(); break;
                    case 2: await RecordPurchaseAsync(); break;
                    case 3: ShowReports(); break;
                    case 4: ApplyPriceRule(); break;
                    case 5: running = false; break;
                }
            }
            catch (ValidationException ex)
            {
                Print.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Print.Error($"Something unexpected happened: {ex.Message}");
            }
        }
    }


    private async Task BookMenuAsync()
    {
        var options = new[]
        {
            "Add a book",
            "Remove a book",
            "Search books (title/author)",
            "List all books",
            "Filter by category",
            "Filter by author",
            "Filter by price range",
            "Back"
        };
        var choice = ConsoleInput.ReadChoiceIndex("Books menu:", options);

        switch (choice)
        {
            case 0: AddBook(); break;
            case 1: RemoveBook(); break;
            case 2: SearchBooks(); break;
            case 3: ListBooks(_catalog.ListAll()); break;
            case 4: ListBooks(_catalog.FilterByCategory(ConsoleInput.ReadNonEmptyString("Category")).ToList()); break;
            case 5: ListBooks(_catalog.FilterByAuthor(ConsoleInput.ReadNonEmptyString("Author")).ToList()); break;
            case 6: FilterByPriceRange(); break;
            case 7: return;
        }
        await Task.CompletedTask;
    }

    private void AddBook()
    {
        Print.Header("Add a Book");
        var formats = BookFactory.AvailableFormats.ToList();
        var formatIndex = ConsoleInput.ReadChoiceIndex("Book format:", formats);
        var format = formats[formatIndex];

        var title = ConsoleInput.ReadNonEmptyString("Title");
        var author = ConsoleInput.ReadNonEmptyString("Author");
        var category = ConsoleInput.ReadNonEmptyString("Category");
        var isbn = ConsoleInput.ReadNonEmptyString("ISBN (10 or 13 digits)");
        var price = ConsoleInput.ReadDecimal("Price");
        var stock = ConsoleInput.ReadInt("Initial stock");

        var extra = new Dictionary<string, string>();
        switch (format)
        {
            case "Paperback":
                extra["pageCount"] = ConsoleInput.ReadInt("Page count").ToString();
                break;
            case "Ebook":
                extra["fileSizeMb"] = ConsoleInput.ReadDouble("File size (MB)").ToString();
                extra["fileType"] = ConsoleInput.ReadNonEmptyString("File type (e.g. PDF, EPUB)");
                break;
            case "Audiobook":
                extra["duration"] = ConsoleInput.ReadDuration("Duration").ToString();
                extra["narrator"] = ConsoleInput.ReadNonEmptyString("Narrator");
                break;
            default:
                break;
        }

        var request = new BookCreationRequest
        {
            Title = title,
            Author = author,
            Category = category,
            Isbn = isbn,
            Price = price,
            Stock = stock,
            ExtraFields = extra
        };

        var book = _catalog.AddBook(format, request);
        Print.Success($"Added '{book.Title}' ({book.Format}) - Id: {book.Id}");
    }

    private void RemoveBook()
    {
        Print.Header("Remove a Book");
        if (_catalog.ListAll().Count == 0)
        {
            Print.Warn("There are no books to remove.");
            return;
        }
        ListBooks(_catalog.ListAll());
        var id = ConsoleInput.ReadGuid("Enter the Id of the book to remove");
        if (_catalog.RemoveBook(id))
            Print.Success("Book removed.");
        else
            Print.Error("No book found with that Id.");
    }

    private void SearchBooks()
    {
        var term = ConsoleInput.ReadNonEmptyString("Search term (title or author)");
        ListBooks(_catalog.SearchByTitleOrAuthor(term).ToList());
    }

    private void FilterByPriceRange()
    {
        var min = ConsoleInput.ReadDecimal("Minimum price");
        var max = ConsoleInput.ReadDecimal("Maximum price");
        ListBooks(_catalog.FilterByPriceRange(min, max).ToList());
    }

    private static void ListBooks(IReadOnlyList<BookBase> books)
    {
        Print.Header($"Books ({books.Count})");
        if (books.Count == 0)
        {
            Print.Info("No books to show.");
            return;
        }
        foreach (var book in books)
        {
            Console.WriteLine($"  [{book.Id}]");
            Console.WriteLine($"    {book.Title} by {book.Author} - {book.Format} ({book.FormatDetails})");
            Console.WriteLine($"    Category: {book.Category} | Price: {book.Price:C} | Stock: {book.Stock} | ISBN: {book.Isbn}");
        }
    }

    private void ApplyPriceRule()
    {
        Print.Header("Apply a Price Rule");
        var books = _catalog.ListAll();
        if (books.Count == 0)
        {
            Print.Warn("There are no books yet.");
            return;
        }

        var options = new[] { "Apply a percentage discount", "Apply a flat price adjustment", "Back" };
        var choice = ConsoleInput.ReadChoiceIndex("Choose a rule:", options);
        if (choice == 2) return;

        IEnumerable<BookBase> target = books;
        if (ConsoleInput.ReadYesNo("Limit this to one category?"))
        {
            var category = ConsoleInput.ReadNonEmptyString("Category");
            target = target.ByCategory(category);
        }

        if (choice == 0)
        {
            var percent = ConsoleInput.ReadDecimal("Discount percentage (e.g. 10 for 10%)");
            target.ApplyRule(b => b.UpdatePrice(Math.Round(b.Price * (1 - percent / 100m), 2)));
            Print.Success($"Applied a {percent}% discount.");
        }
        else
        {
            var amount = ConsoleInput.ReadDecimal("Flat amount to subtract from price");
            target.ApplyRule(b => b.UpdatePrice(Math.Max(0, b.Price - amount)));
            Print.Success($"Reduced price by {amount:C} where applicable.");
        }
    }


    private void CustomerMenu()
    {
        var options = new[] { "Register a customer", "List customers", "Back" };
        var choice = ConsoleInput.ReadChoiceIndex("Customers menu:", options);

        switch (choice)
        {
            case 0: RegisterCustomer(); break;
            case 1: ListCustomers(); break;
        }
    }

    private void RegisterCustomer()
    {
        Print.Header("Register a Customer");
        var name = ConsoleInput.ReadNonEmptyString("Name");
        var email = ConsoleInput.ReadNonEmptyString("Email");
        var customer = _customers.Register(name, email);
        Print.Success($"Registered {customer.Name} - Id: {customer.Id}");
    }

    private void ListCustomers()
    {
        var customers = _customers.ListAll();
        Print.Header($"Customers ({customers.Count})");
        if (customers.Count == 0)
        {
            Print.Info("No customers registered yet.");
            return;
        }
        foreach (var c in customers)
            Console.WriteLine($"  [{c.Id}] {c.Name} <{c.Email}>");
    }

    private async Task RecordPurchaseAsync()
    {
        Print.Header("Record a Purchase");
        if (_customers.ListAll().Count == 0)
        {
            Print.Warn("Register a customer first.");
            return;
        }
        if (_catalog.ListAll().Count == 0)
        {
            Print.Warn("There are no books to sell yet.");
            return;
        }

        ListCustomers();
        var customerId = ConsoleInput.ReadGuid("Customer Id");

        var lines = new List<PurchaseRequestLine>();
        var addingMore = true;
        while (addingMore)
        {
            ListBooks(_catalog.ListAll());
            var bookId = ConsoleInput.ReadGuid("Book Id to add to this purchase");
            var quantity = ConsoleInput.ReadInt("Quantity");
            lines.Add(new PurchaseRequestLine(bookId, quantity));
            addingMore = ConsoleInput.ReadYesNo("Add another book to this purchase?");
        }

        var purchase = await _sales.RecordPurchaseAsync(customerId, lines);
        Print.Success($"Purchase recorded. {purchase.Lines.Count} line(s), total {purchase.Total:C}.");
    }


    private void ShowReports()
    {
        Print.Header("Reports");
        Console.WriteLine($"  Total revenue: {_reports.TotalRevenue():C}");

        var bestSeller = _reports.BestSellingBook();
        Console.WriteLine(bestSeller is null
            ? "  Best-selling book: no sales yet."
            : $"  Best-selling book: {bestSeller.Book.Title} ({bestSeller.UnitsSold} units sold)");

        var topCustomer = _reports.TopCustomer();
        Console.WriteLine(topCustomer is null
            ? "  Top customer: no sales yet."
            : $"  Top customer: {topCustomer.Customer.Name} ({topCustomer.TotalSpent:C} spent)");
    }
}
