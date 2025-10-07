# 03 — Accounts Service (Subgraph)

A minimal Accounts GraphQL service exposing User entity.

## Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddType<UserType>();

var app = builder.Build();
app.MapGraphQL();
app.Run();

public record User(int Id, string Name, string Email);

public class Query
{
    public IEnumerable<User> Users() => new[]
    {
        new User(1, "Ada", "ada@example.com"),
        new User(2, "Linus", "linus@example.com")
    };
}

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> d)
    {
        d.Field(f => f.Id).Type<NonNullType<IdType>>();
        d.Field(f => f.Name).Type<NonNullType<StringType>>();
        d.Field(f => f.Email).Type<NonNullType<StringType>>();
    }
}
```

Run:
```powershell
 dotnet run --project src/AccountsApi
```

Query:
```graphql
query { users { id name email } }
```

Next: Products subgraph.
