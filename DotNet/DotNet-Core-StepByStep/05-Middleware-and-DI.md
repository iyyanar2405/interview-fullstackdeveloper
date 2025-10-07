# 05 — Middleware and Dependency Injection

Understand the request pipeline and register services with DI.

## Middleware pipeline
```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Custom middleware (inline)
app.Use(async (ctx, next) =>
{
    var start = DateTime.UtcNow;
    await next();
    var elapsed = DateTime.UtcNow - start;
    Console.WriteLine($"{ctx.Request.Method} {ctx.Request.Path} took {elapsed.TotalMilliseconds:F0} ms");
});

app.MapGet("/hi", () => "hello");
app.Run();
```

Order matters: Use, When, Map, then terminal handlers (MapGet/MapPost etc.).

## Creating reusable middleware
```csharp
public class TimingMiddleware
{
    private readonly RequestDelegate _next;
    public TimingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTime.UtcNow;
        await _next(context);
        var elapsed = DateTime.UtcNow - start;
        context.Response.Headers["X-Elapsed-ms"] = elapsed.TotalMilliseconds.ToString("F0");
    }
}

public static class TimingMiddlewareExtensions
{
    public static IApplicationBuilder UseTiming(this IApplicationBuilder app)
        => app.UseMiddleware<TimingMiddleware>();
}

// Program.cs
app.UseTiming();
```

## Built-in middleware highlights
- Exception handler: `app.UseExceptionHandler("/error")`
- HSTS/HTTPS redirection
- Static files, Routing, CORS, Authentication, Authorization

## DI basics: register and consume services
```csharp
// Define a service
public interface IClock
{
    DateTime UtcNow { get; }
}

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}

// Register
builder.Services.AddSingleton<IClock, SystemClock>();

// Consume in Minimal API
app.MapGet("/now", (IClock clock) => new { now = clock.UtcNow });
```

Lifetimes:
- Singleton: one per app
- Scoped: one per request
- Transient: new each injection

## Options pattern (preview)
```csharp
builder.Services.Configure<MyOptions>(builder.Configuration.GetSection("MyOptions"));

app.MapGet("/cfg", (Microsoft.Extensions.Options.IOptions<MyOptions> opt) => opt.Value);

public record MyOptions(string Greeting, int MaxItems);
```

## Next steps
- Configuration and Options in depth
- EF Core and data access
