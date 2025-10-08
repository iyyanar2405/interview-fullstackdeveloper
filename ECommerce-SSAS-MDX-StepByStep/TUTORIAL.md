# SSAS & MDX Tutorial — E‑Commerce

This tutorial walks from database setup to cube design and MDX analytics for sales.

## 1) Environment
- Install SQL Server Database Engine and Analysis Services (Multidimensional)
- Install SSMS and Visual Studio + SSDT BI templates

## 2) Sample Data
- Run `SampleData-Scripts.sql` to create tables and seed data
- Tables: DimDate, DimProduct, DimCustomer, DimGeography, FactSales

## 3) Star Schema
- Create relationships: FactSales keys → dimension surrogate keys
- Validate referential integrity and null handling

## 4) SSAS Project
- Create a new Multidimensional project
- Add a Data Source to your SQL DB
- Create Data Source View selecting FactSales and dimensions

## 5) Dimensions
- Create Date, Product, Customer, Geography dimensions
- Add attribute relationships (e.g., Date → Month → Quarter → Year)
- Define hierarchies and set attribute types (e.g., Year, MonthName)

## 6) Cube
- Create a cube, add FactSales as the measure group
- Define measures: Sales Amount, Order Count, Quantity
- Add dimension usage and confirm granularity

## 7) Calculations & Time Intelligence
- In the cube Calculations tab, add YTD/MTD/YOY calculations (see MDX-Queries.md)
- Add KPIs for Gross Margin, Conversion Rate

## 8) Deploy & Process
- Deploy to local SSAS instance
- Process dimensions then cube (Full), test MDX in SSMS

## 9) Incremental Processing
- Create partitions by Year for FactSales
- Implement ProcessAdd for incremental loads

## 10) Security & Perspectives
- Create roles restricting Customer/Geography members
- Add perspectives for Sales vs Marketing views

## 11) Performance
- Attribute relationships; aggregations; cache warming query
- Avoid unnecessary calculated members on large axes

## 12) Automation
- Schedule SQL Agent jobs (process nightly) or use SSIS/ADF

## 13) Capstone
- Build 3 analytics stories (e.g., Top products by region; YOY growth; Customer LTV)
