using AI.Anthropic.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Add API Explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddClaudeSwagger();

// Add Anthropic Claude services
builder.Services.AddAnthropicClaude(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
    .AddUrlGroup(new Uri("https://api.anthropic.com/v1/messages"), "Anthropic API");

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddEventSourceLogger();
}

// Add application insights if configured
var appInsightsConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights");
if (!string.IsNullOrEmpty(appInsightsConnectionString))
{
    builder.Services.AddApplicationInsightsTelemetry(appInsightsConnectionString);
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseClaudeSwagger(app.Configuration);
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }
    
    await next();
});

app.UseHttpsRedirection();

// CORS
var corsPolicy = app.Environment.IsDevelopment() ? "Development" : "AllowAll";
app.UseCors(corsPolicy);

// Authentication and Authorization (if needed)
// app.UseAuthentication();
// app.UseAuthorization();

// Use Anthropic Claude middleware
app.UseAnthropicClaude();

// Map controllers
app.MapControllers()
   .RequireRateLimiting("ClaudeApi");

// Map health checks
app.MapHealthChecks("/health")
   .AllowAnonymous();

app.MapHealthChecks("/health/ready")
   .AllowAnonymous();

app.MapHealthChecks("/health/live")
   .AllowAnonymous();

// Default route for API information
app.MapGet("/", () => new
{
    Service = "Anthropic Claude API",
    Version = "1.0.0",
    Description = "API for interacting with Anthropic Claude models with Constitutional AI features",
    Documentation = "/swagger",
    Health = "/health",
    Endpoints = new
    {
        Chat = "/api/claude/chat",
        StreamingChat = "/api/claude/chat/stream",
        LongContext = "/api/claude/long-context",
        ConstitutionalValidation = "/api/claude/constitutional/validate",
        ConstitutionalImprovement = "/api/claude/constitutional/improve",
        ConstitutionalCritique = "/api/claude/constitutional/critique",
        TokenEstimation = "/api/claude/tokens/estimate",
        Models = "/api/claude/models"
    },
    Features = new[]
    {
        "Chat Completions",
        "Streaming Responses",
        "Long Context Processing",
        "Constitutional AI Validation",
        "Token Estimation",
        "Cost Calculation",
        "Rate Limiting",
        "Error Handling",
        "Request Logging",
        "Health Monitoring"
    }
})
.AllowAnonymous();

// Global exception handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(error.Error, "Unhandled exception occurred");

            var response = new
            {
                error = "An unexpected error occurred",
                requestId = context.TraceIdentifier,
                timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    });
});

// Request timeout
app.Use(async (context, next) =>
{
    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
    context.RequestAborted = cts.Token;
    await next(context);
});

try
{
    app.Logger.LogInformation("Starting Anthropic Claude API service...");
    app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
    app.Logger.LogInformation("Swagger UI available at: /swagger");
    app.Logger.LogInformation("Health checks available at: /health");
    
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application failed to start");
    throw;
}
finally
{
    app.Logger.LogInformation("Anthropic Claude API service stopped");
}