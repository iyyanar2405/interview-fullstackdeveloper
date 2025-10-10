USE EcomTraining;
GO

-- Inserts
INSERT INTO sales.Customers(FirstName, LastName, Email) VALUES
('Ada','Lovelace','ada@example.com'),
('Alan','Turing','alan@example.com');

INSERT INTO sales.Products(Sku, Name, Price) VALUES
('SKU-1','Keyboard',49.99),
('SKU-2','Mouse',19.99);

-- Create an order
DECLARE @OrderId INT;
INSERT INTO sales.Orders(CustomerId) VALUES (1);
SET @OrderId = SCOPE_IDENTITY();
INSERT INTO sales.OrderItems(OrderId, ProductId, Quantity, UnitPrice)
VALUES (@OrderId, 1, 1, 49.99), (@OrderId, 2, 2, 19.99);

-- Basic selects
SELECT * FROM sales.Customers;
SELECT * FROM sales.Products;
SELECT * FROM sales.Orders;
SELECT * FROM sales.OrderItems;

-- Update
UPDATE sales.Customers SET LastName = 'Lovelace-Byron' WHERE CustomerId = 1;

-- Delete
DELETE FROM sales.Products WHERE Sku = 'SKU-2';
