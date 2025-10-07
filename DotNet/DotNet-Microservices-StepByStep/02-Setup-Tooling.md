# 02 — Setup & Tooling

## Requirements
- .NET SDK 8.0+
- Docker Desktop (for containers, optional now)

## Solution layout
```
DotNet-Microservices-StepByStep/
  src/
    CatalogService/
    OrdersService/
    ApiGateway/
  tests/
    Catalog.Tests/
    Orders.Tests/
    E2E.Tests/
```

## Create projects
```powershell
 dotnet new web -o src/CatalogService
 dotnet new web -o src/OrdersService
 dotnet new web -o src/ApiGateway
```

## Recommended packages
```powershell
 # API basics
 dotnet add src/* package Swashbuckle.AspNetCore
 dotnet add src/* package Microsoft.AspNetCore.Mvc.Versioning
 
 # Http + resilience
 dotnet add src/* package Polly.Extensions.Http
 
 # Observability
 dotnet add src/* package Serilog.AspNetCore
 dotnet add src/* package OpenTelemetry.Exporter.Console
 dotnet add src/* package OpenTelemetry.Extensions.Hosting
```

Next: Define solution structure conventions.
