# 🧮 Dynamic Programming with LINQ

Use LINQ to express transitions and summaries; keep core DP arrays for performance.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## 0/1 Knapsack (LINQ transitions)
```csharp
public static class Knapsack01
{
    public static int MaxValue((int W, int V)[] items, int capacity)
    {
        var dp = new int[capacity + 1];
        foreach (var it in items)
        {
            // iterate reversed; LINQ for readability of candidate values
            foreach (var w in Enumerable.Range(0, capacity - it.W + 1).Reverse())
            {
                dp[w + it.W] = Math.Max(dp[w + it.W], dp[w] + it.V);
            }
        }
        return dp.Max();
    }
}
```

---

## Coin Change (min coins)
```csharp
public static class CoinChange
{
    public static int MinCoins(int[] coins, int amount)
    {
        var INF = amount + 1;
        var dp = Enumerable.Repeat(INF, amount + 1).ToArray();
        dp[0] = 0;
        foreach (var c in coins)
        {
            foreach (var a in Enumerable.Range(c, amount - c + 1))
                dp[a] = Math.Min(dp[a], dp[a - c] + 1);
        }
        return dp[amount] > amount ? -1 : dp[amount];
    }
}
```

---

## Longest Increasing Subsequence (patience) with LINQ helpers
```csharp
public static class LIS
{
    public static int Length(int[] nums)
    {
        var tails = new List<int>();
        foreach (var x in nums)
        {
            int i = tails.BinarySearch(x);
            if (i < 0) i = ~i;
            if (i == tails.Count) tails.Add(x); else tails[i] = x;
        }
        return tails.Count;
    }
}
```
