# 🔤 String Algorithms with LINQ

Use expressive LINQ chains for scanning, counting, grouping, and transforming strings.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Frequency, Anagrams, and Palindromes
```csharp
public static class StringBasics
{
    public static Dictionary<char, int> CharFrequency(string s)
        => s.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

    public static bool AreAnagrams(string a, string b)
        => a.OrderBy(c => c).SequenceEqual(b.OrderBy(c => c));

    public static bool IsPalindrome(string s)
    {
        var normalized = new string(s.Where(char.IsLetterOrDigit)
                                     .Select(char.ToLowerInvariant)
                                     .ToArray());
        return normalized.SequenceEqual(normalized.Reverse());
    }
}
```

---

## Substrings and Windows
```csharp
public static class SubstringWindows
{
    // All substrings (O(n^2))
    public static IEnumerable<string> AllSubstrings(string s)
        => Enumerable.Range(0, s.Length)
                     .SelectMany(i => Enumerable.Range(1, s.Length - i).Select(len => s.Substring(i, len)));

    // Longest substring without repeating characters
    public static int LengthOfLongestUnique(string s)
    {
        var seen = new Dictionary<char, int>();
        int start = 0, best = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (seen.TryGetValue(s[i], out var prev) && prev >= start)
                start = prev + 1;
            seen[s[i]] = i;
            best = Math.Max(best, i - start + 1);
        }
        return best;
    }
}
```

---

## Group Anagrams with LINQ
```csharp
public static class AnagramGroups
{
    public static string[][] Group(string[] words)
        => words.GroupBy(w => new string(w.OrderBy(c => c).ToArray()))
                .Select(g => g.ToArray())
                .ToArray();
}
```

---

## Pattern Matching (Naive) and LINQ Helpers
```csharp
public static class PatternMatching
{
    // Naive contains (for illustration) with LINQ to pick candidates
    public static int IndexOf(string text, string pattern)
    {
        if (string.IsNullOrEmpty(pattern)) return 0;
        return Enumerable.Range(0, Math.Max(0, text.Length - pattern.Length + 1))
                         .FirstOrDefault(i => string.CompareOrdinal(text, i, pattern, 0, pattern.Length) == 0, -1);
    }

    // Count occurrences (overlap allowed)
    public static int CountOccurrences(string text, string pattern)
        => Enumerable.Range(0, Math.Max(0, text.Length - pattern.Length + 1))
                      .Count(i => string.CompareOrdinal(text, i, pattern, 0, pattern.Length) == 0);
}
```

---

## Tokenization, Transformations, and Joins
```csharp
public static class TokenOps
{
    public static string[] Words(string text)
        => text.Split(new[] { ' ', '\t', '\n', '\r', ',', ';', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

    public static string Normalize(string text)
        => string.Join(" ", Words(text).Select(w => w.ToLowerInvariant()));

    public static string ReverseWords(string text)
        => string.Join(" ", Words(text).Reverse());
}
```

---

## Tips
- Prefer projections (`Select`) over building strings char-by-char.
- Materialize when needed at the boundary (`ToArray`, `ToList`), keep middle steps lazy.
- For heavy workloads, use Span/Memory-based APIs from `System.Memory`.
