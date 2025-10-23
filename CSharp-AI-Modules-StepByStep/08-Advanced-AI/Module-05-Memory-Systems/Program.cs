using Microsoft.OpenApi.Models;
using Module_05_Memory_Systems.HostedServices;
using Module_05_Memory_Systems.Models;
using Module_05_Memory_Systems.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions();
builder.Services.Configure<MemorySystemsOptions>(builder.Configuration.GetSection("MemorySystems"));

builder.Services.AddSingleton<MemoryProfileCatalogService>();
builder.Services.AddSingleton<MemoryStoreService>();
builder.Services.AddSingleton<MemoryRetrievalService>();
builder.Services.AddSingleton<MemoryAnalyticsService>();
builder.Services.AddSingleton<MemoryConsolidationService>();

builder.Services.AddHostedService<MemoryConsolidationWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Memory Systems API",
        Version = "v1",
        Description = "Reference implementation of AI memory services including short-term, long-term, episodic, and semantic stores"
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
