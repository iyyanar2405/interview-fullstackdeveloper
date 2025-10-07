# 02 — Setup & Tooling

Solution structure and dependencies for services and gateway.

## Suggested layout
```
DotNet-GraphQL-Microservices-StepByStep/
  src/
    AccountsApi/
    ProductsApi/
    GraphGateway/
  tests/
    E2E.Tests/
```

## Create projects
```powershell
 dotnet new web -o src/AccountsApi
 dotnet new web -o src/ProductsApi
 dotnet new web -o src/GraphGateway
```

## Add packages
```powershell
 # Services
 dotnet add src/AccountsApi package HotChocolate.AspNetCore
 dotnet add src/ProductsApi package HotChocolate.AspNetCore
 
 # Gateway (Fusion)
 dotnet add src/GraphGateway package HotChocolate.Fusion
 dotnet add src/GraphGateway package HotChocolate.AspNetCore
```

Optional
```powershell
 dotnet add src/* package Serilog.AspNetCore
 dotnet add src/* package Microsoft.AspNetCore.Authentication.JwtBearer
```

Next: Build an Accounts subgraph.
