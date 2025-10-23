using Microsoft.OpenApi.Models;
using Module_03_Infrastructure_as_Code.HostedServices;
using Module_03_Infrastructure_as_Code.Models;
using Module_03_Infrastructure_as_Code.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions();
builder.Services.Configure<InfrastructureOptions>(builder.Configuration.GetSection("Infrastructure"));

builder.Services.AddSingleton<InfrastructureTemplateService>();
builder.Services.AddSingleton<TerraformManifestService>();
builder.Services.AddSingleton<ArmTemplateService>();
builder.Services.AddSingleton<BicepTemplateService>();
builder.Services.AddSingleton<ComplianceInsightsService>();
builder.Services.AddSingleton<DriftDetectionService>();
builder.Services.AddSingleton<InfrastructurePlanService>();
builder.Services.AddSingleton<ProvisioningQueueService>();
builder.Services.AddSingleton<ProvisioningHistoryService>();

builder.Services.AddHostedService<ProvisioningWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Infrastructure as Code API",
        Version = "v1",
        Description = "Reference implementation for IaC planning and provisioning"
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
