# 04 — Products Service (Subgraph)

A minimal Products GraphQL service exposing Product entity.

## Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddType<ProductType>();

var app = builder.Build();
app.MapGraphQL();
app.Run();

public record Product(int Id, string Name, decimal Price, int OwnerId);

public class Query
{
    public IEnumerable<Product> Products() => new[]
    {
        new Product(1, "Keyboard", 49.99m, 1),
        new Product(2, "Mouse", 29.99m, 2)
    };
}

public class ProductType : ObjectType<Product>
{
    protected override void Configure(IObjectTypeDescriptor<Product> d)
    {
        d.Field(f => f.Id).Type<NonNullType<IdType>>();
        d.Field(f => f.Name).Type<NonNullType<StringType>>();
        d.Field(f => f.Price).Type<NonNullType<DecimalType>>();
        d.Field(f => f.OwnerId).Ignore();
    }
}
```

Run:
```powershell
 dotnet run --project src/ProductsApi
```

Next: compose via a gateway.
