# 🗃️ Hash Tables (Dictionaries/Sets) with LINQ

Use `Dictionary<TKey,TValue>` and `HashSet<T>` with LINQ for fast lookups and clean analytics.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Frequency Maps, Grouping, and Joins
```csharp
public static class HashBasics
{
    public static Dictionary<T, int> Frequency<T>(IEnumerable<T> items)
        => items.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

    public static Dictionary<TKey, List<T>> ToLookupDict<T, TKey>(IEnumerable<T> items, Func<T, TKey> key)
        => items.GroupBy(key).ToDictionary(g => g.Key, g => g.ToList());

    public static (TKey Key, int Count)[] TopK<T, TKey>(IEnumerable<T> items, Func<T, TKey> key, int k)
        => items.GroupBy(key).OrderByDescending(g => g.Count()).Take(k).Select(g => (g.Key, g.Count())).ToArray();
}
```

---

## Set Operations with LINQ
```csharp
public static class SetOps
{
    public static T[] Unique<T>(IEnumerable<T> items) => new HashSet<T>(items).ToArray();
    public static T[] Intersection<T>(IEnumerable<T> a, IEnumerable<T> b) => new HashSet<T>(a).Intersect(b).ToArray();
    public static T[] Union<T>(IEnumerable<T> a, IEnumerable<T> b) => new HashSet<T>(a).Union(b).ToArray();
    public static T[] Except<T>(IEnumerable<T> a, IEnumerable<T> b) => new HashSet<T>(a).Except(b).ToArray();
}
```

---

## Two Sum, First Unique, and Anagrams
```csharp
public static class ClassicProblems
{
    public static int[] TwoSum(int[] nums, int target)
    {
        var map = new Dictionary<int, int>();
        for (int i = 0; i < nums.Length; i++)
        {
            int need = target - nums[i];
            if (map.TryGetValue(need, out var j)) return new[] { j, i };
            map[nums[i]] = i;
        }
        return Array.Empty<int>();
    }

    public static int FirstUniqueChar(string s)
    {
        var freq = s.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
        return s.Select((c, i) => (c, i)).FirstOrDefault(t => freq[t.c] == 1, (-1, -1)).i;
    }

    public static bool AreAnagrams(string a, string b)
        => a.Length == b.Length && a.GroupBy(c => c).OrderBy(g => g.Key).Select(g => (g.Key, g.Count()))
                                    .SequenceEqual(b.GroupBy(c => c).OrderBy(g => g.Key).Select(g => (g.Key, g.Count())));
}
```
