USE EcomTraining;
GO

-- View: order totals
IF OBJECT_ID('sales.vw_OrderTotals') IS NOT NULL DROP VIEW sales.vw_OrderTotals;
GO
CREATE VIEW sales.vw_OrderTotals AS
SELECT o.OrderId, o.CustomerId, SUM(oi.Quantity * oi.UnitPrice) AS OrderTotal
FROM sales.Orders o
JOIN sales.OrderItems oi ON oi.OrderId = o.OrderId
GROUP BY o.OrderId, o.CustomerId;
GO

-- Stored procedure: create order
IF OBJECT_ID('sales.usp_CreateOrder') IS NOT NULL DROP PROCEDURE sales.usp_CreateOrder;
GO
CREATE PROCEDURE sales.usp_CreateOrder @CustomerId INT AS
BEGIN
  SET NOCOUNT ON;
  INSERT INTO sales.Orders(CustomerId) VALUES(@CustomerId);
  SELECT SCOPE_IDENTITY() AS NewOrderId;
END
GO
