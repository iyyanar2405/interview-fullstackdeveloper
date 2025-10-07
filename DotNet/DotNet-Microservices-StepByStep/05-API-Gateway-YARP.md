# 05 — API Gateway (YARP)

Use YARP as a reverse proxy/gateway to route requests to services.

## Add YARP
```powershell
 dotnet add src/ApiGateway package Yarp.ReverseProxy
```

## Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromMemory(new[]
    {
        new Yarp.ReverseProxy.Configuration.RouteConfig
        {
            RouteId = "catalog",
            ClusterId = "catalog-cluster",
            Match = new() { Path = "/catalog/{**catch-all}" }
        }
    },
    new[]
    {
        new Yarp.ReverseProxy.Configuration.ClusterConfig
        {
            ClusterId = "catalog-cluster",
            Destinations = new Dictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig>
            {
                ["d1"] = new() { Address = "http://localhost:5001/" }
            }
        }
    });

var app = builder.Build();
app.MapReverseProxy();
app.Run();
```

Now calls to `/catalog/...` are forwarded to the CatalogService.

Next: Config & service discovery patterns.
