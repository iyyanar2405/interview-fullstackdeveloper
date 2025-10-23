using AI.RetrievalStrategies.Extensions;
using AI.RetrievalStrategies.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Retrieval Strategies API", 
        Version = "v1",
        Description = "ASP.NET Core Web API for Advanced Retrieval Strategies including Semantic Search, Hybrid Search, Re-ranking, and Context Optimization"
    });
});

// Configure Retrieval Strategies
builder.Services.AddRetrievalStrategies(options =>
{
    options.OpenAIApiKey = builder.Configuration["OpenAI:ApiKey"] 
        ?? builder.Configuration["Azure:OpenAI:ApiKey"]
        ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    options.OpenAIEndpoint = builder.Configuration["Azure:OpenAI:Endpoint"];
    options.EmbeddingModel = builder.Configuration["OpenAI:EmbeddingModel"] ?? "text-embedding-3-small";
    options.ChatModel = builder.Configuration["OpenAI:ChatModel"] ?? "gpt-4";
    
    options.DefaultTopK = int.Parse(builder.Configuration["Retrieval:DefaultTopK"] ?? "10");
    options.DefaultScoreThreshold = float.Parse(builder.Configuration["Retrieval:DefaultScoreThreshold"] ?? "0.7");
    
    options.EnableCaching = bool.Parse(builder.Configuration["Retrieval:EnableCaching"] ?? "true");
    options.CacheDurationMinutes = int.Parse(builder.Configuration["Retrieval:CacheDurationMinutes"] ?? "30");
    
    options.MaxContextTokens = int.Parse(builder.Configuration["Retrieval:MaxContextTokens"] ?? "4000");
    options.MaxQueryExpansions = int.Parse(builder.Configuration["Retrieval:MaxQueryExpansions"] ?? "5");
    
    options.HybridSemanticWeight = float.Parse(builder.Configuration["Retrieval:HybridSemanticWeight"] ?? "0.7");
    options.HybridKeywordWeight = float.Parse(builder.Configuration["Retrieval:HybridKeywordWeight"] ?? "0.3");
    
    options.MMRLambda = float.Parse(builder.Configuration["Retrieval:MMRLambda"] ?? "0.5");
    options.RRFConstant = int.Parse(builder.Configuration["Retrieval:RRFConstant"] ?? "60");
    
    options.VectorDatabaseEndpoint = builder.Configuration["VectorDatabase:Endpoint"] ?? "http://localhost:5003";
    options.RedisConnectionString = builder.Configuration["Redis:ConnectionString"];
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Retrieval Strategies API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Startup message
app.Logger.LogInformation("Retrieval Strategies API started successfully");
app.Logger.LogInformation("Swagger UI available at: http://localhost:{Port}", 
    builder.Configuration["ASPNETCORE_URLS"]?.Split(':').LastOrDefault()?.TrimEnd('/') ?? "5000");

app.Run();
