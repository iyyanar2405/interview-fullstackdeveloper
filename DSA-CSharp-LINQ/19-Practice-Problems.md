# 🧩 Practice Problems with LINQ Solutions

A curated set of interview problems showcasing concise, readable LINQ-based approaches.

---

## 1) Top K Frequent Elements
```csharp
public static class TopKFrequent
{
    public static int[] Solve(int[] nums, int k)
        => nums.GroupBy(x => x)
               .OrderByDescending(g => g.Count())
               .Take(k)
               .Select(g => g.Key)
               .ToArray();
}
```

## 2) Group by Ranges (Bucketing)
```csharp
public static class RangeBuckets
{
    public static (string Bucket, int Count)[] Bucketize(int[] nums, int size)
        => nums.GroupBy(x => $"[{(x/size)*size}-{((x/size)+1)*size - 1}]")
               .Select(g => (g.Key, g.Count()))
               .OrderBy(b => b.Key)
               .ToArray();
}
```

## 3) K Most Common Words
```csharp
public static class CommonWords
{
    public static string[] Solve(string text, int k)
        => text.Split(new[]{' ','\n','\t','\r','.',',',';','!','?'}, StringSplitOptions.RemoveEmptyEntries)
               .Select(w => w.ToLowerInvariant())
               .GroupBy(w => w)
               .OrderByDescending(g => g.Count())
               .ThenBy(g => g.Key)
               .Take(k)
               .Select(g => g.Key)
               .ToArray();
}
```

## 4) Merge Overlapping Intervals
```csharp
public static class MergeIntervalsProblem
{
    public static int[][] Merge(int[][] intervals)
    {
        if (intervals.Length <= 1) return intervals;
        return intervals
            .OrderBy(x => x[0])
            .Aggregate(new List<int[]>(), (acc, cur) =>
            {
                if (!acc.Any() || acc[^1][1] < cur[0]) acc.Add(new[] { cur[0], cur[1] });
                else acc[^1][1] = Math.Max(acc[^1][1], cur[1]);
                return acc;
            })
            .ToArray();
    }
}
```

## 5) Matrix Row/Column Sums and Argmax
```csharp
public static class MatrixAnalytics
{
    public static (int Row, int Sum)[] RowSums(int[][] m)
        => m.Select((row, r) => (r, row.Sum())).ToArray();

    public static (int Col, int Sum)[] ColSums(int[][] m)
        => Enumerable.Range(0, m[0].Length)
                      .Select(c => (c, m.Sum(row => row[c])))
                      .ToArray();

    public static (string Type, int Index, int Sum) ArgMax(int[][] m)
    {
        var rowMax = RowSums(m).OrderByDescending(x => x.Sum).First();
        var colMax = ColSums(m).OrderByDescending(x => x.Sum).First();
        return rowMax.Sum >= colMax.Sum
            ? ("Row", rowMax.Row, rowMax.Sum)
            : ("Col", colMax.Col, colMax.Sum);
    }
}
```

## 6) Inventory Reconciliation (Joins)
```csharp
public record Item(string Sku, string Name);
public record Stock(string Sku, int Qty);

public static class InventoryJoin
{
    public static (string Sku, string Name, int Qty)[] Combine(Item[] items, Stock[] stocks)
        => from i in items
           join s in stocks on i.Sku equals s.Sku into gj
           from s in gj.DefaultIfEmpty()
           select (i.Sku, i.Name, s?.Qty ?? 0);
}
```

## 7) Time Series — Moving Average
```csharp
public static class MovingAverage
{
    public static double[] Compute(double[] series, int k)
        => series.Select((_, i) => series.Skip(Math.Max(0, i - k + 1)).Take(k).Average())
                 .ToArray();
}
```

## 8) Distinct by Key
```csharp
public static class DistinctByKey
{
    public static T[] DistinctBy<T, TKey>(IEnumerable<T> items, Func<T, TKey> key)
        => items.GroupBy(key).Select(g => g.First()).ToArray();
}
```

## 9) Histogram Equalization (Toy)
```csharp
public static class Histogram
{
    public static int[] Equalize(int[] pixels)
    {
        var n = pixels.Length;
        var freq = pixels.GroupBy(p => p).OrderBy(g => g.Key).ToArray();
        var cum = freq.Select((g, i) => new { g.Key, Cum = freq.Take(i + 1).Sum(h => h.Count()) }).ToArray();
        var map = cum.ToDictionary(x => x.Key, x => (int)Math.Round((double)(x.Cum - 1) * 255 / (n - 1)));
        return pixels.Select(p => map[p]).ToArray();
    }
}
```

## 10) Calendar Merge (Events)
```csharp
public record Event(DateTime Start, DateTime End);

public static class CalendarMerge
{
    public static Event[] Merge(Event[] events)
        => events.OrderBy(e => e.Start)
                 .Aggregate(new List<Event>(), (acc, e) =>
                 {
                     if (!acc.Any() || acc[^1].End < e.Start) acc.Add(e);
                     else acc[^1] = new Event(acc[^1].Start, new DateTime(Math.Max(acc[^1].End.Ticks, e.End.Ticks)));
                     return acc;
                 })
                 .ToArray();
}
```

---

## Tips
- Prefer GroupBy + Select for counting/frequencies.
- For joins/outside joins, use query syntax or `GroupJoin` + `SelectMany`.
- Materialize once per pipeline with `ToArray/ToList` as needed.
