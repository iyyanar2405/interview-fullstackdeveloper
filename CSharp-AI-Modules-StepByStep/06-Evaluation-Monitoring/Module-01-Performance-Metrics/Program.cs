using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using Module_01_Performance_Metrics.HostedServices;
using Module_01_Performance_Metrics.Middleware;
using Module_01_Performance_Metrics.Models;
using Module_01_Performance_Metrics.Services;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions();
builder.Services.Configure<CostSettings>(builder.Configuration.GetSection("CostSettings"));
builder.Services.Configure<SLAOptions>(builder.Configuration.GetSection("SLA"));
builder.Services.Configure<ResourceSamplingOptions>(builder.Configuration.GetSection("ResourceSampling"));

builder.Services.AddSingleton<RequestMetricsService>();
builder.Services.AddSingleton<ResourceUtilizationService>(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ResourceSamplingOptions>>().Value;
    return new ResourceUtilizationService(options);
});
builder.Services.AddSingleton<CostAnalysisService>();
builder.Services.AddSingleton<SLAMonitoringService>();
builder.Services.AddSingleton<MetricsExportService>();

builder.Services.AddHostedService<ResourceSamplingHostedService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Performance Metrics API",
        Version = "v1",
        Description = "Performance monitoring endpoints for AI workloads"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseMiddleware<RequestMetricsMiddleware>();
app.UseRouting();
app.UseHttpMetrics();
app.UseAuthorization();

app.MapControllers();
app.MapMetrics();

app.MapHealthChecks("/health");

app.Run();
