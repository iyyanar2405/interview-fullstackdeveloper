# 02 — Minimal GraphQL API

Create the smallest working GraphQL server with Hot Chocolate and test it.

## Program.cs
```csharp
using HotChocolate;
using HotChocolate.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

app.MapGraphQL("/graphql");

// Optional: Voyager or Banana Cake Pop IDE
app.UseVoyager(new() { GraphQLEndPoint = "/graphql" }, "/voyager");

app.Run();

public class Query
{
    public string Hello() => "world";
}
```

## Run and explore
```powershell
 dotnet run --project src/GraphApi
```

Open in the browser:
- GraphQL endpoint: http://localhost:5000/graphql
- Voyager: http://localhost:5000/voyager

Query example:
```graphql
query {
  hello
}
```

You should get:
```json
{
  "data": {
    "hello": "world"
  }
}
```

Next: define shapes with schema types and build resolvers.
