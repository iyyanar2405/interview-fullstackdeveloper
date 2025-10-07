# 10 — Testing GraphQL

Test resolvers and endpoints with xUnit and WebApplicationFactory.

## Setup test project
```powershell
 dotnet new xunit -o tests/GraphApi.Tests
 dotnet add tests/GraphApi.Tests reference src/GraphApi
 dotnet add tests/GraphApi.Tests package Microsoft.AspNetCore.Mvc.Testing
```

## Basic endpoint test
```csharp
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

public class GraphTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public GraphTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Hello_Works()
    {
        var client = _factory.CreateClient();
        var payload = new { query = "query { hello }" };
        var res = await client.PostAsJsonAsync("/graphql", payload);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(json);
        Assert.Contains("data", json!.Keys);
    }
}
```

Next: Client integration and code generation.
