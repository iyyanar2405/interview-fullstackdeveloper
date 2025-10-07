# 08 — Logging and Health Checks

Add structured logging and basic health endpoints.

## Logging
- Built-in logging via `ILogger<T>`
- Add Serilog for structured logging

```powershell
 dotnet add src/MyApi package Serilog.AspNetCore
 dotnet add src/MyApi package Serilog.Sinks.Console
```

```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.MapGet("/work", (ILoggerFactory lf) =>
{
    var log = lf.CreateLogger("Work");
    log.LogInformation("Doing some work at {Time}", DateTime.UtcNow);
    return Results.Ok();
});
```

## Health checks
```powershell
 dotnet add src/MyApi package AspNetCore.HealthChecks.UI.Client
 dotnet add src/MyApi package AspNetCore.HealthChecks.Sqlite
 dotnet add src/MyApi package Microsoft.Extensions.Diagnostics.HealthChecks
```

```csharp
builder.Services.AddHealthChecks()
    .AddSqlite(builder.Configuration.GetConnectionString("db") ?? "Data Source=app.db", name: "database");

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = AspNetCore.HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
});
```

Optional UI: https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks

## Next steps
- Testing with xUnit and WebApplicationFactory
