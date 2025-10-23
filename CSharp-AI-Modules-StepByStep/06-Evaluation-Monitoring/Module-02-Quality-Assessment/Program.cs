using Microsoft.OpenApi.Models;
using Module_02_Quality_Assessment.Models;
using Module_02_Quality_Assessment.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions();
builder.Services.Configure<QualityAssessmentOptions>(builder.Configuration.GetSection("QualityAssessment"));

builder.Services.AddSingleton<ResponseEvaluationService>();
builder.Services.AddSingleton<BiasDetectionService>();
builder.Services.AddSingleton<QualityMetricsService>();
builder.Services.AddSingleton<FeedbackAggregationService>();
builder.Services.AddSingleton<EvaluationPipelineService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Quality Assessment API",
        Version = "v1",
        Description = "API for evaluating LLM responses across quality dimensions"
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
