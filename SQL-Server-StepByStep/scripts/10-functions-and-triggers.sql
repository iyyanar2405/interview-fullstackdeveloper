USE EcomTraining;
GO

-- Scalar function: compute line total
IF OBJECT_ID('sales.ufn_LineTotal') IS NOT NULL DROP FUNCTION sales.ufn_LineTotal;
GO
CREATE FUNCTION sales.ufn_LineTotal(@qty INT, @price DECIMAL(10,2)) RETURNS DECIMAL(10,2)
AS BEGIN RETURN @qty * @price END;
GO

-- Trigger: auto-calc UnitPrice from Products on insert
IF OBJECT_ID('sales.trg_OrderItems_SetUnitPrice') IS NOT NULL DROP TRIGGER sales.trg_OrderItems_SetUnitPrice;
GO
CREATE TRIGGER sales.trg_OrderItems_SetUnitPrice ON sales.OrderItems
INSTEAD OF INSERT AS
BEGIN
  SET NOCOUNT ON;
  INSERT INTO sales.OrderItems(OrderId, ProductId, Quantity, UnitPrice)
  SELECT i.OrderId, i.ProductId, i.Quantity, p.Price
  FROM inserted i
  JOIN sales.Products p ON p.ProductId = i.ProductId;
END
GO
