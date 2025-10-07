# 10 — Authentication and Authorization

Add JWT bearer auth, policies, and (optionally) ASP.NET Core Identity.

## JWT Bearer authentication
```powershell
 dotnet add src/MyApi package Microsoft.AspNetCore.Authentication.JwtBearer
```

`Program.cs`:
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Jwt:Authority"]; // e.g., https://demo.duendesoftware.com
        options.Audience = builder.Configuration["Jwt:Audience"];   // e.g., api
        options.RequireHttpsMetadata = false; // dev only
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admins", p => p.RequireClaim("role", "admin"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/secure", [Microsoft.AspNetCore.Authorization.Authorize] () => "secret");
app.MapGet("/admin", [Microsoft.AspNetCore.Authorization.Authorize(Policy = "admins")] () => "admin only");

app.Run();
```

`appsettings.json` (example):
```json
{
  "Jwt": {
    "Authority": "https://demo.duendesoftware.com",
    "Audience": "api"
  }
}
```

## Role- and policy-based authorization
- Roles via claims (e.g., `role`)
- Policies for complex rules (requirements/handlers)

## ASP.NET Core Identity (optional)
```powershell
 dotnet add src/MyApi package Microsoft.AspNetCore.Identity.EntityFrameworkCore
 dotnet add src/MyApi package Microsoft.EntityFrameworkCore.Sqlite
```

Scaffold Identity UI (if using MVC): https://learn.microsoft.com/aspnet/core/security/authentication/scaffold-identity

## Next steps
- Dockerize and deploy
