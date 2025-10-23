using Microsoft.OpenApi.Models;
using Module_04_Scaling_Strategies.HostedServices;
using Module_04_Scaling_Strategies.Models;
using Module_04_Scaling_Strategies.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions();
builder.Services.Configure<ScalingOptions>(builder.Configuration.GetSection("Scaling"));

builder.Services.AddSingleton<ScalingProfileService>();
builder.Services.AddSingleton<CapacityForecastService>();
builder.Services.AddSingleton<CostProjectionService>();
builder.Services.AddSingleton<ScalingPolicyBuilderService>();
builder.Services.AddSingleton<ScalingRecommendationService>();
builder.Services.AddSingleton<ScalingSimulationService>();
builder.Services.AddSingleton<ScalingQueueService>();
builder.Services.AddSingleton<ScalingHistoryService>();

builder.Services.AddHostedService<ScalingSimulationWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Scaling Strategies API",
        Version = "v1",
        Description = "Reference implementation for scaling and auto-scaling simulations"
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
