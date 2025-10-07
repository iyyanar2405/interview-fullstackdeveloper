# 🎤 Interview Questions — LINQ-centric Solutions

Quick prompts with patterns you can adapt in interviews.

---

## Remove duplicates from array while preserving order
```csharp
public static class PreserveOrderDistinct
{
    public static int[] Solve(int[] a)
    {
        var seen = new HashSet<int>();
        return a.Where(x => seen.Add(x)).ToArray();
    }
}
```

## Find top N frequent users per country
```csharp
public record User(string Country, string Name);

public static class TopUsers
{
    public static (string Country, string Name)[] Solve(User[] users, int n)
        => users.GroupBy(u => u.Country)
                .SelectMany(g => g.GroupBy(u => u.Name)
                                   .OrderByDescending(gg => gg.Count())
                                   .Take(n)
                                   .Select(gg => (g.Key, gg.Key)))
                .ToArray();
}
```

## Detect anagram groups and pick representative
```csharp
public static class AnagramRepresentative
{
    public static string[] Solve(string[] words)
        => words.GroupBy(w => new string(w.OrderBy(c => c).ToArray()))
                .Select(g => g.OrderBy(w => w).First())
                .ToArray();
}
```

## Join orders with latest status
```csharp
public record Order(int Id);
public record Status(int OrderId, DateTime When, string State);

public static class LatestStatus
{
    public static (int OrderId, string State)[] Solve(Order[] orders, Status[] status)
        => from o in orders
           join s in status on o.Id equals s.OrderId into gj
           select (o.Id, gj.OrderByDescending(x => x.When).FirstOrDefault()?.State ?? "Unknown");
}
```
