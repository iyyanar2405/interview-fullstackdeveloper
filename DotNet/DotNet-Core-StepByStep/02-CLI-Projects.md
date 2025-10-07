# 02 — CLI Projects (Console, Web API, Tests)

Create and run .NET projects with the CLI.

## Console app
```powershell
# create
 dotnet new console -o src/HelloConsole
# run
 dotnet run --project src/HelloConsole
```

Program.cs template (top-level):
```csharp
Console.WriteLine("Hello from .NET!");
```

## Class Library and reference
```powershell
 dotnet new classlib -o src/Utilities
 dotnet add src/HelloConsole reference src/Utilities
```

## xUnit Test project
```powershell
 dotnet new xunit -o tests/HelloConsole.Tests
 dotnet add tests/HelloConsole.Tests reference src/Utilities
 dotnet test
```

## ASP.NET Core Web API (Minimal API)
```powershell
 dotnet new webapi -o src/MyApi --no-https
 dotnet run --project src/MyApi
```

Program.cs (snippet):
```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/ping", () => Results.Ok(new { ok = true, now = DateTime.UtcNow }));

app.Run();
```

## Swagger/OpenAPI
```powershell
 dotnet add src/MyApi package Swashbuckle.AspNetCore
```

Program.cs (snippet):
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/weather", () => new[] { "sunny", "rainy" });
app.Run();
```

## Configuration profiles
```powershell
 dotnet build -c Release
 dotnet run --project src/MyApi --environment Production
```
