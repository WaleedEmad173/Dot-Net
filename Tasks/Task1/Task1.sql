-- =========================
-- Task 1: Create Tables
-- =========================

CREATE TABLE Authors (
    AuthorId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Books (
    BookId INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(200) NOT NULL,
    Price DECIMAL(10,2) CHECK (Price > 0),
    Stock INT CHECK (Stock >= 0),
    AuthorId INT FOREIGN KEY REFERENCES Authors(AuthorId),
    CategoryId INT FOREIGN KEY REFERENCES Categories(CategoryId)
);

CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) UNIQUE NOT NULL,
    City NVARCHAR(100)
);

CREATE TABLE Purchases (
    PurchaseId INT PRIMARY KEY IDENTITY,
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId),
    PurchaseDate DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE PurchaseDetails (
    PurchaseDetailId INT PRIMARY KEY IDENTITY,
    PurchaseId INT FOREIGN KEY REFERENCES Purchases(PurchaseId),
    BookId INT FOREIGN KEY REFERENCES Books(BookId),
    Quantity INT CHECK (Quantity > 0),
    PriceAtPurchase DECIMAL(10,2) CHECK (PriceAtPurchase > 0)
);

-- =========================
-- Task 2: Insert Sample Data
-- =========================

INSERT INTO Authors (Name) VALUES 
('Ahmed Khaled Tawfik'), 
('Naguib Mahfouz'), 
('Stephen King');

INSERT INTO Categories (Name) VALUES 
('Fiction'), 
('Horror'), 
('Science'), 
('History');

INSERT INTO Books (Title, Price, Stock, AuthorId, CategoryId) VALUES
('Utopia', 150, 10, 1, 1),
('The Harafish', 200, 5, 2, 1),
('IT', 300, 8, 3, 2),
('History of Egypt', 250, 12, 2, 4),
('Science Basics', 180, 20, 1, 3),
('Pet Sematary', 220, 7, 3, 2);

INSERT INTO Customers (Name, Email, City) VALUES
('Ali Hassan', 'ali@test.com', 'Cairo'),
('Sara Mohamed', 'sara@test.com', 'Alexandria'),
('Waleed Emad', 'waleed@test.com', 'Cairo'),
('Mona Adel', 'mona@test.com', 'Giza');

INSERT INTO Purchases (CustomerId, PurchaseDate) VALUES
(1, '2026-06-01'),
(2, '2026-06-02'),
(1, '2026-06-05'),
(3, '2026-06-10');

INSERT INTO PurchaseDetails (PurchaseId, BookId, Quantity, PriceAtPurchase) VALUES
(1, 1, 2, 150),
(1, 2, 1, 200),
(2, 3, 1, 300),
(3, 4, 2, 250),
(4, 1, 1, 150),
(4, 6, 1, 220);

-- =========================
-- Task 3: List all books sorted by price
-- =========================
SELECT Title, Price FROM Books ORDER BY Price DESC;

-- =========================
-- Task 4: Titles uppercase, authors lowercase
-- =========================
SELECT UPPER(b.Title) AS TitleUpper, LOWER(a.Name) AS AuthorLower
FROM Books b
JOIN Authors a ON b.AuthorId = a.AuthorId;

-- =========================
-- Task 5: Every book with category and author
-- =========================
SELECT b.Title, c.Name AS Category, a.Name AS Author
FROM Books b
JOIN Categories c ON b.CategoryId = c.CategoryId
JOIN Authors a ON b.AuthorId = a.AuthorId;

-- =========================
-- Task 6: Every customer with number of purchases
-- =========================
SELECT c.Name, COUNT(p.PurchaseId) AS PurchaseCount
FROM Customers c
LEFT JOIN Purchases p ON c.CustomerId = p.CustomerId
GROUP BY c.Name;

-- =========================
-- Task 7: Top 5 best-selling books
-- =========================
SELECT TOP 5 b.Title, SUM(pd.Quantity) AS TotalSold
FROM PurchaseDetails pd
JOIN Books b ON pd.BookId = b.BookId
GROUP BY b.Title
ORDER BY TotalSold DESC;

-- =========================
-- Task 8: City with highest number of customers
-- =========================
SELECT TOP 1 City, COUNT(*) AS CustomerCount
FROM Customers
GROUP BY City
ORDER BY CustomerCount DESC;

-- =========================
-- Task 9: Categories with more than 5 books
-- =========================
SELECT c.Name, COUNT(b.BookId) AS BookCount
FROM Categories c
JOIN Books b ON c.CategoryId = b.CategoryId
GROUP BY c.Name
HAVING COUNT(b.BookId) > 5;

-- =========================
-- Task 10: Books costing more than average price
-- =========================
SELECT Title, Price
FROM Books
WHERE Price > (SELECT AVG(Price) FROM Books);

-- =========================
-- Task 11: Customers who never purchased
-- =========================
SELECT c.Name
FROM Customers c
LEFT JOIN Purchases p ON c.CustomerId = p.CustomerId
WHERE p.PurchaseId IS NULL;

-- =========================
-- Task 12: Total revenue per month
-- =========================
SELECT YEAR(p.PurchaseDate) AS Year, MONTH(p.PurchaseDate) AS Month,
       SUM(pd.Quantity * pd.PriceAtPurchase) AS Revenue
FROM Purchases p
JOIN PurchaseDetails pd ON p.PurchaseId = pd.PurchaseId
GROUP BY YEAR(p.PurchaseDate), MONTH(p.PurchaseDate)
ORDER BY Year, Month;

-- =========================
-- Task 13: Create a view (Book Info)
-- =========================
CREATE VIEW BookInfo AS
SELECT b.Title, c.Name AS Category, a.Name AS Author, b.Price
FROM Books b
JOIN Categories c ON b.CategoryId = c.CategoryId
JOIN Authors a ON b.AuthorId = a.AuthorId;

-- =========================
-- Task 14: Stored Procedure for purchases of a customer
-- =========================
CREATE PROCEDURE GetCustomerPurchases
    @CustomerId INT
AS
BEGIN
    SELECT p.PurchaseId, p.PurchaseDate, b.Title, pd.Quantity, pd.PriceAtPurchase,
           (pd.Quantity * pd.PriceAtPurchase) AS Total
    FROM Purchases p
    JOIN PurchaseDetails pd ON p.PurchaseId = pd.PurchaseId
    JOIN Books b ON pd.BookId = b.BookId
    WHERE p.CustomerId = @CustomerId;
END;