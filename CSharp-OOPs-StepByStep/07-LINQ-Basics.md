# 07 — LINQ Basics

Concepts: Where, Select, OrderBy, GroupBy.

```csharp
var adults = people.Where(p => p.Age >= 18).Select(p => p.FullName());
```
