using AI.PromptEngineering.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Prompt Engineering API", Version = "v1" });
});

// Add Prompt Engineering services
builder.Services.AddPromptEngineering(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => new
{
    Service = "Prompt Engineering API",
    Version = "1.0.0",
    Features = new[] { "Template Management", "Few-Shot Learning", "Chain-of-Thought", "Prompt Optimization", "A/B Testing" },
    Endpoints = new
    {
        Templates = "/api/prompt/templates",
        FewShot = "/api/prompt/few-shot",
        ChainOfThought = "/api/prompt/chain-of-thought",
        Optimize = "/api/prompt/optimize",
        ABTest = "/api/prompt/ab-test"
    }
});

app.Run();
