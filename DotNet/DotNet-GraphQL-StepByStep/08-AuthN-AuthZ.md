# 08 — Authentication & Authorization

Protect schemas and fields with policies and JWT bearer auth.

## Add JWT bearer
```powershell
 dotnet add src/GraphApi package Microsoft.AspNetCore.Authentication.JwtBearer
 dotnet add src/GraphApi package HotChocolate.AspNetCore.Authorization
```

```csharp
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Jwt:Authority"];
        options.Audience = builder.Configuration["Jwt:Audience"];
        options.RequireHttpsMetadata = false; // dev only
    });

builder.Services.AddAuthorization();

builder.Services
    .AddGraphQLServer()
    .AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL();
```

## Apply [Authorize]
```csharp
public class Query
{
    [Authorize]
    public IEnumerable<Book> GetSecureBooks([Service] BooksService svc) => svc.All();

    [Authorize(Roles = new[] { "admin" })]
    public IEnumerable<Book> GetAdminBooks([Service] BooksService svc) => svc.All();
}
```

Field-level auth is also supported via descriptors.

Next: Errors, result types, and validation.
