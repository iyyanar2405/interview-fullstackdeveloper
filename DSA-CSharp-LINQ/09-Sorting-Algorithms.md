# 🔢 Sorting Algorithms in C# with LINQ

Learn canonical sorts and how to use LINQ for quick sorting, grouping, and custom orderings.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Built-in LINQ Sorting
```csharp
public static class LinqSorts
{
    public static int[] Asc(int[] arr) => arr.OrderBy(x => x).ToArray();
    public static int[] Desc(int[] arr) => arr.OrderByDescending(x => x).ToArray();

    // Sort by multiple keys
    public static IEnumerable<(string Name, int Age, double Score)> ByMultiple(
        IEnumerable<(string Name, int Age, double Score)> people)
        => people.OrderBy(p => p.Age).ThenByDescending(p => p.Score);

    // Custom comparer (case-insensitive, then length)
    public static string[] CustomStrings(IEnumerable<string> words)
        => words.OrderBy(w => w, StringComparer.OrdinalIgnoreCase)
                .ThenBy(w => w.Length)
                .ToArray();
}
```

---

## Implementations (educational)

### 1) Insertion Sort
```csharp
public static class InsertionSort
{
    public static int[] Sort(int[] arr)
    {
        var a = (int[])arr.Clone();
        for (int i = 1; i < a.Length; i++)
        {
            int key = a[i]; int j = i - 1;
            while (j >= 0 && a[j] > key)
            {
                a[j + 1] = a[j];
                j--;
            }
            a[j + 1] = key;
        }
        return a;
    }
}
```

### 2) Merge Sort (+LINQ glue)
```csharp
public static class MergeSort
{
    public static int[] Sort(int[] arr)
    {
        if (arr.Length <= 1) return arr.ToArray();
        int mid = arr.Length / 2;
        var left = Sort(arr.Take(mid).ToArray());
        var right = Sort(arr.Skip(mid).ToArray());
        return Merge(left, right);
    }

    private static int[] Merge(int[] a, int[] b)
    {
        var res = new int[a.Length + b.Length];
        int i = 0, j = 0, k = 0;
        while (i < a.Length && j < b.Length)
            res[k++] = a[i] <= b[j] ? a[i++] : b[j++];
        while (i < a.Length) res[k++] = a[i++];
        while (j < b.Length) res[k++] = b[j++];
        return res;
    }
}
```

### 3) Quick Sort (functional style)
```csharp
public static class QuickSort
{
    public static int[] Sort(int[] arr)
    {
        if (arr.Length <= 1) return arr.ToArray();
        var pivot = arr[arr.Length / 2];
        var less = arr.Where(x => x < pivot);
        var equal = arr.Where(x => x == pivot);
        var greater = arr.Where(x => x > pivot);
        return less.Concat(equal).Concat(greater).ToArray();
    }
}
```

### 4) Counting Sort (non-negative)
```csharp
public static class CountingSort
{
    public static int[] Sort(int[] arr)
    {
        if (arr.Length == 0) return Array.Empty<int>();
        int max = arr.Max();
        var counts = new int[max + 1];
        foreach (var x in arr) counts[x]++;
        return counts
            .SelectMany((cnt, val) => Enumerable.Repeat(val, cnt))
            .ToArray();
    }
}
```

---

## Sort Objects with Projections
```csharp
public record Product(string Name, string Category, decimal Price, double Rating);

public static class ProductSorting
{
    public static Product[] ByCategoryThenPrice(IEnumerable<Product> products)
        => products.OrderBy(p => p.Category)
                   .ThenBy(p => p.Price)
                   .ToArray();

    public static Product[] TopNByRating(IEnumerable<Product> products, int n)
        => products.OrderByDescending(p => p.Rating)
                   .ThenBy(p => p.Price)
                   .Take(n)
                   .ToArray();
}
```

---

## Stability, Complexity, and Tips
- OrderBy is stable; QuickSort above (functional) is stable for equal keys due to grouping.
- Complexities: Insertion O(n^2), Merge O(n log n), Quick (avg) O(n log n), Counting O(n+k).
- Materialize once with ToArray/ToList at the edges, not between every step.
- Prefer built-in `OrderBy/ThenBy` unless you need a specific learning implementation.
