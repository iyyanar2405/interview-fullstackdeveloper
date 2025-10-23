using Microsoft.OpenApi.Models;
using Module_04_Tool_Integration.HostedServices;
using Module_04_Tool_Integration.Models;
using Module_04_Tool_Integration.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions();
builder.Services.Configure<ToolIntegrationOptions>(builder.Configuration.GetSection("ToolIntegration"));

builder.Services.AddSingleton<ToolCatalogService>();
builder.Services.AddSingleton<ToolExecutionHistoryService>();
builder.Services.AddSingleton<ToolExecutionQueueService>();
builder.Services.AddSingleton<ApiGatewayService>();
builder.Services.AddSingleton<DatabaseExecutorService>();
builder.Services.AddSingleton<FileSystemAutomationService>();
builder.Services.AddSingleton<WebScrapingService>();
builder.Services.AddSingleton<CustomToolService>();
builder.Services.AddSingleton<ToolExecutionOrchestrator>();
builder.Services.AddSingleton<ToolExecutionService>();

builder.Services.AddHostedService<ToolExecutionWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tool Integration API",
        Version = "v1",
        Description = "Simulated tool execution environment covering APIs, databases, file systems, web scraping, and custom tools"
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
