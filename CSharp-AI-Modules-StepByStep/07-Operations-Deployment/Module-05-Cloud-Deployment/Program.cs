using Microsoft.OpenApi.Models;
using Module_05_Cloud_Deployment.HostedServices;
using Module_05_Cloud_Deployment.Models;
using Module_05_Cloud_Deployment.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions();
builder.Services.Configure<CloudDeploymentOptions>(builder.Configuration.GetSection("CloudDeployment"));

builder.Services.AddSingleton<CloudBlueprintService>();
builder.Services.AddSingleton<ResourceManifestService>();
builder.Services.AddSingleton<ReleaseRunbookService>();
builder.Services.AddSingleton<GuardrailEvaluationService>();
builder.Services.AddSingleton<IntegrationComposerService>();
builder.Services.AddSingleton<TrafficStrategyService>();
builder.Services.AddSingleton<DeploymentPlanService>();
builder.Services.AddSingleton<DeploymentQueueService>();
builder.Services.AddSingleton<DeploymentHistoryService>();
builder.Services.AddSingleton<DeploymentSimulationService>();

builder.Services.AddHostedService<DeploymentWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cloud Deployment API",
        Version = "v1",
        Description = "Reference implementation for multi-cloud deployment orchestration"
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
