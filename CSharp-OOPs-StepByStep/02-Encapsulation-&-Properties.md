# 02 — Encapsulation & Properties

Concepts: private fields, public properties, validation.

Exercise:
- Add age with validation.

```csharp
public class Person
{
    private int _age;
    public int Age
    {
        get => _age;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException("Age must be >= 0");
            _age = value;
        }
    }
}
```
