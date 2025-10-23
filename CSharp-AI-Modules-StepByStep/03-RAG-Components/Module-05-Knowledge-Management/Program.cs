using AI.KnowledgeManagement.Data;
using AI.KnowledgeManagement.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/knowledge-management-.log", rollingInterval: RollingInterval.Day)
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
        Title = "AI Knowledge Management API", 
        Version = "v1",
        Description = "Comprehensive knowledge management system with versioning, access control, and analytics"
    });
});

// Add Knowledge Management services
builder.Services.AddKnowledgeManagement(builder.Configuration);

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

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<KnowledgeDbContext>();
    try
    {
        context.Database.Migrate();
        Log.Information("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error applying database migrations");
    }
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => new
{
    Service = "AI Knowledge Management API",
    Version = "1.0.0",
    Status = "Running",
    Endpoints = new[]
    {
        "/swagger - API Documentation",
        "/api/knowledgemanagement/knowledge-bases - Knowledge Base Management",
        "/api/knowledgemanagement/documents - Document Management",
        "/api/knowledgemanagement/search - Search Operations",
        "/api/knowledgemanagement/analytics - Analytics",
        "/api/knowledgemanagement/sync - Synchronization Jobs"
    }
});

Log.Information("AI Knowledge Management API starting up...");

app.Run();

Log.Information("AI Knowledge Management API shutting down...");
Log.CloseAndFlush();
