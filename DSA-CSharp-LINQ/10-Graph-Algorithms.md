# 🧭 Graph Algorithms with LINQ-friendly Patterns

Implement BFS/DFS, Dijkstra, and topological sort; expose results as `IEnumerable<T>` for LINQ composition.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Dijkstra (weighted, non-negative)
```csharp
public static class Dijkstra
{
    public static (int[] Dist, int?[] Prev) ShortestPaths(Dictionary<int, List<(int v, int w)>> g, int src)
    {
        int n = g.Count;
        var dist = Enumerable.Repeat(int.MaxValue, n).ToArray();
        var prev = new int?[n];
        var pq = new PriorityQueue<int, int>();
        dist[src] = 0; pq.Enqueue(src, 0);
        while (pq.Count > 0)
        {
            var u = pq.Dequeue();
            foreach (var (v, w) in g[u])
            {
                if (dist[u] + w < dist[v])
                {
                    dist[v] = dist[u] + w;
                    prev[v] = u;
                    pq.Enqueue(v, dist[v]);
                }
            }
        }
        return (dist, prev);
    }

    public static int[] ReconstructPath(int?[] prev, int target)
    {
        var path = new List<int>();
        for (int? at = target; at != null; at = prev[at.Value]) path.Add(at.Value);
        path.Reverse();
        return path.ToArray();
    }
}
```

---

## Topological Sort (Kahn)
```csharp
public static class TopoSort
{
    public static int[] Sort(Dictionary<int, List<int>> g)
    {
        var indeg = g.ToDictionary(kv => kv.Key, kv => 0);
        foreach (var (_, list) in g) foreach (var v in list) indeg[v] = indeg.GetValueOrDefault(v) + 1;
        var q = new Queue<int>(indeg.Where(kv => kv.Value == 0).Select(kv => kv.Key));
        var order = new List<int>();
        while (q.Any())
        {
            var u = q.Dequeue(); order.Add(u);
            foreach (var v in g.GetValueOrDefault(u, new List<int>()))
                if (--indeg[v] == 0) q.Enqueue(v);
        }
        return order.ToArray();
    }
}
```

---

## Cycle Detection (Directed)
```csharp
public static class DirectedCycles
{
    public static bool HasCycle(Dictionary<int, List<int>> g)
    {
        var state = new Dictionary<int, int>(); // 0=unseen,1=visiting,2=done
        bool Dfs(int u)
        {
            if (state.GetValueOrDefault(u) == 1) return true;
            if (state.GetValueOrDefault(u) == 2) return false;
            state[u] = 1;
            foreach (var v in g.GetValueOrDefault(u, new())) if (Dfs(v)) return true;
            state[u] = 2; return false;
        }
        return g.Keys.Any(Dfs);
    }
}
```
