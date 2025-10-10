# 10 — Async & Await Basics

Concepts: Task, async/await, I/O bound ops.

```csharp
public async Task<int> FetchAsync() { await Task.Delay(100); return 42; }
```
