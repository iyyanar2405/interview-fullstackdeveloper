# 🧰 Advanced Data Structures with LINQ-friendly APIs

Provide enumerables for LINQ composition while keeping core operations optimal.

## Trie
```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class Trie
{
    private class Node { public bool End; public Dictionary<char, Node> Next = new(); }
    private readonly Node root = new();

    public void Insert(string word)
    {
        var n = root;
        foreach (var c in word)
        {
            if (!n.Next.TryGetValue(c, out var nxt)) n.Next[c] = nxt = new Node();
            n = nxt;
        }
        n.End = true;
    }

    public bool Search(string word)
    {
        var n = root; foreach (var c in word) if (!n.Next.TryGetValue(c, out n)) return false; return n.End;
    }

    public IEnumerable<string> StartsWith(string prefix)
    {
        var n = root; foreach (var c in prefix) if (!n.Next.TryGetValue(c, out n)) yield break;
        foreach (var w in Dfs(n, prefix)) yield return w;

        IEnumerable<string> Dfs(Node node, string path)
        {
            if (node.End) yield return path;
            foreach (var kv in node.Next.OrderBy(kv => kv.Key))
                foreach (var s in Dfs(kv.Value, path + kv.Key)) yield return s;
        }
    }
}
```

## Disjoint Set Union (Union-Find) with LINQ summaries
```csharp
public static class Dsu
{
    public static (int Components, int[] Sizes) Analyze(int n, int[][] edges)
    {
        var parent = Enumerable.Range(0, n).ToArray();
        var size = Enumerable.Repeat(1, n).ToArray();
        int Find(int x) => parent[x] == x ? x : parent[x] = Find(parent[x]);
        void Union(int a, int b)
        {
            a = Find(a); b = Find(b);
            if (a == b) return;
            if (size[a] < size[b]) (a, b) = (b, a);
            parent[b] = a; size[a] += size[b];
        }
        foreach (var e in edges) Union(e[0], e[1]);
        var roots = Enumerable.Range(0, n).Select(Find).ToArray();
        var sizes = roots.GroupBy(r => r).Select(g => g.Count()).OrderByDescending(x => x).ToArray();
        return (sizes.Length, sizes);
    }
}
```