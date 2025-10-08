# 01 — Star Schema for E‑Commerce

- Dimensions: Date, Product, Customer, Geography
- Fact: FactSales (surrogate keys to each dim)
- Add foreign keys and ensure referential integrity
- Consider degenerate dimension (OrderNumber) in FactSales
