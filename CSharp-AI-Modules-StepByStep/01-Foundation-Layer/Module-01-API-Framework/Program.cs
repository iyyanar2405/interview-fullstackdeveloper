using AI.Foundation.API.Extensions;
using AI.Foundation.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom services
builder.Services.AddFoundationServices();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Foundation API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at root
    });
}

// Custom middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();

// Controller routes
app.MapControllers();

// Minimal API endpoints
app.MapGet("/", () => "AI Foundation API is running!")
   .WithName("Root")
   .WithTags("General");

app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow })
   .WithName("HealthCheck")
   .WithTags("Health");

// AI-related minimal endpoints
app.MapPost("/api/minimal/chat", async (ChatRequest request) =>
{
    // Simple echo response for demonstration
    var response = new ChatResponse
    {
        Message = $"Echo: {request.Message}",
        Timestamp = DateTime.UtcNow,
        ProcessingTime = TimeSpan.FromMilliseconds(Random.Shared.Next(50, 200))
    };
    
    // Simulate processing delay
    await Task.Delay(50);
    
    return Results.Ok(response);
})
.WithName("MinimalChat")
.WithTags("AI")
.WithOpenApi();

app.Run();

// Request/Response models for minimal API
public record ChatRequest(string Message);

public record ChatResponse
{
    public string Message { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public TimeSpan ProcessingTime { get; init; }
}