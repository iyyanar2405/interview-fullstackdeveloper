using Microsoft.OpenApi.Models;
using Module_01_Agentic_AI.HostedServices;
using Module_01_Agentic_AI.Models;
using Module_01_Agentic_AI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions();
builder.Services.Configure<AgenticOptions>(builder.Configuration.GetSection("Agentic"));

builder.Services.AddSingleton<AgentCatalogService>();
builder.Services.AddSingleton<ToolCatalogService>();
builder.Services.AddSingleton<PlanSynthesisService>();
builder.Services.AddSingleton<MemoryStoreService>();
builder.Services.AddSingleton<AgentTaskRegistryService>();
builder.Services.AddSingleton<AgentTaskQueueService>();
builder.Services.AddSingleton<AgentExecutionService>();
builder.Services.AddSingleton<AgentSessionService>();

builder.Services.AddHostedService<AgentTaskWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Agentic AI Orchestrator API",
        Version = "v1",
        Description = "Reference implementation showcasing agentic planning, tool use, and reflection"
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
