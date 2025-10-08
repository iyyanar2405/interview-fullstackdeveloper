# MDX Queries — Beginner → Advanced

Use SSMS (connect to Analysis Services) and open a new MDX query window.

## Basics

1) Select all measures by Calendar Year
SELECT
  { Measures.AllMembers } ON COLUMNS,
  { [Date].[Calendar].[Calendar Year].Members } ON ROWS
FROM [Sales]

2) Slice by a Product Category
SELECT
  { [Measures].[Sales Amount] } ON COLUMNS,
  { [Date].[Calendar].[Month].Members } ON ROWS
FROM [Sales]
WHERE ( [Product].[Category].&[Bikes] )

3) Tuple vs Set example
-- Tuple (single coordinate): ([Measures].[Sales Amount], [Date].[Calendar].[Calendar Year].&[2024])
-- Set (multiple coords): { [Date].[Calendar].[Calendar Year].&[2024], [Date].[Calendar].[Calendar Year].&[2025] }

## Time Intelligence

4) YTD Sales
WITH MEMBER [Measures].[Sales YTD] AS
  SUM( YTD([Date].[Calendar].CurrentMember), [Measures].[Sales Amount] )
SELECT [Measures].[Sales YTD] ON COLUMNS,
       [Date].[Calendar].[Month].Members ON ROWS
FROM [Sales]

5) Prior Year (PY) and YOY Delta
WITH
  MEMBER [Measures].[Sales PY] AS ( [Measures].[Sales Amount], ParallelPeriod([Date].[Calendar].[Calendar Year], 1) )
  MEMBER [Measures].[Sales YOY] AS ( [Measures].[Sales Amount] - [Measures].[Sales PY] )
SELECT { [Measures].[Sales Amount], [Measures].[Sales PY], [Measures].[Sales YOY] } ON COLUMNS,
       [Date].[Calendar].[Month].Members ON ROWS
FROM [Sales]

6) MTD / QTD
WITH
  MEMBER [Measures].[Sales MTD] AS SUM( MTD([Date].[Calendar].CurrentMember), [Measures].[Sales Amount] )
  MEMBER [Measures].[Sales QTD] AS SUM( QTD([Date].[Calendar].CurrentMember), [Measures].[Sales Amount] )
SELECT { [Measures].[Sales MTD], [Measures].[Sales QTD] } ON COLUMNS,
       [Date].[Calendar].[Day].Members ON ROWS
FROM [Sales]

## Ranking & Filters

7) Top 10 Products by Sales Amount in 2024
WITH
  SET [TopProducts] AS TOPCOUNT( [Product].[Product Name].Members, 10, [Measures].[Sales Amount] )
SELECT [Measures].[Sales Amount] ON COLUMNS,
       [TopProducts] ON ROWS
FROM [Sales]
WHERE ( [Date].[Calendar].[Calendar Year].&[2024] )

8) Customers with Sales > 1000
SELECT [Measures].[Sales Amount] ON COLUMNS,
       FILTER( [Customer].[Customer].Members, [Measures].[Sales Amount] > 1000 ) ON ROWS
FROM [Sales]

9) Percent of Total by Category
WITH MEMBER [Measures].[Pct of Total] AS
  IIF( [Measures].[Sales Amount] = NULL OR ([Measures].[Sales Amount] = 0), NULL,
       [Measures].[Sales Amount] / ( [Measures].[Sales Amount], [Product].[Category].[All].Parent ) )
SELECT { [Measures].[Sales Amount], [Measures].[Pct of Total] } ON COLUMNS,
       [Product].[Category].Members ON ROWS
FROM [Sales]

## Named Sets, KPIs, Calculations

10) Named Set for Current Year Months
CREATE SET CURRENTCUBE.[CurrentYearMonths] AS
  FILTER( [Date].[Calendar].[Month].Members,
          [Date].[Calendar].CurrentMember.Parent IS [Date].[Calendar].[Calendar Year].CurrentMember );

