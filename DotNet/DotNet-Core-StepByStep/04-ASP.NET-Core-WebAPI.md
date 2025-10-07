# 04 — ASP.NET Core Web API

Build a Minimal API and a Controller-based API with Swagger.

## Minimal API quickstart
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/time", () => Results.Ok(new { now = DateTime.UtcNow }));
app.MapPost("/api/echo", (Message msg) => Results.Ok(msg));

app.Run();

record Message(string Text);
```

Run it:
```powershell
 dotnet run --project src/MyApi
```

## Controllers project
```powershell
 dotnet new webapi -o src/ContactsApi --no-https
```

`Program.cs` (key bits):
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
```

`Controllers/ContactsController.cs`:
```csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private static readonly List<Contact> Contacts = new()
    {
        new Contact(1, "Ada", "ada@example.com"),
        new Contact(2, "Linus", "linus@example.com")
    };

    [HttpGet]
    public ActionResult<IEnumerable<Contact>> Get() => Contacts;

    [HttpGet("{id}")]
    public ActionResult<Contact> Get(int id)
        => Contacts.FirstOrDefault(c => c.Id == id) is { } c ? c : NotFound();

    [HttpPost]
    public ActionResult<Contact> Create(Contact input)
    {
        var nextId = Contacts.Any() ? Contacts.Max(c => c.Id) + 1 : 1;
        var created = input with { Id = nextId };
        Contacts.Add(created);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
}

public record Contact(int Id, string Name, string Email);
```

## Model validation
```csharp
public record CreateContactRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.MinLength(2)]
    public string Name { get; init; } = string.Empty;

    [System.ComponentModel.DataAnnotations.EmailAddress]
    public string Email { get; init; } = string.Empty;
}
```

## DTOs and mapping
- Use records for request/response models
- Consider a mapper (e.g., Mapster/AutoMapper) for complex domains

## Versioning and CORS (quick add)
```csharp
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

app.UseCors();
```

## Next steps
- Middleware & DI
- Configuration & Options
- EF Core persistence
