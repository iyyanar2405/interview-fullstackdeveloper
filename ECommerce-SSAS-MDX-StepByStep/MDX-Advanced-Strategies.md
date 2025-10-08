# MDX Advanced Strategies — Patterns and Pitfalls

This guide explains advanced modeling and MDX scripting techniques for robust e‑commerce analytics.

## Calculation Script Patterns
- Define reusable calculations in the cube script; use `WITH` only for ad-hoc queries
- Order matters: time intel first, then ratios; set `SOLVE_ORDER` when needed

## SCOPE Assignments
- Use `SCOPE` to override calculations for specific subcubes (e.g., promo categories)
- Keep scopes narrow; exit scopes properly; document rationale

## Semi-Additive Measures
- Use `LastNonEmpty` (inventory/balances); ensure a proper Time dimension
- For snapshots, store at period end and mark calculations accordingly

## Many-to-Many Dimensions
- Use a bridge measure group; configure Dimension Usage as M2M
- Be mindful of performance; pre-aggregate when possible

## Currency Conversion
- Store FX rates in a dimension/measure group
- Calculate normalized currency (e.g., USD) early in the script

## Subselect vs WHERE
- Subselect reduces cube space before calculations; WHERE is a slicer applied after
- Test both if results differ around calculated members

## Parameterization
- Use Named Sets and calculated members to emulate parameters
- BI tools can pass selections as slicers; expose user-friendly hierarchies

## Partitions & Aggregations
- Partition large fact groups by Year/Month
- Design aggregations for common axes (Year→Month, Category) and reprocess when schema changes

## Security Considerations
- Role filters can affect calc results; test with `Change User`
- Avoid exposing surrogate key attributes; use friendly names

## Performance Tuning Checklist
- Attribute relationships set; key columns compact
- Avoid expensive calcs on large axes; consider pre-calcs in ETL
- Warm caches post-processing; monitor query perf in Profiler/Extended Events
