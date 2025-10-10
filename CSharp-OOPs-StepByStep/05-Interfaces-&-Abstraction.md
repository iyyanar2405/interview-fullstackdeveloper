# 05 — Interfaces & Abstraction

Concepts: interfaces, abstract classes, multiple interface implementation.

```csharp
public interface IWalk { void Walk(); }
public abstract class Animal { public abstract string Speak(); }
public class RobotDog : Animal, IWalk { public override string Speak() => "Beep"; public void Walk() {} }
```
