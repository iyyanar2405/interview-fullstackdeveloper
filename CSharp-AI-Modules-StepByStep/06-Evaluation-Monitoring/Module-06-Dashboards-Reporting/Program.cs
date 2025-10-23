using Microsoft.OpenApi.Models;
using Module_06_Dashboards_Reporting.HostedServices;
using Module_06_Dashboards_Reporting.Models;
using Module_06_Dashboards_Reporting.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions();
builder.Services.Configure<DashboardOptions>(builder.Configuration.GetSection("Dashboard"));
builder.Services.Configure<ReportingOptions>(builder.Configuration.GetSection("Reporting"));
builder.Services.Configure<MetricSimulationOptions>(builder.Configuration.GetSection("MetricsSimulation"));

builder.Services.AddSingleton<DashboardDefinitionService>();
builder.Services.AddSingleton<MetricArchiveService>();
builder.Services.AddSingleton<DashboardSnapshotService>();
builder.Services.AddSingleton<DashboardComposerService>();
builder.Services.AddSingleton<ReportGenerationService>();
builder.Services.AddSingleton<ExecutiveSummaryService>();

builder.Services.AddHostedService<MetricSimulationHostedService>();
builder.Services.AddHostedService<DashboardSnapshotHostedService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Dashboards & Reporting API",
		Version = "v1",
		Description = "Dynamic dashboards, reporting, and executive summaries for AI workloads"
	});
});

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
