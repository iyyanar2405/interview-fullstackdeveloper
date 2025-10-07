# 03 — C# Fundamentals for .NET Apps

This chapter is a fast, practical tour of modern C# you’ll use in .NET Core apps.

## Project defaults you’ll see
- Top-level statements (no explicit `Main` needed)
- File-scoped namespaces
- Implicit `global using` for common namespaces
- Nullable reference types enabled

## Types: class, struct, record
```csharp
// Class: reference type (mutable by default)
public class Person
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

// Record class: ideal for immutable data/DTOs, value-based equality
public record PersonDto(string FirstName, string LastName);

// Struct: value type (avoid large/complex structs)
public readonly struct Point(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;
}
```

## Pattern matching
```csharp
static string Classify(object value) => value switch
{
    null => "null",
    int i when i > 0 => ">$0 int",
    int i => "<=0 int",
    string s when s.Length > 3 => "long string",
    string => "short string",
    _ => "unknown"
};

// Property patterns
static bool IsOrigin(Point p) => p is { X: 0, Y: 0 };
```

## LINQ essentials
```csharp
var nums = new[] { 1, 2, 3, 4, 5, 6 };
var evensSquaredDesc = nums
    .Where(n => n % 2 == 0)
    .Select(n => n * n)
    .OrderByDescending(n => n)
    .ToArray(); // [36, 16, 4]
```

## Async/await and cancellation
```csharp
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

static async Task<string> FetchAsync(string url, CancellationToken ct)
{
    using var http = new HttpClient();
    using var response = await http.GetAsync(url, ct);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync(ct);
}

// IAsyncEnumerable
static async IAsyncEnumerable<int> CountAsync(int max, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
{
    for (var i = 1; i <= max; i++)
    {
        ct.ThrowIfCancellationRequested();
        await Task.Delay(10, ct);
        yield return i;
    }
}
```

## Error handling tips
```csharp
try
{
    // risky work
}
catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
{
    // handle 404 specifically
}
catch (Exception ex)
{
    // log and rethrow or return a Result type
    throw;
}
```

## Nullability and guards
```csharp
string Normalize(string? input)
{
    if (string.IsNullOrWhiteSpace(input)) return string.Empty;
    return input.Trim().ToLowerInvariant();
}
```

## Minimal program (top-level statements)
```csharp
using System;

Console.WriteLine("Enter your name:");
var name = Console.ReadLine();
Console.WriteLine($"Hello, {name}!");
```

## Global usings (optional)
Create `GlobalUsings.cs` in your project:
```csharp
global using System;
global using System.Linq;
```

## Next steps
- You’re ready for Web APIs next: routing, validation, and DTOs
- Revisit this chapter as you hit new language features (records, pattern matching, async streams)
