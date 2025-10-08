-- Sample e-commerce star schema and seed data (minimal)
CREATE TABLE DimDate (
  DateKey INT PRIMARY KEY,
  [Date] DATE,
  [Day] TINYINT,
  [Month] TINYINT,
  [MonthName] NVARCHAR(20),
  [Quarter] TINYINT,
  [Calendar Year] INT
);

CREATE TABLE DimProduct (
  ProductKey INT IDENTITY PRIMARY KEY,
  ProductName NVARCHAR(100),
  Subcategory NVARCHAR(100),
  Category NVARCHAR(100)
);

CREATE TABLE DimCustomer (
  CustomerKey INT IDENTITY PRIMARY KEY,
  CustomerName NVARCHAR(100),
  City NVARCHAR(100),
  State NVARCHAR(100),
  Country NVARCHAR(100)
);

CREATE TABLE DimGeography (
  GeographyKey INT IDENTITY PRIMARY KEY,
  City NVARCHAR(100),
  State NVARCHAR(100),
  Country NVARCHAR(100)
);

CREATE TABLE FactSales (
  SalesKey BIGINT IDENTITY PRIMARY KEY,
  DateKey INT NOT NULL,
  ProductKey INT NOT NULL,
  CustomerKey INT NOT NULL,
  GeographyKey INT NOT NULL,
  OrderNumber NVARCHAR(50),
  Quantity INT,
  SalesAmount DECIMAL(18,2)
);

-- Minimal seed (add more rows as needed)
INSERT INTO DimProduct (ProductName, Subcategory, Category) VALUES
('Road Bike 100', 'Road Bikes', 'Bikes'),
('Helmet Standard', 'Helmets', 'Accessories');

INSERT INTO DimCustomer (CustomerName, City, State, Country) VALUES
('Alice', 'Seattle', 'WA', 'USA'),
('Bob', 'Austin', 'TX', 'USA');

INSERT INTO DimGeography (City, State, Country) VALUES
('Seattle', 'WA', 'USA'),
('Austin', 'TX', 'USA');

-- Basic dates for 2024
DECLARE @d DATE='2024-01-01';
WHILE @d < '2024-03-01'
BEGIN
  INSERT INTO DimDate (DateKey,[Date],[Day],[Month],[MonthName],[Quarter],[Calendar Year])
  VALUES (CONVERT(INT,FORMAT(@d,'yyyyMMdd')),@d,DATEPART(DAY,@d),DATEPART(MONTH,@d),DATENAME(MONTH,@d),DATEPART(QUARTER,@d),DATEPART(YEAR,@d));
  SET @d = DATEADD(DAY,1,@d);
END

-- A few sales
INSERT INTO FactSales (DateKey, ProductKey, CustomerKey, GeographyKey, OrderNumber, Quantity, SalesAmount)
SELECT TOP 10 d.DateKey, 1, 1, 1, CONCAT('ORD',ROW_NUMBER() OVER(ORDER BY d.DateKey)), 1, 999.99
FROM DimDate d ORDER BY d.[Date];
