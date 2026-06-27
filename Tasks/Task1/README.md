# BookStore SQL Database

## Description
SQL database for an online bookstore.  
Supports selling books, tracking stock, organizing by author and category, and analyzing sales and customers.  
Constraints: price > 0, stock ≥ 0, unique email, old purchases keep original price.

## ERD
Authors ───< Books >─── Categories  
             |  
             v  
      PurchaseDetails >─── Purchases >─── Customers
