# 11 — Client Integration

Call GraphQL from .NET and consider codegen with Strawberry Shake.

## Simple HttpClient
```csharp
var http = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
var payload = new { query = "query { hello }" };
var res = await http.PostAsJsonAsync("/graphql", payload);
res.EnsureSuccessStatusCode();
var json = await res.Content.ReadAsStringAsync();
```

## Strawberry Shake (optional)
```powershell
 dotnet tool install -g StrawberryShake.Tools
```

Configure a client (docs): https://chillicream.com/docs/strawberryshake

Next: Dockerizing and deploying the GraphQL server.
