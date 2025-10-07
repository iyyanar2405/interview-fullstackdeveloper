# 🧱 Stacks & Queues with LINQ — Practical Guide

Master common stack and queue patterns in C# with readable LINQ-powered examples.

## What you'll learn
- Idiomatic use of `Stack<T>` and `Queue<T>`
- How LINQ makes inspection, slicing, grouping, and analytics easier
- Real interview problems (valid parentheses, sliding window, task scheduling)

---

## Essentials

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Stack<T> — LIFO Operations

### Build, peek, and analyze with LINQ
```csharp
public static class StackBasics
{
    public static void Demo()
    {
        var st = new Stack<int>(new[] { 1, 2, 3, 4, 5 });

        // Peek many (without popping): LINQ over stack snapshot
        var top3 = st.Take(3).ToArray();         // [5,4,3]

        // Find items meeting a condition
        bool hasLarge = st.Any(x => x > 10);     // false

        // Frequency of values (grouping)
        var freq = new Stack<int>(new[] { 3, 1, 3, 2, 2, 3 })
            .GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());

        // Copy/reverse quickly
        var reversed = new Stack<int>(st.Reverse());       // bottom-to-top copy

        Console.WriteLine(string.Join(", ", top3));
        Console.WriteLine(string.Join(", ", reversed));
        Console.WriteLine(string.Join(", ", freq.Select(kv => $"{kv.Key}:{kv.Value}")));
    }
}
```

### Valid Parentheses (Interview classic)
Use a stack for matching. LINQ helps with quick checks and mapping.
```csharp
public static class ParenthesesValidator
{
    private static readonly Dictionary<char, char> Match = new()
    {
        [')'] = '(',
        [']'] = '[',
        ['}'] = '{'
    };

    public static bool IsValid(string s)
    {
        if (string.IsNullOrEmpty(s)) return true;
        if (s.Length % 2 == 1) return false;                 // quick prune
        if (!s.All(c => "(){}[]".Contains(c))) return false; // only bracket chars

        var st = new Stack<char>();
        foreach (var c in s)
        {
            if (Match.Values.Contains(c)) st.Push(c);        // opening
            else if (Match.TryGetValue(c, out var need))
            {
                if (st.Count == 0 || st.Pop() != need) return false;
            }
        }
        return st.Count == 0;
    }
}
```

### Monotonic Stack with LINQ inspection (Next Greater Element)
```csharp
public static class NextGreaterElement
{
    // Returns next greater element to the right for each index; -1 if none
    public static int[] Compute(int[] nums)
    {
        var res = Enumerable.Repeat(-1, nums.Length).ToArray();
        var st = new Stack<int>(); // store indices
        for (int i = 0; i < nums.Length; i++)
        {
            while (st.Any() && nums[i] > nums[st.Peek()])
            {
                var idx = st.Pop();
                res[idx] = nums[i];
            }
            st.Push(i);
        }
        return res;
    }
}
```

---

## Queue<T> — FIFO Operations

### Basics with LINQ helpers
```csharp
public static class QueueBasics
{
    public static void Demo()
    {
        var q = new Queue<string>(new[] { "A", "B", "C", "D" });

        // Take snapshot safely and query with LINQ
        var firstTwo = q.Take(2).ToArray();          // [A,B]
        bool hasC = q.Contains("C");                 // quick membership

        // Filter view (non-destructive)
        var exceptB = q.Where(x => x != "B").ToArray();

        Console.WriteLine(string.Join(", ", firstTwo));
        Console.WriteLine(string.Join(", ", exceptB));
        Console.WriteLine(hasC);
    }
}
```

### Sliding Window Maximum (Queue + LINQ view)
```csharp
public static class SlidingWindow
{
    // O(n) deque approach for performance; use LINQ only for inspecting results
    public static int[] MaxInWindow(int[] nums, int k)
    {
        var dq = new LinkedList<int>(); // store indices, values decreasing
        var ans = new List<int>();

        for (int i = 0; i < nums.Length; i++)
        {
            // drop out-of-window indices
            while (dq.Any() && dq.First!.Value <= i - k) dq.RemoveFirst();
            // maintain decreasing deque
            while (dq.Any() && nums[dq.Last!.Value] < nums[i]) dq.RemoveLast();
            dq.AddLast(i);
            if (i >= k - 1) ans.Add(nums[dq.First!.Value]);
        }
        return ans.ToArray();
    }
}
```

### Task Scheduling (Round Robin) using Queue
```csharp
public static class RoundRobinScheduler
{
    public record TaskItem(string Name, int BurstMs);

    public static IEnumerable<(string Name, int Slice)> Schedule(
        IEnumerable<TaskItem> tasks, int quantum)
    {
        var q = new Queue<TaskItem>(tasks);
        while (q.Any())
        {
            var t = q.Dequeue();
            var slice = Math.Min(quantum, t.BurstMs);
            yield return (t.Name, slice);
            var remaining = t.BurstMs - slice;
            if (remaining > 0) q.Enqueue(t with { BurstMs = remaining });
        }
    }
}
```

---

## Deque patterns with LINQ snapshots
```csharp
public static class DequePatterns
{
    // Check if sequence is palindrome using deque-like operations
    public static bool IsPalindrome<T>(IEnumerable<T> sequence)
    {
        var arr = sequence as T[] ?? sequence.ToArray();
        return arr.SequenceEqual(arr.Reverse());
    }
}
```

---

## Tips and Best Practices
- Prefer Stack/Queue for core mutation; use LINQ for views, filtering, grouping, analytics.
- LINQ queries on Stack/Queue enumerate from top/front respectively—OK for inspection, not for mutation.
- Avoid repeated materialization (ToArray/ToList) inside tight loops; compose queries then materialize once.
- For hot paths, measure: sometimes loops beat LINQ in perf-critical sections.
