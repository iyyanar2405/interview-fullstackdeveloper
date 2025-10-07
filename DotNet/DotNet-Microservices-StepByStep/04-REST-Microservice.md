# 04 — REST Microservice (Minimal API)

Create a minimal REST service with Swagger, versioning, and validation.

## Program.cs
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

var items = new List<Item>();

app.MapGet("/api/v1/items", () => items);
app.MapPost("/api/v1/items", (CreateItemRequest input) =>
{
    if (string.IsNullOrWhiteSpace(input.Name))
        return Results.BadRequest(new { error = "Name required" });

    var item = new Item(Guid.NewGuid(), input.Name);
    items.Add(item);
    return Results.Created($"/api/v1/items/{item.Id}", item);
});

app.Run();

public record Item(Guid Id, string Name);
public record CreateItemRequest(string Name);
```

Run:
```powershell
 dotnet run --project src/CatalogService
```

Next: API Gateway with YARP.
