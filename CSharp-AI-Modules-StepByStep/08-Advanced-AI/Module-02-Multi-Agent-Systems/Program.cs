using Microsoft.OpenApi.Models;
using Module_02_Multi_Agent_Systems.HostedServices;
using Module_02_Multi_Agent_Systems.Models;
using Module_02_Multi_Agent_Systems.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOptions();
builder.Services.Configure<MultiAgentOptions>(builder.Configuration.GetSection("MultiAgent"));

builder.Services.AddSingleton<TeamRegistryService>();
builder.Services.AddSingleton<ChannelRegistryService>();
builder.Services.AddSingleton<PlaybookLibraryService>();
builder.Services.AddSingleton<MessageRouterService>();
builder.Services.AddSingleton<TaskBoardService>();
builder.Services.AddSingleton<TaskDispatchQueueService>();
builder.Services.AddSingleton<CoordinationEngineService>();

builder.Services.AddHostedService<CoordinationWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Multi-Agent Coordination API",
        Version = "v1",
        Description = "Reference implementation for multi-agent teaming, coordination, and consensus"
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
