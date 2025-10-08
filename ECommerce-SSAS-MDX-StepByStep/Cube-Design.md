# Cube Design — Dimensions, Hierarchies, Measures

## Dimensions
- Date: Year → Quarter → Month → Day; set attribute types
- Product: Category → Subcategory → Product Name; define attribute relationships
- Customer: Country → State → City → Customer
- Geography: Country → State → City

## Measures
- From FactSales: Sales Amount, Quantity, Order Count, Gross Margin
- Add calculated measures in Calculations tab (e.g., Avg Order Value)

## Dimension Usage
- Confirm relationships (Regular, Reference, Many-to-Many if needed)
- Set granularity attribute and correct keys

## Partitions & Aggregations
- Partition FactSales by Year
- Design aggregations for common query paths (Year/Month, Category)

## Security & Perspectives
- Roles: row-level filters for Customer/Geography
- Perspectives: Sales, Marketing
