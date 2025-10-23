using AI.RAGPipeline.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/rag-pipeline-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "AI RAG Pipeline API",
        Version = "v1",
        Description = "End-to-end Retrieval Augmented Generation (RAG) pipeline with document ingestion, query processing, context retrieval, and response generation"
    });
});

// Add RAG Pipeline services
builder.Services.AddRAGPipeline(builder.Configuration);

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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => new
{
    Service = "AI RAG Pipeline API",
    Version = "1.0.0",
    Status = "Running",
    Description = "Complete RAG implementation integrating all modules",
    Endpoints = new[]
    {
        "/swagger - API Documentation",
        "/api/rag/ingest - Document Ingestion",
        "/api/rag/query - Query Execution",
        "/api/rag/query/stream - Streaming Query",
        "/api/rag/metrics - Pipeline Metrics",
        "/api/rag/health - Health Check"
    },
    Modules = new[]
    {
        "Module-01: Document Processing (PDF, DOCX, TXT, HTML)",
        "Module-02: Embeddings (OpenAI, Azure OpenAI)",
        "Module-03: Vector Databases (In-Memory, Qdrant)",
        "Module-04: Retrieval Strategies (Semantic, Hybrid, Multi-Query, HyDE)",
        "Module-05: Knowledge Management (Knowledge Bases, Versioning)",
        "Module-06: RAG Pipeline (Complete Integration)"
    }
});

Log.Information("AI RAG Pipeline API starting up...");

app.Run();

Log.Information("AI RAG Pipeline API shutting down...");
Log.CloseAndFlush();
