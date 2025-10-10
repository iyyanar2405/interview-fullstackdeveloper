USE EcomTraining;
GO

-- Index on Orders(CustomerId)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Orders_CustomerId')
CREATE INDEX IX_Orders_CustomerId ON sales.Orders(CustomerId);

-- Covering index for OrderItems(OrderId, ProductId, Quantity, UnitPrice)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OrderItems_OrderId_ProductId')
CREATE INDEX IX_OrderItems_OrderId_ProductId ON sales.OrderItems(OrderId, ProductId) INCLUDE (Quantity, UnitPrice);

-- Basic perf check using SET STATISTICS IO/TIME ON (run in SSMS)
