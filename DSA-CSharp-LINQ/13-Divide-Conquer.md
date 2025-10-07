# 🪓 Divide and Conquer with LINQ helpers

Split problems and use LINQ to combine or analyze results.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Maximum Subarray (Divide & Conquer) + LINQ for merges
```csharp
public static class MaxSubarrayDC
{
    public static int MaxSum(int[] arr)
        => Solve(arr, 0, arr.Length - 1).MaxSum;

    private static (int MaxSum, int Prefix, int Suffix, int Total) Solve(int[] a, int l, int r)
    {
        if (l == r) return (a[l], Math.Max(0, a[l]), Math.Max(0, a[l]), a[l]);
        int m = (l + r) / 2;
        var L = Solve(a, l, m);
        var R = Solve(a, m + 1, r);
        var maxSum = new[] { L.MaxSum, R.MaxSum, L.Suffix + R.Prefix }.Max();
        var prefix = Math.Max(L.Prefix, L.Total + R.Prefix);
        var suffix = Math.Max(R.Suffix, R.Total + L.Suffix);
        var total = L.Total + R.Total;
        return (maxSum, prefix, suffix, total);
    }
}
```

---

## Closest Pair (1D) with recursive splits
```csharp
public static class ClosestPair1D
{
    public static int MinDiff(int[] a)
    {
        var arr = a.OrderBy(x => x).ToArray();
        return Enumerable.Range(0, arr.Length - 1).Select(i => arr[i + 1] - arr[i]).Min();
    }
}
```
