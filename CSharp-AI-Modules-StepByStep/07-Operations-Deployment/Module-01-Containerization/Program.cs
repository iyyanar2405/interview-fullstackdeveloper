using Microsoft.OpenApi.Models;
using Module_01_Containerization.HostedServices;
using Module_01_Containerization.Models;
using Module_01_Containerization.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions();
builder.Services.Configure<ContainerizationOptions>(builder.Configuration.GetSection("Containerization"));

builder.Services.AddSingleton<DockerfileTemplateService>();
builder.Services.AddSingleton<DockerfileGeneratorService>();
builder.Services.AddSingleton<DockerComposeService>();
builder.Services.AddSingleton<ImageOptimizationService>();
builder.Services.AddSingleton<ContainerBuildPlannerService>();
builder.Services.AddSingleton<ContainerBuildQueueService>();
builder.Services.AddSingleton<ContainerBuildHistoryService>();

builder.Services.AddHostedService<ContainerBuildWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Containerization API",
        Version = "v1",
        Description = "Reference implementation for container build automation"
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
