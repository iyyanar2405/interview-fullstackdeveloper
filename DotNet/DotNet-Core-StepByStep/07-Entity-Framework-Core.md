# 07 — Entity Framework Core

Add EF Core to your Web API, create a DbContext, run migrations, and query data.

## Install packages
```powershell
 dotnet add src/MyApi package Microsoft.EntityFrameworkCore
 dotnet add src/MyApi package Microsoft.EntityFrameworkCore.Sqlite
 dotnet add src/MyApi package Microsoft.EntityFrameworkCore.Tools
```

## Define model and DbContext
```csharp
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<TodoItem> Todos => Set<TodoItem>();
}
```

## Register DbContext (Sqlite)
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("db") ?? "Data Source=app.db"));
```

`appsettings.json`:
```json
{
  "ConnectionStrings": {
    "db": "Data Source=app.db"
  }
}
```

## Create and apply migrations
```powershell
 dotnet ef migrations add InitialCreate --project src/MyApi
 dotnet ef database update --project src/MyApi
```

If the `dotnet ef` tool is missing:
```powershell
 dotnet tool install --global dotnet-ef
```

## Minimal API endpoints using EF Core
```csharp
app.MapGet("/todos", async (AppDbContext db) => await db.Todos.AsNoTracking().ToListAsync());

app.MapPost("/todos", async (AppDbContext db, TodoItem input) =>
{
    db.Todos.Add(input);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{input.Id}", input);
});

app.MapPut("/todos/{id}", async (AppDbContext db, int id, TodoItem update) =>
{
    var existing = await db.Todos.FindAsync(id);
    if (existing is null) return Results.NotFound();
    existing.Title = update.Title;
    existing.IsDone = update.IsDone;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todos/{id}", async (AppDbContext db, int id) =>
{
    var existing = await db.Todos.FindAsync(id);
    if (existing is null) return Results.NotFound();
    db.Todos.Remove(existing);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
```

## Repository pattern (optional)
```csharp
public interface ITodoRepository
{
    Task<List<TodoItem>> GetAllAsync(CancellationToken ct);
}

public class EfTodoRepository(AppDbContext db) : ITodoRepository
{
    public Task<List<TodoItem>> GetAllAsync(CancellationToken ct) => db.Todos.AsNoTracking().ToListAsync(ct);
}

builder.Services.AddScoped<ITodoRepository, EfTodoRepository>();
```

## Next steps
- Logging and Health Checks
- Testing (unit + integration)
