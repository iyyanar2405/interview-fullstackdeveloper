USE EcomTraining;
GO

-- Top products by revenue
SELECT TOP 5 p.Name, SUM(oi.Quantity * oi.UnitPrice) AS Revenue
FROM sales.OrderItems oi
JOIN sales.Products p ON p.ProductId = oi.ProductId
GROUP BY p.Name
ORDER BY Revenue DESC;

-- Simple cohort by order month
SELECT FORMAT(o.OrderDate, 'yyyy-MM') AS OrderMonth, COUNT(DISTINCT o.CustomerId) AS ActiveCustomers
FROM sales.Orders o
GROUP BY FORMAT(o.OrderDate, 'yyyy-MM')
ORDER BY OrderMonth;
