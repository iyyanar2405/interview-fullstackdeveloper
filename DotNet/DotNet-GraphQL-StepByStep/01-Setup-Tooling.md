# 01 — Setup & Tooling

We’ll use Hot Chocolate (ChilliCream) to build GraphQL servers in .NET.

## Install .NET and templates
- .NET SDK 8.0+
- Optional: Hot Chocolate templates (not required but handy)

```powershell
# verify
 dotnet --version

# install dotnet-ef (optional for EF chapter)
 dotnet tool install --global dotnet-ef
```

## Create a new Web project
```powershell
 dotnet new web -o src/GraphApi
```

Add Hot Chocolate packages:
```powershell
 dotnet add src/GraphApi package HotChocolate.AspNetCore
 dotnet add src/GraphApi package HotChocolate.AspNetCore.Voyager
 dotnet add src/GraphApi package HotChocolate.Data
 dotnet add src/GraphApi package HotChocolate.Data.Filters
 dotnet add src/GraphApi package HotChocolate.Data.Sorting
 dotnet add src/GraphApi package HotChocolate.Subscriptions
```

Optional (EF Core integration):
```powershell
 dotnet add src/GraphApi package Microsoft.EntityFrameworkCore
 dotnet add src/GraphApi package Microsoft.EntityFrameworkCore.Sqlite
```

## Run the empty project
```powershell
 dotnet run --project src/GraphApi
```

Next: build a minimal GraphQL API and explore Banana Cake Pop/Voyager.
