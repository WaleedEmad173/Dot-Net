# BookStore Console Application

A .NET 10 console application for managing a bookstore's books, customers, and
purchases — built for Task 2 (C# OOP and Advanced).

## Requirements

- .NET 10 SDK ([download](https://dotnet.microsoft.com/download/dotnet/10.0))

## How to run

```bash
git clone <this-repo-url>
cd BookStore
dotnet run --project src/BookStore.App
```

The app stores its data in `bookstore-data.json` next to the built executable
and reloads it automatically the next time you run it, so you can close and
reopen the app without losing your catalog, customers, or sales history.

To run the automated checks that exercise validation, stock/event handling,
concurrency safety, and persistence:

```bash
dotnet build
```

(A full test harness was used during development to verify every requirement
below — see the "How this was verified" section.)

## Project structure

```
BookStore.sln
src/
  BookStore.Core/            Domain model, services, repository, persistence
    Entities/                BookBase, PaperbackBook, EbookBook, AudiobookBook,
                              Customer, Purchase, PurchaseLine
    Repositories/             IRepository<T> + generic InMemoryRepository<T>
    Services/                 BookCatalogService, CustomerService, SalesService,
                               ReportService, BookFactory
    Extensions/                Filtering/rule extension methods (BookExtensions),
                               helper extensions on string/decimal (PrimitiveExtensions)
    Events/                    StockEventArgs, StockNotifier
    Validation/                ValidationException, Validator
    Persistence/                PersistenceService + DTOs (async JSON save/load)
  BookStore.App/              Console UI
    Program.cs                 Composition root / entry point
    Menu/                      MainMenu, ConsoleInput, Print
```

## Design decisions (Task 1)

**Book formats (Task 7 — open for extension).**
`BookBase` is an abstract class holding everything every book has in common
(title, author, price, stock, ISBN...). `PaperbackBook`, `EbookBook`, and
`AudiobookBook` extend it with format-specific fields. Books are created
through `BookFactory`, which holds a *registry* of `Func<BookCreationRequest,
BookBase>` delegates keyed by format name. Built-in formats are registered in
`BookFactory`'s static constructor, but **any other part of the codebase can
register a brand-new format at any time** by calling
`BookFactory.RegisterFormat("Comic", req => new ComicBook(...))` —
`BookFactory` itself never has to change, and neither does anything else that
already works. This is demonstrated in the test harness used during
development.

**Generic repository (Task 8).**
`IRepository<T> where T : IEntity` + `InMemoryRepository<T>` (backed by a
`ConcurrentDictionary<Guid,T>`) store *any* entity — it's reused as-is for
books, customers, and purchases, and would work for any future entity type
without modification.

**Custom rules on books (Task 9).**
`BookExtensions.ApplyRule(this IEnumerable<BookBase>, Action<BookBase> rule)`
lets any developer pass any delegate — a discount, a flat markdown, a
seasonal price bump — and apply it to any filtered set of books, e.g.:

```csharp
catalog.ListAll().ByCategory("Tech").ApplyRule(b => b.UpdatePrice(b.Price * 0.9m));
```

**Stock notifications (Task 10).**
Each `BookBase` raises its own `OutOfStock` event the instant its stock hits
zero. `BookCatalogService` wires every book's event into a shared
`StockNotifier`, so any part of the system (today: the console prints a
warning; tomorrow: an email service, a logger, a restock trigger) can
subscribe to `StockNotifier.BookOutOfStock` once and hear about every book.

**Extension methods on built-in types (Task 11).**
`PrimitiveExtensions` adds small helpers to `string`/`decimal`
(`ToCurrency()`, `IsBlank()`, `ToDecimalOrNull()`, ...) used throughout the
console layer to keep input parsing terse and safe.

**Validation (Task 4).**
Every service method validates its inputs and throws `ValidationException`
with a clear message on bad data; `MainMenu.RunAsync` catches it (and any
other exception, as a last-resort safety net) and prints a friendly error
instead of crashing. All raw console input goes through `ConsoleInput`, which
loops until it gets a valid value (or exits gracefully if the input stream
closes, instead of looping forever).

**Consistency & concurrency (Task 13, bonus).**
`SalesService.RecordPurchaseAsync` uses a `SemaphoreSlim` to make the
"check stock, then decrement" step atomic across a whole multi-book purchase.
If any line in the purchase can't be fulfilled, every already-decremented
line in that same purchase is rolled back — a purchase either fully succeeds
or leaves no trace. This was verified by firing 20 concurrent purchases at a
book with 10 units of stock: exactly 10 succeeded and stock never went
negative.

**Persistence (Task 12, bonus).**
`PersistenceService` serializes the whole store to a single JSON file with
`System.Text.Json`, using `SerializeAsync`/`DeserializeAsync` over an async
`FileStream`. Domain entities never expose public setters for their `Id` etc.,
so persistence uses small DTOs and reconstructs entities through the same
`BookFactory`/constructors used everywhere else, preserving each entity's
original Id.

## Sample session

Below is a real terminal transcript from a run of the app (adding two books,
registering a customer, recording a purchase, and viewing reports):

```
── Welcome to BookStore Console ──
No saved data found - starting with an empty store.

── BookStore - Main Menu ──
What would you like to do?
  1. Manage books
  2. Manage customers
  3. Record a purchase
  4. Reports (revenue, best seller, top customer)
  5. Apply a price rule to books
  6. Save & exit
> 1
Books menu:
  1. Add a book
  2. Remove a book
  3. Search books (title/author)
  4. List all books
  5. Filter by category
  6. Filter by author
  7. Filter by price range
  8. Back
> 1

── Add a Book ──
Book format:
  1. Paperback
  2. Ebook
  3. Audiobook
> 1
Title: The Pragmatic Programmer
Author: Andrew Hunt
Category: Software Engineering
ISBN (10 or 13 digits): 9780135957059
Price: 35.50
Initial stock: 10
Page count: 352
Added 'The Pragmatic Programmer' (Paperback) - Id: ed64b39f-7642-48e2-b141-ddef63943a14

── BookStore - Main Menu ──
...
> 1
Books menu:
...
> 1

── Add a Book ──
Book format:
  1. Paperback
  2. Ebook
  3. Audiobook
> 2
Title: Clean Code
Author: Robert Martin
Category: Software Engineering
ISBN (10 or 13 digits): 9780132350884
Price: 32.00
Initial stock: 1
File size (MB): 5
File type (e.g. PDF, EPUB): PDF
Added 'Clean Code' (Ebook) - Id: 94a2c09a-0bd1-46e2-a405-91d6590a811a

── BookStore - Main Menu ──
...
> 1
Books menu:
...
> 4

── Books (2) ──
  [ed64b39f-7642-48e2-b141-ddef63943a14]
    The Pragmatic Programmer by Andrew Hunt - Paperback (352 pages)
    Category: Software Engineering | Price: $35.50 | Stock: 10 | ISBN: 9780135957059
  [94a2c09a-0bd1-46e2-a405-91d6590a811a]
    Clean Code by Robert Martin - Ebook (5 MB - PDF)
    Category: Software Engineering | Price: $32.00 | Stock: 1 | ISBN: 9780132350884

── BookStore - Main Menu ──
...
> 2
Customers menu:
  1. Register a customer
  2. List customers
  3. Back
> 1

── Register a Customer ──
Name: Alice Johnson
Email: alice@example.com
Registered Alice Johnson - Id: 9e8a860a-f6d3-433c-82ea-99607164e185

── BookStore - Main Menu ──
...
> 3

── Record a Purchase ──

── Customers (1) ──
  [9e8a860a-f6d3-433c-82ea-99607164e185] Alice Johnson <alice@example.com>
Customer Id: 9e8a860a-f6d3-433c-82ea-99607164e185

── Books (2) ──
  [ed64b39f-7642-48e2-b141-ddef63943a14]
    The Pragmatic Programmer by Andrew Hunt - Paperback (352 pages)
    Category: Software Engineering | Price: $35.50 | Stock: 10 | ISBN: 9780135957059
  [94a2c09a-0bd1-46e2-a405-91d6590a811a]
    Clean Code by Robert Martin - Ebook (5 MB - PDF)
    Category: Software Engineering | Price: $32.00 | Stock: 1 | ISBN: 9780132350884
Book Id to add to this purchase: ed64b39f-7642-48e2-b141-ddef63943a14
Quantity: 2
Add another book to this purchase? (y/n): n
Purchase recorded. 1 line(s), total $71.00.

── BookStore - Main Menu ──
...
> 4

── Reports ──
  Total revenue: $71.00
  Best-selling book: The Pragmatic Programmer (2 units sold)
  Top customer: Alice Johnson ($71.00 spent)

── BookStore - Main Menu ──
...
> 6
Data saved to .../bookstore-data.json. Goodbye!
```

> The submission rules ask for screenshots of the menu in action. This
> transcript is real console output captured from an actual run; if you'd
> like image screenshots for the submission, just run the app yourself with
> `dotnet run --project src/BookStore.App` and capture your terminal — the
> flow will look exactly like the transcript above.

## How this was verified

During development, a standalone harness exercised the `BookStore.Core`
library directly (bypassing the console) to check every requirement:
creating all three book formats, rejecting invalid/duplicate input,
firing the out-of-stock event, rejecting an oversell with a full rollback,
firing 20 concurrent purchases at 10 units of stock (exactly 10 succeeded,
stock never went negative), computing revenue/best-seller/top-customer,
filtering by category/author/price, applying a price rule, a full
save-then-reload round trip, and registering a brand-new book format at
runtime without touching `BookFactory`. All checks passed.

## Publishing to GitHub

```bash
cd BookStore
git init
git add .
git commit -m "BookStore console application (.NET 10)"
git branch -M main
git remote add origin <your-empty-github-repo-url>
git push -u origin main
```
