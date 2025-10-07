# 🔎 Searching Algorithms with LINQ

Use LINQ for expressive searches, and fall back to classic patterns when needed for performance.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Linear Search (Index and Element)
```csharp
public static class LinearSearch
{
    public static int IndexOf<T>(IEnumerable<T> items, T target)
    {
        // Returns -1 if not found
        var match = items.Select((v, i) => new { v, i })
                         .FirstOrDefault(x => EqualityComparer<T>.Default.Equals(x.v, target));
        return match == null ? -1 : match.i;
    }

    public static T? Find<T>(IEnumerable<T> items, Func<T, bool> predicate)
        => items.FirstOrDefault(predicate);
}
```

---

## Binary Search (sorted inputs)
```csharp
public static class BinarySearchAlgo
{
    public static int IndexOf(int[] sorted, int target)
    {
        int lo = 0, hi = sorted.Length - 1;
        while (lo <= hi)
        {
            int mid = lo + (hi - lo) / 2;
            if (sorted[mid] == target) return mid;
            if (sorted[mid] < target) lo = mid + 1; else hi = mid - 1;
        }
        return -1;
    }

    // Lower bound using LINQ for skipping/skimming after search window is known
    public static int LowerBound(int[] sorted, int target)
    {
        int lo = 0, hi = sorted.Length;
        while (lo < hi)
        {
            int mid = (lo + hi) / 2;
            if (sorted[mid] < target) lo = mid + 1; else hi = mid;
        }
        return lo; // first index with value >= target
    }
}
```

---

## 2D Search (matrix) using SelectMany
```csharp
public static class MatrixSearch
{
    public static (int Row, int Col)? Find(int[][] matrix, int target)
    {
        var rows = matrix.Length;
        var cols = rows == 0 ? 0 : matrix[0].Length;
        var hit = Enumerable.Range(0, rows)
            .SelectMany(r => Enumerable.Range(0, cols).Select(c => (r, c)))
            .FirstOrDefault(rc => matrix[rc.r][rc.c] == target);
        return hit == default ? null : (hit.r, hit.c);
    }
}
```

---

## Search with Dictionaries (hash)
```csharp
public static class DictionarySearch
{
    public static int[] TwoSum(int[] nums, int target)
    {
        var map = new Dictionary<int, int>(); // value -> index
        for (int i = 0; i < nums.Length; i++)
        {
            int need = target - nums[i];
            if (map.TryGetValue(need, out var j)) return new[] { j, i };
            map[nums[i]] = i;
        }
        return Array.Empty<int>();
    }
}
```

---

## Search in Objects (LINQ predicates)
```csharp
public record Person(int Id, string Name, int Age, string City);

public static class ObjectSearch
{
    public static Person? FindByName(IEnumerable<Person> people, string name)
        => people.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

    public static Person[] AdultsInCity(IEnumerable<Person> people, string city, int minAge = 18)
        => people.Where(p => p.City == city && p.Age >= minAge).ToArray();

    public static (string City, double AvgAge)[] AvgAgeByCity(IEnumerable<Person> people)
        => people.GroupBy(p => p.City)
                 .Select(g => (g.Key, g.Average(p => p.Age)))
                 .OrderByDescending(t => t.Item2)
                 .ToArray();
}
```

---

## Fuzzy Search: starts-with/contains/score
```csharp
public static class FuzzySearch
{
    public static string[] StartsWith(IEnumerable<string> items, string prefix)
        => items.Where(s => s.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToArray();

    public static string[] ContainsText(IEnumerable<string> items, string text)
        => items.Where(s => s.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();

    // Toy score: prefix match > contains > Levenshtein stub (not implemented here)
    public static (string Item, int Score)[] Ranked(IEnumerable<string> items, string query)
    {
        return items
            .Select(s => (
                Item: s,
                Score: (s.StartsWith(query, StringComparison.OrdinalIgnoreCase) ? 100 : 0)
                     + (s.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ? 10 : 0)
            ))
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ToArray();
    }
}
```

---

## Tips
- Prefer LINQ for clarity on small/medium collections; for very large inputs or tight loops, measure classic loops.
- For sorted data, use binary search; for key lookups, prefer dictionaries/sets over scanning.
