# 04 — Inheritance & Polymorphism

Concepts: base/derived classes, virtual/override, new keyword.

```csharp
public class Animal { public virtual string Speak() => "..."; }
public class Dog : Animal { public override string Speak() => "Woof"; }
```
