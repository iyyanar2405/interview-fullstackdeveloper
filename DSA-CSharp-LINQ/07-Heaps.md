# ⛏️ Heaps (PriorityQueue) with LINQ

Use .NET `PriorityQueue<TElement, TPriority>` for heap behaviors and combine with LINQ for views and analytics.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Top-K, Merge K Sorted Lists, and Running Median
```csharp
public static class HeapProblems
{
    public static int[] TopK(int[] nums, int k)
    {
        var pq = new PriorityQueue<int, int>();
        foreach (var x in nums)
        {
            pq.Enqueue(x, x);
            if (pq.Count > k) pq.Dequeue(); // min-heap keeps smallest; overflow pops smallest
        }
        return Enumerable.Range(0, pq.Count).Select(_ => pq.Dequeue()).OrderByDescending(x => x).ToArray();
    }

    public static int[] MergeKSorted(int[][] lists)
    {
        var pq = new PriorityQueue<(int val, int r, int c), int>();
        for (int r = 0; r < lists.Length; r++) if (lists[r].Length > 0) pq.Enqueue((lists[r][0], r, 0), lists[r][0]);
        var res = new List<int>();
        while (pq.Count > 0)
        {
            var (v, r, c) = pq.Dequeue();
            res.Add(v);
            if (c + 1 < lists[r].Length) pq.Enqueue((lists[r][c + 1], r, c + 1), lists[r][c + 1]);
        }
        return res.ToArray();
    }
}
```

---

## LINQ views over heap snapshots
```csharp
public static class HeapViews
{
    public static int[] AsSortedDescending<T>(PriorityQueue<T, int> pq)
        where T : notnull
        => pq.UnorderedItems.Select(x => x.Element).OfType<int>().OrderByDescending(x => x).ToArray();
}
```
