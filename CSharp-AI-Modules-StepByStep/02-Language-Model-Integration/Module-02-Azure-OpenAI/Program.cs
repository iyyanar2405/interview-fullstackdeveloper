using AI.AzureOpenAI.Extensions;
using AI.AzureOpenAI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Azure OpenAI services
builder.Services.AddAzureOpenAI(builder.Configuration);

// Add logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Use Azure OpenAI middleware
app.UseAzureOpenAI();

app.UseAuthorization();
app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    service = "Azure OpenAI API"
});

// Example endpoints for testing
app.MapGet("/", () => new
{
    message = "Azure OpenAI API is running",
    version = "1.0.0",
    timestamp = DateTime.UtcNow,
    endpoints = new[]
    {
        "/api/azureopenai/chat/completions",
        "/api/azureopenai/chat/completions/stream",
        "/api/azureopenai/embeddings",
        "/api/azureopenai/deployments",
        "/api/azureopenai/tokens/count",
        "/api/azureopenai/content/check",
        "/api/azureopenai/usage",
        "/api/azureopenai/health"
    }
});

app.Run();