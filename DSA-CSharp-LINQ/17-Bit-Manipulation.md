# 🧮 Bit Manipulation with LINQ helpers

Use LINQ to visualize and aggregate bit operations while core ops remain bitwise.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Counting Bits, Single Number, Hamming Weight
```csharp
public static class Bits
{
    public static int[] CountBits(int n)
        => Enumerable.Range(0, n + 1)
                      .Select(x => Convert.ToString(x, 2).Count(c => c == '1'))
                      .ToArray();

    public static int SingleNumber(int[] nums) => nums.Aggregate(0, (acc, x) => acc ^ x);

    public static int HammingWeight(uint x)
    {
        int cnt = 0; while (x != 0) { x &= (x - 1); cnt++; } return cnt;
    }
}
```

---

## Visualize bits with LINQ
```csharp
public static class BitViews
{
    public static string ToBinary(int x, int width = 8)
        => string.Concat(Convert.ToString(x, 2).PadLeft(width, '0').Chunk(width).Select(chunk => new string(chunk)));
}
```
