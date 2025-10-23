using Microsoft.OpenApi.Models;
using Module_06_Production_Monitoring.HostedServices;
using Module_06_Production_Monitoring.Models;
using Module_06_Production_Monitoring.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions();
builder.Services.Configure<MonitoringOptions>(builder.Configuration.GetSection("Monitoring"));

builder.Services.AddSingleton<DashboardCatalogService>();
builder.Services.AddSingleton<TelemetryStoreService>();
builder.Services.AddSingleton<SloEvaluationService>();
builder.Services.AddSingleton<AlertingService>();
builder.Services.AddSingleton<IncidentService>();
builder.Services.AddSingleton<MonitoringReportService>();

builder.Services.AddHostedService<MonitoringSimulationWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Production Monitoring API",
        Version = "v1",
        Description = "Reference implementation for production telemetry, alerting, and incident response"
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
