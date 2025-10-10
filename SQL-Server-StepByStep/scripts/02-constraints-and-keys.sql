USE EcomTraining;
GO

-- Foreign keys
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Orders_Customers')
ALTER TABLE sales.Orders ADD CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId)
  REFERENCES sales.Customers(CustomerId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_OrderItems_Orders')
ALTER TABLE sales.OrderItems ADD CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId)
  REFERENCES sales.Orders(OrderId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_OrderItems_Products')
ALTER TABLE sales.OrderItems ADD CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId)
  REFERENCES sales.Products(ProductId);
GO

-- Uniques
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Customers_Email')
ALTER TABLE sales.Customers ADD CONSTRAINT UQ_Customers_Email UNIQUE (Email);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Products_Sku')
ALTER TABLE sales.Products ADD CONSTRAINT UQ_Products_Sku UNIQUE (Sku);
GO
