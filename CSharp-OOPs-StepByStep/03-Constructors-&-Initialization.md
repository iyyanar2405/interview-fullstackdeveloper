# 03 — Constructors & Initialization

Concepts: default/parameterized constructors, init-only setters.

```csharp
public class Person
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public Person(string first, string last)
    {
        FirstName = first; LastName = last;
    }
}
```
