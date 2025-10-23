using Module_05_Rate_Limiting.Middleware;
using Module_05_Rate_Limiting.Models;
using Module_05_Rate_Limiting.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Rate Limiting & DDoS Protection API",
        Version = "v1",
        Description = "Comprehensive API rate limiting, quota management, DDoS protection, and throttling"
    });
});

// Memory Cache (for development)
builder.Services.AddMemoryCache();

// Redis Cache (for production - uncomment when Redis is available)
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = builder.Configuration["Redis:ConnectionString"];
//     options.InstanceName = "RateLimit_";
// });

// Configuration
var rateLimitConfig = new RateLimitConfiguration
{
    EnableRateLimiting = true,
    DefaultAlgorithm = RateLimitAlgorithm.SlidingWindow,
    DefaultLimit = 100,
    DefaultPeriodSeconds = 60,
    EnableIpRateLimiting = true,
    EnableClientRateLimiting = true,
    EnableEndpointRateLimiting = true,
    UseDistributedCache = false, // Set to true when using Redis
    WhitelistedIps = new List<string> { "127.0.0.1", "::1" },
    WhitelistedClients = new List<string>()
};

var ddosConfig = new DDoSConfiguration
{
    EnableDDoSProtection = true,
    RequestsPerSecondThreshold = 100,
    RequestsPerMinuteThreshold = 1000,
    AnalysisWindow = TimeSpan.FromMinutes(5),
    AutoBlockSuspiciousIps = true,
    AutoBlockDurationMinutes = 60,
    AutoBlockThreshold = ThreatLevel.High,
    EnableChallengeResponse = false
};

builder.Services.AddSingleton(rateLimitConfig);
builder.Services.AddSingleton(ddosConfig);

// Register services
builder.Services.AddScoped<IRateLimitingService, RateLimitingService>();
builder.Services.AddScoped<IQuotaManagementService, QuotaManagementService>();
builder.Services.AddScoped<IDDoSProtectionService, DDoSProtectionService>();
builder.Services.AddScoped<IThrottlingService, ThrottlingService>();

// Logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders(
                  "X-RateLimit-Limit",
                  "X-RateLimit-Remaining",
                  "X-RateLimit-Reset",
                  "X-Quota-Remaining-Daily",
                  "X-Quota-Remaining-Monthly",
                  "Retry-After");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rate Limiting API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Apply rate limiting middleware
app.UseRateLimiting();

app.UseAuthorization();
app.MapControllers();

// Startup logging
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Rate Limiting API started");
logger.LogInformation("Rate Limiting: {Enabled}, Algorithm: {Algorithm}, Limit: {Limit}/{Period}s",
    rateLimitConfig.EnableRateLimiting,
    rateLimitConfig.DefaultAlgorithm,
    rateLimitConfig.DefaultLimit,
    rateLimitConfig.DefaultPeriodSeconds);
logger.LogInformation("DDoS Protection: {Enabled}, RPS Threshold: {RPS}, RPM Threshold: {RPM}",
    ddosConfig.EnableDDoSProtection,
    ddosConfig.RequestsPerSecondThreshold,
    ddosConfig.RequestsPerMinuteThreshold);
logger.LogInformation("Swagger UI available at: https://localhost:7005");

app.Run();
