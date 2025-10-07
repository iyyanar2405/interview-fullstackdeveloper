# 09 — Testing with xUnit

Unit tests for services and integration tests for Web APIs.

## xUnit setup
```powershell
 dotnet new xunit -o tests/MyApi.Tests
 dotnet add tests/MyApi.Tests reference src/MyApi
 dotnet test
```

## Unit test example
```csharp
public class MathUtils
{
    public static int Add(int a, int b) => a + b;
}

public class MathUtilsTests
{
    [Fact]
    public void Add_Works()
    {
        Assert.Equal(5, MathUtils.Add(2, 3));
    }
}
```

## Integration test with WebApplicationFactory
```powershell
 dotnet add tests/MyApi.Tests package Microsoft.AspNetCore.Mvc.Testing
```

```csharp
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public ApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task TimeEndpoint_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/time");
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(body);
        Assert.True(body!.ContainsKey("now"));
    }
}
```

## Test tips
- Keep tests fast and deterministic
- Use `WebApplicationFactory` for HTTP-level assertions
- Prefer in-memory or Sqlite for data tests
