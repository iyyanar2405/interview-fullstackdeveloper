# ⚡ LINQ Performance: Patterns and Benchmarks

Practical tips to write fast, memory-conscious LINQ with small benchmarks.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
```

---

## Deferred vs Immediate Execution
```csharp
public static class DeferredVsImmediate
{
    public static void Show()
    {
        var data = Enumerable.Range(1, 10).ToList();
        var q = data.Where(x => { Console.WriteLine($"check {x}"); return x % 2 == 0; }); // deferred
        Console.WriteLine("not executed yet");
        var arr = q.ToArray(); // executes now
    }
}
```

---

## Allocate once, compose lazily
```csharp
public static class AllocationTips
{
    public static int[] Good(int[] a)
        => a.Where(x => x > 0).Select(x => x * x).Where(x => x < 1000).ToArray();

    public static int[] Bad(int[] a)
    {
        var s1 = a.Where(x => x > 0).ToArray();
        var s2 = s1.Select(x => x * x).ToArray();
        return s2.Where(x => x < 1000).ToArray();
    }
}
```

---

## Micro-benchmark template
```csharp
public static class MicroBench
{
    public static (long linqMs, long loopMs) Compare()
    {
        var data = Enumerable.Range(1, 1_000_00).ToArray();
        var sw = new Stopwatch();

        sw.Restart();
        var r1 = data.Where(x => x % 2 == 0).Select(x => x * x).Where(x => x > 1000).ToArray();
        sw.Stop();
        var linqMs = sw.ElapsedMilliseconds;

        sw.Restart();
        var r2 = new List<int>();
        foreach (var x in data) if ((x & 1) == 0) { var y = x * x; if (y > 1000) r2.Add(y); }
        sw.Stop();
        var loopMs = sw.ElapsedMilliseconds;

        return (linqMs, loopMs);
    }
}
```

---

## Tips
- Push filters earlier in the chain.
- Avoid re-enumeration: cache with `ToList`/`ToArray` only when needed.
- Prefer `Any`/`All` over `Count()>0` or `Where(...).Any()`.
- Use `HashSet`/`Dictionary` to reduce O(n^2) scans to O(n).
