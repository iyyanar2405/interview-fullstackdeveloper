var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Authorization token endpoint - accepts POST requests only
app.MapPost("/api/authorize/token", (TokenRequest request) =>
{
    // Simple validation
    if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
    {
        return Results.BadRequest(new { error = "Username and password are required" });
    }

    // In a real application, validate credentials against a database
    // For this example, we'll accept any non-empty credentials
    var token = new TokenResponse
    {
        AccessToken = GenerateToken(request.Username),
        TokenType = "Bearer",
        ExpiresIn = 3600
    };

    return Results.Ok(token);
})
.WithName("GetAuthToken")
.WithOpenApi();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

// Simple token generation (in production, use proper JWT tokens)
static string GenerateToken(string username)
{
    var random = new Random();
    var tokenBytes = new byte[32];
    random.NextBytes(tokenBytes);
    return $"{username}_{Convert.ToBase64String(tokenBytes)}";
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record TokenRequest(string Username, string Password);

record TokenResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = string.Empty;
    public int ExpiresIn { get; init; }
}
