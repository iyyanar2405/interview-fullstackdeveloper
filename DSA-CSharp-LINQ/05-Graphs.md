# 🕸️ Graphs in C# with LINQ

Represent graphs and use LINQ to query neighbors, degrees, and run traversals.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Representations
```csharp
public static class GraphRep
{
    // Adjacency list (undirected by default)
    public static Dictionary<int, List<int>> BuildUndirected(int n, int[][] edges)
    {
        var g = Enumerable.Range(0, n).ToDictionary(i => i, _ => new List<int>());
        foreach (var e in edges)
        {
            g[e[0]].Add(e[1]);
            g[e[1]].Add(e[0]);
        }
        return g;
    }

    // Degree sequence with LINQ
    public static (int Node, int Degree)[] Degrees(Dictionary<int, List<int>> g)
        => g.Select(kv => (kv.Key, kv.Value.Count))
            .OrderByDescending(t => t.Item2)
            .ToArray();
}
```

---

## BFS / DFS
```csharp
public static class Traversal
{
    public static int[] Bfs(Dictionary<int, List<int>> g, int src)
    {
        var q = new Queue<int>();
        var vis = new HashSet<int>();
        var order = new List<int>();
        q.Enqueue(src); vis.Add(src);
        while (q.Any())
        {
            var u = q.Dequeue();
            order.Add(u);
            foreach (var v in g[u]) if (vis.Add(v)) q.Enqueue(v);
        }
        return order.ToArray();
    }

    public static int[] Dfs(Dictionary<int, List<int>> g, int src)
    {
        var st = new Stack<int>();
        var vis = new HashSet<int>();
        var order = new List<int>();
        st.Push(src);
        while (st.Any())
        {
            var u = st.Pop();
            if (!vis.Add(u)) continue;
            order.Add(u);
            foreach (var v in g[u].AsEnumerable().Reverse()) st.Push(v);
        }
        return order.ToArray();
    }
}
```

---

## Connected Components with LINQ-friendly outputs
```csharp
public static class Components
{
    public static List<int[]> Connected(Dictionary<int, List<int>> g)
    {
        var vis = new HashSet<int>();
        var comps = new List<int[]>();
        foreach (var start in g.Keys)
        {
            if (vis.Contains(start)) continue;
            var comp = new List<int>();
            var q = new Queue<int>();
            q.Enqueue(start); vis.Add(start);
            while (q.Any())
            {
                var u = q.Dequeue();
                comp.Add(u);
                foreach (var v in g[u]) if (vis.Add(v)) q.Enqueue(v);
            }
            comps.Add(comp.OrderBy(x => x).ToArray());
        }
        return comps.OrderBy(c => c.First()).ToList();
    }
}
```
