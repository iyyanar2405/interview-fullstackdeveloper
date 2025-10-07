# 🧩 Backtracking with LINQ-friendly sets

Use sets/lists for state and LINQ to generate candidates cleanly.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Permutations
```csharp
public static class Permutations
{
    public static IEnumerable<int[]> Generate(int[] nums)
    {
        var used = new bool[nums.Length];
        var cur = new List<int>();
        foreach (var p in Dfs()) yield return p.ToArray();

        IEnumerable<List<int>> Dfs()
        {
            if (cur.Count == nums.Length) { yield return new List<int>(cur); yield break; }
            foreach (var i in Enumerable.Range(0, nums.Length).Where(i => !used[i]))
            {
                used[i] = true; cur.Add(nums[i]);
                foreach (var x in Dfs()) yield return x;
                cur.RemoveAt(cur.Count - 1); used[i] = false;
            }
        }
    }
}
```

---

## N-Queens (LINQ for safe positions)
```csharp
public static class NQueens
{
    public static IEnumerable<int[]> Solve(int n)
    {
        var cols = new HashSet<int>();
        var d1 = new HashSet<int>();
        var d2 = new HashSet<int>();
        var board = new int[n]; Array.Fill(board, -1);

        foreach (var sol in Place(0)) yield return sol.ToArray();

        IEnumerable<int[]> Place(int r)
        {
            if (r == n) { yield return board; yield break; }
            foreach (var c in Enumerable.Range(0, n).Where(c => !cols.Contains(c) && !d1.Contains(r - c) && !d2.Contains(r + c)))
            {
                board[r] = c; cols.Add(c); d1.Add(r - c); d2.Add(r + c);
                foreach (var s in Place(r + 1)) yield return s;
                cols.Remove(c); d1.Remove(r - c); d2.Remove(r + c);
            }
        }
    }
}
```
