using BookStore.App.Menu;
using BookStore.Core.Entities;
using BookStore.Core.Events;
using BookStore.Core.Persistence;
using BookStore.Core.Repositories;
using BookStore.Core.Services;
using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
Console.OutputEncoding = System.Text.Encoding.UTF8;

Print.Header("Welcome to BookStore Console");

var bookRepository = new InMemoryRepository<BookBase>();
var customerRepository = new InMemoryRepository<Customer>();
var purchaseRepository = new InMemoryRepository<Purchase>();

var stockNotifier = new StockNotifier();
stockNotifier.BookOutOfStock += (_, args) =>
    Print.Warn($"Stock alert: '{args.Book.Title}' just ran out of stock!");

var catalogService = new BookCatalogService(bookRepository, stockNotifier);
var customerService = new CustomerService(customerRepository);
var salesService = new SalesService(bookRepository, customerRepository, purchaseRepository);
var reportService = new ReportService(bookRepository, customerRepository, purchaseRepository);

var dataFilePath = Path.Combine(AppContext.BaseDirectory, "bookstore-data.json");
var persistenceService = new PersistenceService(dataFilePath);

var loaded = await persistenceService.LoadAsync(catalogService, customerService, salesService);
Print.Info(loaded
    ? $"Loaded saved data from {dataFilePath}"
    : "No saved data found - starting with an empty store.");

var menu = new MainMenu(catalogService, customerService, salesService, reportService);
await menu.RunAsync();

await persistenceService.SaveAsync(catalogService, customerService, salesService);
Print.Success($"Data saved to {dataFilePath}. Goodbye!");
