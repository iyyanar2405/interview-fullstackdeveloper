# 💡 Greedy Algorithms with LINQ

Use sorting, grouping, and scans to implement common greedy patterns.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Activity Selection (max non-overlapping)
```csharp
public record Interval(int Start, int End);

public static class ActivitySelection
{
    public static Interval[] Select(Interval[] intervals)
        => intervals.OrderBy(i => i.End)
                    .Aggregate(new List<Interval>(), (acc, cur) =>
                    {
                        if (!acc.Any() || acc[^1].End <= cur.Start) acc.Add(cur);
                        return acc;
                    })
                    .ToArray();
}
```

---

## Huffman Coding (skeleton using PriorityQueue + LINQ view)
```csharp
public static class Huffman
{
    public record Node(char Ch, int Freq, Node? Left = null, Node? Right = null);

    public static Node Build(string text)
    {
        var freq = text.GroupBy(c => c).Select(g => new Node(g.Key, g.Count())).ToList();
        var pq = new PriorityQueue<Node, int>(freq.Select(n => (n, n.Freq)));
        while (pq.Count > 1)
        {
            var a = pq.Dequeue(); var b = pq.Dequeue();
            pq.Enqueue(new Node('\0', a.Freq + b.Freq, a, b), a.Freq + b.Freq);
        }
        return pq.Dequeue();
    }
}
```
