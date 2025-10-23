using AI.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Dependency Injection Patterns API",
        Version = "v1",
        Description = "Demonstrates advanced dependency injection patterns in .NET"
    });
});

// Add dependency injection module with all patterns
builder.Services.AddDependencyInjectionModule();

// Configure service options from configuration
builder.Services.ConfigureServiceOptions(builder.Configuration);

// Add service lifetime validation
builder.Services.AddServiceLifetimeValidation();

// Add logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
    if (builder.Environment.IsDevelopment())
    {
        config.SetMinimumLevel(LogLevel.Debug);
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dependency Injection Patterns API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseCors();

// Use dependency injection validation
app.UseDependencyInjectionValidation();

// Use service information middleware
app.UseServiceInformation();

app.UseAuthorization();
app.MapControllers();

// Add health check endpoint
app.MapHealthChecks("/health");

// Add root endpoint with API information
app.MapGet("/api", () => new
{
    title = "Dependency Injection Patterns API",
    version = "1.0.0",
    description = "Demonstrates advanced dependency injection patterns and service management",
    timestamp = DateTime.UtcNow,
    patterns = new[]
    {
        "Service Factory - Dynamic service creation",
        "Named Service Resolver - Service resolution by name",
        "Lazy Service - Deferred service initialization",
        "Decorator Pattern - Service enhancement",
        "Custom Scope - Isolated service scopes",
        "Service Validation - Registration validation"
    },
    endpoints = new
    {
        users = "/api/users",
        patterns = "/api/dependencyinjection",
        patternsInfo = "/api/dependencyinjection/patterns-info",
        health = "/health",
        swagger = "/swagger"
    }
});

// Example endpoint demonstrating service resolution
app.MapGet("/api/services/info", (IServiceProvider serviceProvider) =>
{
    try
    {
        var userService = serviceProvider.GetService<AI.DependencyInjection.Services.IUserService>();
        var cacheService = serviceProvider.GetService<AI.DependencyInjection.Services.ICacheService>();
        var notificationService = serviceProvider.GetService<AI.DependencyInjection.Services.INotificationService>();
        
        return Results.Ok(new
        {
            message = "Service resolution successful",
            services = new
            {
                userService = userService?.GetType().Name ?? "Not resolved",
                cacheService = cacheService?.GetType().Name ?? "Not resolved",
                notificationService = notificationService?.GetType().Name ?? "Not resolved"
            },
            timestamp = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Service resolution failed: {ex.Message}");
    }
});

// Demonstration endpoint for service lifetimes
app.MapGet("/api/services/lifetimes", (IServiceProvider serviceProvider) =>
{
    var services = new Dictionary<string, object>();
    
    // Demonstrate different service lifetimes
    using (var scope1 = serviceProvider.CreateScope())
    {
        var userService1 = scope1.ServiceProvider.GetRequiredService<AI.DependencyInjection.Services.IUserService>();
        var cacheService1 = scope1.ServiceProvider.GetRequiredService<AI.DependencyInjection.Services.ICacheService>();
        
        using (var scope2 = serviceProvider.CreateScope())
        {
            var userService2 = scope2.ServiceProvider.GetRequiredService<AI.DependencyInjection.Services.IUserService>();
            var cacheService2 = scope2.ServiceProvider.GetRequiredService<AI.DependencyInjection.Services.ICacheService>();
            
            return Results.Ok(new
            {
                demonstration = "Service Lifetime Comparison",
                scoped = new
                {
                    description = "New instance per scope",
                    userServiceSame = ReferenceEquals(userService1, userService2),
                    scope1HashCode = userService1.GetHashCode(),
                    scope2HashCode = userService2.GetHashCode()
                },
                singleton = new
                {
                    description = "Same instance across scopes",
                    cacheServiceSame = ReferenceEquals(cacheService1, cacheService2),
                    scope1HashCode = cacheService1.GetHashCode(),
                    scope2HashCode = cacheService2.GetHashCode()
                },
                timestamp = DateTime.UtcNow
            });
        }
    }
});

app.Run();