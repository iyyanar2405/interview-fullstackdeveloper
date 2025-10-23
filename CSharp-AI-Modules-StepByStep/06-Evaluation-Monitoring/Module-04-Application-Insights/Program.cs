using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.OpenApi.Models;
using Module_04_Application_Insights.Services;
using Module_04_Application_Insights.Telemetry;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/appinsights-module-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.EnableAdaptiveSampling = true;
    options.EnableDebugLogger = builder.Environment.IsDevelopment();
});

builder.Services.Configure<ApplicationInsightsServiceOptions>(options =>
{
    options.EnableRequestTrackingTelemetryModule = true;
    options.EnableEventCounterCollectionModule = true;
    options.EnableDependencyTrackingTelemetryModule = true;
    options.EnablePerformanceCounterCollectionModule = true;
});

builder.Services.AddSingleton<ITelemetryInitializer, CloudRoleTelemetryInitializer>();
builder.Services.AddApplicationInsightsTelemetryProcessor<TelemetryFilterProcessor>();

builder.Services.AddSingleton<TelemetryService>();
builder.Services.AddHostedService<TelemetryHeartbeatService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Application Insights API",
        Version = "v1",
        Description = "Demonstrates custom telemetry with Azure Application Insights"
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

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
