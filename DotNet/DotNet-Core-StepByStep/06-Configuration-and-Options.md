# 06 — Configuration and Options

Load settings from appsettings, env vars, user-secrets, and bind to typed options.

## Configuration sources (default order)
- appsettings.json
- appsettings.{Environment}.json
- User secrets (Development)
- Environment variables
- Command-line args

```csharp
var builder = WebApplication.CreateBuilder(args);
// builder.Configuration already wired with defaults
var value = builder.Configuration["MyOptions:Greeting"]; // string?
```

`appsettings.json`:
```json
{
  "MyOptions": {
    "Greeting": "Hello",
    "MaxItems": 100
  }
}
```

`Program.cs` — bind to typed options:
```csharp
builder.Services.Configure<MyOptions>(builder.Configuration.GetSection("MyOptions"));

app.MapGet("/opts", (Microsoft.Extensions.Options.IOptions<MyOptions> o) => o.Value);

public record MyOptions
{
    public string Greeting { get; init; } = "Hello";
    public int MaxItems { get; init; } = 100;
}
```

## Environment-specific config
`appsettings.Development.json`, `appsettings.Production.json` override values.

Run with environment:
```powershell
 dotnet run --project src/MyApi --environment Production
```

## User secrets (Development)
```powershell
 dotnet user-secrets init --project src/MyApi
 dotnet user-secrets set "MyOptions:Greeting" "Secret Hello" --project src/MyApi
```

## Options validation
```csharp
builder.Services
    .AddOptions<MyOptions>()
    .Bind(builder.Configuration.GetSection("MyOptions"))
    .Validate(o => o.MaxItems > 0 && o.MaxItems <= 1000, "MaxItems must be 1..1000")
    .ValidateOnStart();
```

## Reload on change
appsettings.json changes reload automatically when `reloadOnChange: true` (default in Web).

## Next steps
- Persist data with EF Core
- Health checks and logging
