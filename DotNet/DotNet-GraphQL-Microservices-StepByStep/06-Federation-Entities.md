# 06 — Federation Entities

Model entities with keys and extend them across services.

## Define keys
```csharp
// Accounts subgraph
[Key("id")]
public record User(int Id, string Name, string Email);
```

## Extend in another subgraph
```csharp
// Products subgraph: add owner field resolving User by id
[ExtendObjectType(typeof(Product))]
public static class ProductExtensions
{
    public static User? Owner([Parent] Product product, [Service] UsersClient client)
        => client.FindById(product.OwnerId);
}
```

With Fusion, configure entity resolution per remote schema; with Apollo Federation, implement `__resolveReference`.

Next: Auth propagation and policies.
