USE EcomTraining;
GO

-- Joins
SELECT o.OrderId, c.FirstName, c.LastName, oi.Quantity, p.Name, oi.UnitPrice
FROM sales.Orders o
JOIN sales.Customers c ON c.CustomerId = o.CustomerId
JOIN sales.OrderItems oi ON oi.OrderId = o.OrderId
JOIN sales.Products p ON p.ProductId = oi.ProductId;

-- Set ops
SELECT Email FROM sales.Customers
UNION
SELECT 'guest@example.com';