11) KPI Example (defined in Cube UI)
-- KPI Value: [Measures].[Gross Margin]
-- Goal: target margin from a measure or constant
-- Status: traffic light via banding
-- Trend: month-over-month change

## Advanced: SCOPE

12) SCOPE assignment to override a calculation for a specific category
SCOPE( [Product].[Category].&[Accessories] );
  THIS = [Measures].[Sales Amount] * 0.98; -- 2% promo adjustment example
END SCOPE;

---

## More Advanced Patterns (E‑Commerce)

13) Rolling 12 Months Sales
WITH MEMBER [Measures].[Sales R12] AS
  SUM( LastPeriods(12, [Date].[Calendar].CurrentMember), [Measures].[Sales Amount] )
SELECT { [Measures].[Sales Amount], [Measures].[Sales R12] } ON COLUMNS,
       [Date].[Calendar].[Month].Members ON ROWS
FROM [Sales]

14) Distinct Customers (unique count) via DistinctCount measure
-- In cube: add DistinctCount on CustomerKey in the measure group
SELECT { [Measures].[Distinct Customers] } ON COLUMNS,
       [Date].[Calendar].[Calendar Year].Members ON ROWS
FROM [Sales]

15) Conversion Rate (Orders/Distinct Customers)
WITH MEMBER [Measures].[Conversion Rate] AS
  IIF([Measures].[Distinct Customers] = 0, NULL,
      [Measures].[Order Count] / [Measures].[Distinct Customers])
SELECT { [Measures].[Order Count], [Measures].[Distinct Customers], [Measures].[Conversion Rate] } ON COLUMNS,
       [Date].[Calendar].[Month].Members ON ROWS
FROM [Sales]

16) Top 5 per Category (per-group ranking)
WITH
  SET [CatProds] AS [Product].[Product Name].Members
  SET [Top5EachCat] AS GENERATE(
    [Product].[Category].Members,
    TOPCOUNT( EXISTING ([CatProds]), 5, [Measures].[Sales Amount] )
  )
SELECT [Measures].[Sales Amount] ON COLUMNS,
       [Top5EachCat] ON ROWS
FROM [Sales]

17) Pareto 80/20 (products contributing to 80% of sales)
WITH
  SET [Sorted] AS ORDER([Product].[Product Name].Members, [Measures].[Sales Amount], DESC)
  MEMBER [Measures].[CumPct] AS
    SUM( HEAD([Sorted], RANK([Product].[Product Name].CurrentMember, [Sorted]) ), [Measures].[Sales Amount])
      / SUM([Sorted], [Measures].[Sales Amount])
  SET [Pareto] AS FILTER([Sorted], [Measures].[CumPct] <= 0.8)
SELECT { [Measures].[Sales Amount], [Measures].[CumPct] } ON COLUMNS,
       [Pareto] ON ROWS
FROM [Sales]

18) Subselect vs WHERE slicer
-- Subselect restricts cube space before calc; WHERE is a slicer applied after
SELECT [Measures].[Sales Amount] ON COLUMNS,
       [Date].[Calendar].[Month].Members ON ROWS
FROM ( SELECT ( [Product].[Category].&[Bikes] ) ON 0 FROM [Sales] )
-- Compare to:
-- FROM [Sales]
-- WHERE ( [Product].[Category].&[Bikes] )

19) Parameterized Set for Selected Categories
WITH SET [SelectedCats] AS { [Product].[Category].&[Bikes], [Product].[Category].&[Accessories] }
SELECT [Measures].[Sales Amount] ON COLUMNS,
       CROSSJOIN([SelectedCats], [Date].[Calendar].[Calendar Year].Members) ON ROWS
FROM [Sales]

20) KPI Status via thresholds
WITH MEMBER [Measures].[GM %] AS [Measures].[Gross Margin] / NULLIF([Measures].[Sales Amount], 0)
     MEMBER [Measures].[GM Status] AS
       CASE WHEN [Measures].[GM %] >= 0.35 THEN 1 WHEN [Measures].[GM %] >= 0.25 THEN 0 ELSE -1 END
