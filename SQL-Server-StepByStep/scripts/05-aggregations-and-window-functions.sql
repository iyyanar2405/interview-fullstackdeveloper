USE EcomTraining;
GO

-- Aggregations
SELECT o.CustomerId, SUM(oi.Quantity * oi.UnitPrice) AS Total
FROM sales.Orders o
JOIN sales.OrderItems oi ON oi.OrderId = o.OrderId
GROUP BY o.CustomerId
HAVING SUM(oi.Quantity * oi.UnitPrice) > 50;

-- Window functions
SELECT o.OrderId,
       SUM(oi.Quantity * oi.UnitPrice) OVER (PARTITION BY o.CustomerId ORDER BY o.OrderId ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS RunningTotal
FROM sales.Orders o
JOIN sales.OrderItems oi ON oi.OrderId = o.OrderId;
