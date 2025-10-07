# 🌲 Trees & BSTs with LINQ-Friendly Traversals

Define clean traversals that return `IEnumerable<T>` so you can compose with LINQ for analytics and queries.

## Essentials
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

---

## Node and Builders
```csharp
public class TreeNode<T>
{
    public T Value { get; set; }
    public TreeNode<T>? Left { get; set; }
    public TreeNode<T>? Right { get; set; }
    public TreeNode(T value) => Value = value;
}

public static class Bst
{
    public static TreeNode<int>? Insert(TreeNode<int>? root, int v)
    {
        if (root == null) return new TreeNode<int>(v);
        if (v < root.Value) root.Left = Insert(root.Left, v);
        else root.Right = Insert(root.Right, v);
        return root;
    }

    public static TreeNode<int>? Build(params int[] values)
        => values.Aggregate<TreeNode<int>?>(null, (root, v) => Insert(root, v));
}
```

---

## Traversals that compose with LINQ
```csharp
public static class Traversals
{
    public static IEnumerable<T> InOrder<T>(TreeNode<T>? n)
    {
        if (n == null) yield break;
        foreach (var x in InOrder(n.Left)) yield return x;
        yield return n.Value;
        foreach (var x in InOrder(n.Right)) yield return x;
    }

    public static IEnumerable<T> PreOrder<T>(TreeNode<T>? n)
    {
        if (n == null) yield break;
        yield return n.Value;
        foreach (var x in PreOrder(n.Left)) yield return x;
        foreach (var x in PreOrder(n.Right)) yield return x;
    }

    public static IEnumerable<T> PostOrder<T>(TreeNode<T>? n)
    {
        if (n == null) yield break;
        foreach (var x in PostOrder(n.Left)) yield return x;
        foreach (var x in PostOrder(n.Right)) yield return x;
        yield return n.Value;
    }

    public static IEnumerable<IEnumerable<T>> LevelOrderLayers<T>(TreeNode<T>? root)
    {
        if (root == null) yield break;
        var q = new Queue<TreeNode<T>>();
        q.Enqueue(root);
        while (q.Any())
        {
            var level = new List<TreeNode<T>>();
            var size = q.Count;
            for (int i = 0; i < size; i++) level.Add(q.Dequeue());
            foreach (var node in level)
            {
                if (node.Left != null) q.Enqueue(node.Left);
                if (node.Right != null) q.Enqueue(node.Right);
            }
            yield return level.Select(n => n.Value);
        }
    }
}
```

---

## LINQ on traversals: analytics and queries
```csharp
public static class TreeQueries
{
    public static int Sum(TreeNode<int>? root) => Traversals.InOrder(root).Sum();
    public static int Count(TreeNode<int>? root) => Traversals.InOrder(root).Count();
    public static int Height<T>(TreeNode<T>? root)
        => root == null ? 0 : 1 + Math.Max(Height(root.Left), Height(root.Right));

    public static bool Contains(TreeNode<int>? root, int target)
        => Traversals.InOrder(root).Any(x => x == target);

    // kth smallest in BST via in-order + LINQ
    public static int? KthSmallest(TreeNode<int>? root, int k)
        => Traversals.InOrder(root).Skip(k).Cast<int?>().FirstOrDefault();

    // level-wise max values
    public static int[] LevelMax(TreeNode<int>? root)
        => Traversals.LevelOrderLayers(root)
            .Select(layer => layer.Max())
            .ToArray();
}
```

---

## Lowest Common Ancestor (BST)
```csharp
public static class LcaBst
{
    public static TreeNode<int>? Lca(TreeNode<int>? root, int p, int q)
    {
        while (root != null)
        {
            if (p < root.Value && q < root.Value) root = root.Left;
            else if (p > root.Value && q > root.Value) root = root.Right;
            else return root;
        }
        return null;
    }
}
```

---

## Example
```csharp
public static class TreeExample
{
    public static void Demo()
    {
        var root = Bst.Build(5, 3, 7, 2, 4, 6, 8);
        var inorder = Traversals.InOrder(root).ToArray();          // 2 3 4 5 6 7 8
        var height = TreeQueries.Height(root);                      // 3
        var has6 = TreeQueries.Contains(root, 6);                   // true
        var kth = TreeQueries.KthSmallest(root, 3);                 // 5 (0-indexed)
        var levelMax = TreeQueries.LevelMax(root);                  // [5,7,8]

        Console.WriteLine(string.Join(", ", inorder));
        Console.WriteLine(height);
        Console.WriteLine(has6);
        Console.WriteLine(kth);
        Console.WriteLine(string.Join(", ", levelMax));
    }
}
```

---

## Tips
- Return `IEnumerable<T>` from traversals to compose naturally with LINQ.
- For heavy workloads, avoid repeated traversals; materialize once if you need multiple passes.
- For non-BST trees, compute queries over traversals exactly the same way.
