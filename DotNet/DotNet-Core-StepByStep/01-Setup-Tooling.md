# 01 — Setup & Tooling

Get the .NET SDK, editors, and CLI basics ready.

## Install
- .NET SDK: https://dotnet.microsoft.com/download
- Editor: Visual Studio 2022 (Windows) or VS Code
- Optional: Git, Postman, Docker Desktop

## Verify SDK
```powershell
# Show installed SDKs and version
 dotnet --info
 dotnet --list-sdks
```

## Folder structure (recommended)
```
src/
  MyApp/             # app
  MyApp.Contracts/   # shared models/interfaces
  MyApp.Infrastructure/ # EF Core, persistence
  MyApp.Tests/       # tests
```

## CLI Basics
```powershell
# New console app
 dotnet new console -o src/MyApp

# Restore/build/run
 dotnet restore src/MyApp
 dotnet build src/MyApp -c Release
 dotnet run --project src/MyApp

# Add package (example: Serilog)
 dotnet add src/MyApp package Serilog.AspNetCore
```

## VS Code Extensions (nice to have)
- C# (official)
- C# Dev Kit (if you prefer)
- .NET Install Tool
- NuGet Gallery
- REST Client or Thunder Client

## Troubleshooting
- If `dotnet` isn’t recognized, re-open PowerShell or add SDK path to PATH.
- Use `dotnet nuget locals all --clear` if restore gets stuck.