SELECT { [Measures].[GM %], [Measures].[GM Status] } ON COLUMNS,
       [Date].[Calendar].[Month].Members ON ROWS
FROM [Sales]

21) Semi-additive measure (LastNonEmpty)
-- Define measure agg as LastNonEmpty (e.g., Inventory)
SELECT [Measures].[Inventory] ON COLUMNS,
       [Date].[Calendar].[Month].Members ON ROWS
FROM [Sales]

22) Many-to-Many dimension check
-- If you have a M2M bridge, ensure dimension usage has M2M type; test:
SELECT [Measures].[Sales Amount] ON COLUMNS,
       [YourM2MDimension].[YourAttribute].Members ON ROWS
FROM [Sales]

23) Currency conversion sketch (multi-currency)
WITH MEMBER [Measures].[Sales USD] AS [Measures].[Sales Amount] * ([Exchange].[ToUSD].CurrentMember)
SELECT { [Measures].[Sales Amount], [Measures].[Sales USD] } ON COLUMNS,
       [Date].[Calendar].[Month].Members ON ROWS
FROM [Sales]

24) Running total within Year
WITH MEMBER [Measures].[Sales Run] AS
  SUM( PERIODSTODATE([Date].[Calendar].[Calendar Year], [Date].[Calendar].CurrentMember), [Measures].[Sales Amount] )
SELECT { [Measures].[Sales Amount], [Measures].[Sales Run] } ON COLUMNS,
       [Date].[Calendar].[Month].Members ON ROWS
FROM [Sales]

25) Customers with YoY Growth > 10%
WITH MEMBER [Measures].[PY] AS ( [Measures].[Sales Amount], ParallelPeriod([Date].[Calendar].[Calendar Year], 1) )
     MEMBER [Measures].[YoY%] AS IIF([Measures].[PY] = 0, NULL, ([Measures].[Sales Amount] - [Measures].[PY]) / [Measures].[PY])
SELECT { [Measures].[Sales Amount], [Measures].[PY], [Measures].[YoY%] } ON COLUMNS,
       FILTER([Customer].[Customer].Members, [Measures].[YoY%] > 0.1) ON ROWS
FROM [Sales]

26) Top Cities per Country (TopCount inside Generate)
WITH SET [TopCitiesPerCountry] AS GENERATE(
  [Geography].[Country].Members,
  TOPCOUNT( EXISTING [Geography].[City].Members, 3, [Measures].[Sales Amount] )
)
SELECT [Measures].[Sales Amount] ON COLUMNS,
       [TopCitiesPerCountry] ON ROWS
FROM [Sales]

27) Basket affinity (co-occurrence sketch)
-- Requires a basket/Order dimension; approximate with FILTER + EXISTS
SELECT [Measures].[Order Count] ON COLUMNS,
       FILTER([Product].[Product Name].Members,
              EXISTS([Product].[Product Name].CurrentMember, [Order].[Order].Members).Count > 0) ON ROWS
FROM [Sales]

28) Sales vs Target variance
WITH MEMBER [Measures].[Var] AS [Measures].[Sales Amount] - [Measures].[Sales Target]
     MEMBER [Measures].[Var %] AS [Measures].[Var] / NULLIF([Measures].[Sales Target],0)
SELECT { [Measures].[Sales Amount], [Measures].[Sales Target], [Measures].[Var], [Measures].[Var %] } ON COLUMNS,
       [Date].[Calendar].[Month].Members ON ROWS
FROM [Sales]

29) Subcube for a specific Year and Category (named subcube)
CREATE SUBCUBE [Sales].[CYBikes] AS SELECT ([Date].[Calendar].[Calendar Year].&[2024], [Product].[Category].&[Bikes]) ON 0 FROM [Sales];
SELECT [Measures].[Sales Amount] ON COLUMNS, [Date].[Calendar].[Month].Members ON ROWS FROM [Sales].[CYBikes]

30) Server-side named calculation (MDX script) note
-- Prefer cube Calculations for reusable calcs; use query WITH for ad-hoc analysis
