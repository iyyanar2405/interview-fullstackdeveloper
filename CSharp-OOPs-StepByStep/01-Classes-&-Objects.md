# 01 — Classes & Objects

Concepts: class, object, fields, methods.

Exercise:
- Create `Person` class with fields and methods; instantiate and call methods.

Example snippet (see full code in examples project):

```csharp
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string FullName() => $"{FirstName} {LastName}";
}
```
