# 08 — DataLoader & N+1

Batch requests and cache results per request to avoid N+1 across services.

## Per-service DataLoader
```csharp
public class UserByIdDataLoader : BatchDataLoader<int, User>
{
    private readonly UsersClient _client;
    public UserByIdDataLoader(IBatchScheduler sched, UsersClient client) : base(sched)
    { _client = client; }

    protected override async Task<IReadOnlyDictionary<int, User>> LoadBatchAsync(IReadOnlyList<int> keys, CancellationToken ct)
    {
        var result = await _client.GetUsersAsync(keys, ct);
        return result.ToDictionary(u => u.Id);
    }
}
```

Use in field resolver:
```csharp
public static class ProductExtensions
{
    public static Task<User> Owner([Parent] Product p, UserByIdDataLoader dl, CancellationToken ct)
        => dl.LoadAsync(p.OwnerId, ct);
}
```

Next: Resilience and messaging.
