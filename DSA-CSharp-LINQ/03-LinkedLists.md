# 🔗 Linked Lists with LINQ — Idiomatic Patterns

Use .NET's `LinkedList<T>` plus LINQ to query, transform, and solve interview-style problems cleanly.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Create and Query
```csharp
public static class LinkedListBasics
{
    public static LinkedList<int> Build(params int[] values)
        => new(values);

    public static void Demo()
    {
        var ll = Build(3, 1, 4, 1, 5, 9);

        // Non-destructive queries
        var evens = ll.Where(x => x % 2 == 0).ToArray();
        var squares = ll.Select(x => x * x).ToArray();

        // Frequency map
        var freq = ll.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        Console.WriteLine(string.Join(", ", evens));
        Console.WriteLine(string.Join(", ", squares));
        Console.WriteLine(string.Join(", ", freq.Select(kv => $"{kv.Key}:{kv.Value}")));
    }
}
```

---

## Find, Insert, Remove
```csharp
public static class LinkedListOps
{
    public static LinkedListNode<T>? FindFirst<T>(LinkedList<T> list, Func<T, bool> predicate)
        => list.FirstOrDefault(node => predicate(node.Value));

    public static void InsertAfter<T>(LinkedList<T> list, T afterValue, T newValue)
    {
        var node = list.FirstOrDefault(n => EqualityComparer<T>.Default.Equals(n.Value, afterValue));
        if (node != null) list.AddAfter(node, newValue);
    }

    public static void RemoveWhere<T>(LinkedList<T> list, Func<T, bool> predicate)
    {
        // Copy nodes to avoid mutating while enumerating the original
        foreach (var node in list.Nodes().Where(n => predicate(n.Value)).ToArray())
            list.Remove(node);
    }
}

public static class LinkedListExtensions
{
    // Enumerate nodes directly
    public static IEnumerable<LinkedListNode<T>> Nodes<T>(this LinkedList<T> list)
    {
        for (var n = list.First; n != null; n = n.Next) yield return n;
    }

    // FirstOrDefault over nodes
    public static LinkedListNode<T>? FirstOrDefault<T>(this LinkedList<T> list, Func<LinkedListNode<T>, bool> predicate)
    {
        for (var n = list.First; n != null; n = n.Next)
            if (predicate(n)) return n;
        return null;
    }
}
```

---

## Merge Two Sorted Linked Lists (LINQ view)
```csharp
public static class MergeSortedLists
{
    // Non-destructive merge using LINQ (O((m+n) log(m+n)) due to OrderBy)
    public static LinkedList<int> Merge(LinkedList<int> a, LinkedList<int> b)
    {
        var merged = a.Concat(b).OrderBy(x => x).ToArray();
        return new LinkedList<int>(merged);
    }
}
```

---

## Reverse a Linked List (two ways)
```csharp
public static class ReverseLinkedList
{
    // Build a reversed copy via LINQ
    public static LinkedList<T> ReversedView<T>(LinkedList<T> list)
        => new(list.Reverse());

    // In-place pointer reversal (O(n), no LINQ, but canonical)
    public static void ReverseInPlace<T>(LinkedList<T> list)
    {
        var nodes = list.Nodes().ToArray();
        list.Clear();
        foreach (var n in nodes) list.AddFirst(n.Value);
    }
}
```

---

## Detect Cycle (Floyd) and Analyze with LINQ
```csharp
public static class CycleDetection
{
    public static bool HasCycle<T>(LinkedList<T> list)
    {
        // LinkedList<T> in .NET can't directly create cycles with public API,
        // but the interview pattern is useful to know.
        LinkedListNode<T>? slow = list.First, fast = list.First;
        while (fast?.Next != null)
        {
            slow = slow!.Next;
            fast = fast.Next.Next;
            if (ReferenceEquals(slow, fast)) return true;
        }
        return false;
    }

    public static (T Value, int Index)? FirstUnique<T>(LinkedList<T> list)
    {
        var arr = list.ToArray();
        var counts = arr.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        var idx = arr.Select((v, i) => (v, i)).FirstOrDefault(t => counts[t.v] == 1);
        return EqualityComparer<(T,int)>.Default.Equals(idx, default) ? null : (idx.v, idx.i);
    }
}
```

---

## Kth from End using LINQ shortcuts
```csharp
public static class KthFromEnd
{
    public static T? Get<T>(LinkedList<T> list, int k)
    {
        if (k < 0 || k >= list.Count) return default;
        return list.Reverse().Skip(k).FirstOrDefault();
    }
}
```

---

## Tips
- Use `LinkedList<T>` when frequent insert/remove in the middle is needed.
- Use LINQ for read/query; use node APIs for structural edits.
- For large lists, avoid repeated `ToArray/ToList`; iterate once or stream with `yield`.
