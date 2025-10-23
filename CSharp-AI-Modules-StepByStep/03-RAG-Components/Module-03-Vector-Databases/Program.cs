using AI.VectorDatabases.Extensions;
using AI.VectorDatabases.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Vector Database API", 
        Version = "v1",
        Description = "ASP.NET Core Web API for Vector Database Operations with Qdrant and In-Memory support"
    });
});

// Configure Vector Databases
builder.Services.AddVectorDatabases(options =>
{
    options.DefaultProvider = VectorDatabaseProvider.InMemory;
    options.QdrantHost = builder.Configuration["VectorDatabase:Qdrant:Host"] ?? "localhost";
    options.QdrantPort = int.Parse(builder.Configuration["VectorDatabase:Qdrant:Port"] ?? "6334");
    options.QdrantApiKey = builder.Configuration["VectorDatabase:Qdrant:ApiKey"];
    options.QdrantUseTls = bool.Parse(builder.Configuration["VectorDatabase:Qdrant:UseTls"] ?? "false");
    options.InMemoryMaxCollections = int.Parse(builder.Configuration["VectorDatabase:InMemory:MaxCollections"] ?? "10");
    options.InMemoryMaxPointsPerCollection = int.Parse(builder.Configuration["VectorDatabase:InMemory:MaxPointsPerCollection"] ?? "100000");
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vector Database API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Startup message
app.Logger.LogInformation("Vector Database API started successfully");
app.Logger.LogInformation("Swagger UI available at: http://localhost:{Port}", 
    builder.Configuration["ASPNETCORE_URLS"]?.Split(':').LastOrDefault()?.TrimEnd('/') ?? "5000");

app.Run();
