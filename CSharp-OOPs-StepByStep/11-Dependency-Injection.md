# 11 — Dependency Injection

Concepts: constructor injection, interfaces for testability.

```csharp
public interface IClock { DateTime Now { get; } }
public class SystemClock : IClock { public DateTime Now => DateTime.UtcNow; }
public class Greeter { private readonly IClock _clock; public Greeter(IClock clock){_clock=clock;} public string Greet(string name)=>$"Hello {name} at {_clock.Now:O}"; }
```
