# 05 — Gateway Composition

Compose subgraphs into a unified API with Hot Chocolate Fusion (or Apollo Gateway).

## Fusion (Hot Chocolate)
```powershell
 dotnet add src/GraphGateway package HotChocolate.Fusion
 dotnet add src/GraphGateway package HotChocolate.AspNetCore
```

Minimal gateway:
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddFusionGateway()
    .AddRemoteSchema("accounts", c => c.Http(new Uri("http://localhost:5001/graphql")))
    .AddRemoteSchema("products", c => c.Http(new Uri("http://localhost:5002/graphql")));

var app = builder.Build();
app.MapGraphQL();
app.Run();
```

Run services on different ports (e.g., Accounts :5001, Products :5002), then run gateway (default :5000).

## Apollo Gateway (alternative)
If you prefer Apollo, expose subgraphs with Federation support and configure Apollo Gateway with their YAML/JS config.

Next: Entities and cross-service references.
