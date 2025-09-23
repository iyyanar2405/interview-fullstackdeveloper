# Comprehensive .NET Core Interview Questions & Answers

## Table of Contents

1. [.NET Core Fundamentals](#net-core-fundamentals)
2. [Entity Framework Core](#entity-framework-core)
3. [Web API Development](#web-api-development)
4. [Authentication & Authorization](#authentication--authorization)
5. [LINQ](#linq)
6. [GraphQL](#graphql)
7. [Microservices](#microservices)
8. [Swagger & API Documentation](#swagger--api-documentation)
9. [Design Patterns & OOP](#design-patterns--oop)
10. [Performance & Architecture](#performance--architecture)

---

## .NET Core Fundamentals

### Beginner Level

#### Q1: What is .NET Core and how does it differ from .NET Framework?
**Answer:**
.NET Core is a cross-platform, open-source framework for building modern applications. It's designed to be modular, lightweight, and cloud-ready.

**Key Differences:**

| Feature | .NET Framework | .NET Core |
|---------|----------------|-----------|
| Platform | Windows only | Cross-platform (Windows, Linux, macOS) |
| Open Source | No | Yes |
| Deployment | System-wide | Side-by-side |
| Performance | Good | Better |
| Cloud Support | Limited | Optimized |

**Example:**
```csharp
// Program.cs in .NET Core 6+ (Top-level statements)
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure custom services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Minimal API example
app.MapGet("/", () => "Hello World!");
app.MapGet("/users/{id:int}", async (int id, IUserService userService) =>
{
    var user = await userService.GetByIdAsync(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.Run();

// Models
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// Service Interface
public interface IUserService
{
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User?> UpdateAsync(int id, User user);
    Task<bool> DeleteAsync(int id);
}

// Service Implementation
public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Users.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateAsync(int id, User user)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null) return null;

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
        
        await _context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}

// DbContext
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}
```

#### Q2: What is Dependency Injection in .NET Core and how do you implement it?
**Answer:**
Dependency Injection (DI) is a design pattern that implements Inversion of Control (IoC) for managing dependencies. .NET Core has built-in DI container support.

**Service Lifetimes:**
- **Transient**: Created each time they're requested
- **Scoped**: Created once per request
- **Singleton**: Created once for the application lifetime

**Example:**
```csharp
// Startup.cs or Program.cs
public void ConfigureServices(IServiceCollection services)
{
    // Transient - new instance every time
    services.AddTransient<IEmailService, EmailService>();
    
    // Scoped - one instance per request
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IUserService, UserService>();
    
    // Singleton - one instance for application lifetime
    services.AddSingleton<ICacheService, MemoryCacheService>();
    
    // Configuration
    services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
    
    // HttpClient
    services.AddHttpClient<IApiService, ApiService>(client =>
    {
        client.BaseAddress = new Uri("https://api.example.com/");
        client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
    });
}

// Service Interfaces
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
}

public interface ICacheService
{
    T Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan expiration);
    void Remove(string key);
}

// Service Implementations
public class EmailService : IEmailService
{
    private readonly IOptions<EmailSettings> _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Sending email to {To} with subject {Subject}", to, subject);
        
        // Email sending logic here
        await Task.Delay(100); // Simulate async operation
        
        _logger.LogInformation("Email sent successfully");
    }
}

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        _logger.LogDebug("Retrieving user with ID {UserId}", id);
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created user with ID {UserId}", user.Id);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated user with ID {UserId}", user.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted user with ID {UserId}", id);
        }
    }
}

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;

    public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public T Get<T>(string key)
    {
        var value = _cache.Get<T>(key);
        _logger.LogDebug("Cache {Operation} for key {Key}", value != null ? "hit" : "miss", key);
        return value;
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        _cache.Set(key, value, expiration);
        _logger.LogDebug("Cache set for key {Key} with expiration {Expiration}", key, expiration);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _logger.LogDebug("Cache removed for key {Key}", key);
    }
}

// Configuration classes
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
}

// Controller using DI
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly ICacheService _cacheService;

    public UsersController(
        IUserService userService, 
        IEmailService emailService, 
        ICacheService cacheService)
    {
        _userService = userService;
        _emailService = emailService;
        _cacheService = cacheService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        // Try cache first
        var cacheKey = $"user_{id}";
        var cachedUser = _cacheService.Get<User>(cacheKey);
        
        if (cachedUser != null)
        {
            return Ok(cachedUser);
        }

        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Cache for 5 minutes
        _cacheService.Set(cacheKey, user, TimeSpan.FromMinutes(5));
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(CreateUserRequest request)
    {
        var user = new User
        {
            Name = request.Name,
            Email = request.Email
        };

        var createdUser = await _userService.CreateAsync(user);

        // Send welcome email
        await _emailService.SendEmailAsync(
            createdUser.Email,
            "Welcome!",
            $"Welcome {createdUser.Name}!");

        return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
    }
}

public class CreateUserRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
```

#### Q3: What is Middleware in .NET Core and how do you create custom middleware?
**Answer:**
Middleware are components that are assembled into an application pipeline to handle requests and responses. Each component can perform operations before and after the next component in the pipeline.

**Example:**
```csharp
// Custom Middleware Classes
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();
        
        // Log request
        _logger.LogInformation(
            "Request {RequestId}: {Method} {Path} started",
            requestId,
            context.Request.Method,
            context.Request.Path);

        try
        {
            // Call the next middleware
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            // Log response
            _logger.LogInformation(
                "Request {RequestId}: {Method} {Path} completed with {StatusCode} in {ElapsedMs}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case NotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = exception.Message;
                break;
            case ValidationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = exception.Message;
                break;
            case UnauthorizedException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "Unauthorized access";
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An error occurred while processing your request";
                break;
        }

        context.Response.StatusCode = response.StatusCode;
        
        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;

    public RateLimitingMiddleware(
        RequestDelegate next,
        IMemoryCache cache,
        IOptions<RateLimitOptions> options,
        ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var key = $"rate_limit_{clientId}";
        
        var requestCount = _cache.Get<int>(key);
        
        if (requestCount >= _options.MaxRequests)
        {
            _logger.LogWarning("Rate limit exceeded for client {ClientId}", clientId);
            
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.Headers.Add("Retry-After", _options.WindowSize.TotalSeconds.ToString());
            
            await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
            return;
        }

        // Increment request count
        _cache.Set(key, requestCount + 1, _options.WindowSize);
        
        await _next(context);
    }

    private static string GetClientIdentifier(HttpContext context)
    {
        // Use IP address as client identifier
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

// Configuration classes
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
}

public class RateLimitOptions
{
    public int MaxRequests { get; set; } = 100;
    public TimeSpan WindowSize { get; set; } = TimeSpan.FromMinutes(1);
}

// Custom Exceptions
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}

// Extension methods for middleware registration
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }

    public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitingMiddleware>();
    }
}

// Program.cs - Middleware configuration
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection("RateLimit"));

var app = builder.Build();

// Order matters! Middleware executes in the order they are added
app.UseCustomExceptionHandling(); // Should be first to catch all exceptions
app.UseRequestLogging();
app.UseRateLimiting();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Inline middleware example
app.Use(async (context, next) =>
{
    // Before next middleware
    context.Response.Headers.Add("X-Custom-Header", "Custom Value");
    
    await next(); // Call next middleware
    
    // After next middleware
    Console.WriteLine($"Response status: {context.Response.StatusCode}");
});

// Terminal middleware example
app.Run(async context =>
{
    await context.Response.WriteAsync("Hello from terminal middleware!");
});

app.Run();

// appsettings.json
/*
{
  "RateLimit": {
    "MaxRequests": 100,
    "WindowSize": "00:01:00"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
*/
```

### Intermediate Level

#### Q4: How do you implement Configuration in .NET Core?
**Answer:**
.NET Core provides a flexible configuration system that can read from multiple sources like appsettings.json, environment variables, command line arguments, etc.

**Example:**
```csharp
// Configuration Models
public class DatabaseSettings
{
    public const string SectionName = "Database";
    
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 30;
    public bool EnableRetryOnFailure { get; set; } = true;
    public int MaxRetryCount { get; set; } = 3;
}

public class JwtSettings
{
    public const string SectionName = "Jwt";
    
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromHours(1);
}

public class EmailSettings
{
    public const string SectionName = "Email";
    
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}

public class FeatureFlags
{
    public const string SectionName = "FeatureFlags";
    
    public bool EnableNewUserInterface { get; set; }
    public bool EnableAdvancedLogging { get; set; }
    public bool EnableCaching { get; set; } = true;
}

// Program.cs - Configuration setup
var builder = WebApplication.CreateBuilder(args);

// Add configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .AddUserSecrets<Program>(); // For development only

// Configure strongly-typed settings
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection(DatabaseSettings.SectionName));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SectionName));

builder.Services.Configure<FeatureFlags>(
    builder.Configuration.GetSection(FeatureFlags.SectionName));

// Validate configuration
builder.Services.AddOptions<DatabaseSettings>()
    .Bind(builder.Configuration.GetSection(DatabaseSettings.SectionName))
    .ValidateDataAnnotations()
    .Validate(settings => !string.IsNullOrEmpty(settings.ConnectionString), 
              "Connection string is required");

// Register services that use configuration
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Configuration service implementation
public interface IConfigurationService
{
    T GetSection<T>(string sectionName) where T : new();
    string GetConnectionString(string name);
    bool IsFeatureEnabled(string featureName);
}

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;

    public ConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public T GetSection<T>(string sectionName) where T : new()
    {
        var section = new T();
        _configuration.GetSection(sectionName).Bind(section);
        return section;
    }

    public string GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name) ?? string.Empty;
    }

    public bool IsFeatureEnabled(string featureName)
    {
        return _configuration.GetValue<bool>($"FeatureFlags:{featureName}");
    }
}

// Services using configuration
public class DatabaseService : IDatabaseService
{
    private readonly DatabaseSettings _settings;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(IOptions<DatabaseSettings> settings, ILogger<DatabaseService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> TestConnectionAsync()
    {
        using var connection = new SqlConnection(_settings.ConnectionString);
        
        try
        {
            using var command = new SqlCommand("SELECT 1", connection)
            {
                CommandTimeout = _settings.CommandTimeout
            };
            
            await connection.OpenAsync();
            await command.ExecuteScalarAsync();
            
            _logger.LogInformation("Database connection successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection failed");
            return false;
        }
    }
}

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IOptions<JwtSettings> jwtSettings, ILogger<TokenService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        
        _logger.LogInformation("Token generated for user {UserId}", user.Id);
        
        return tokenString;
    }
}

// Controller using configuration
[ApiController]
[Route("api/[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IOptions<FeatureFlags> _featureFlags;
    private readonly IOptionsSnapshot<DatabaseSettings> _databaseSettings; // For reloadable config

    public ConfigurationController(
        IConfiguration configuration,
        IOptions<FeatureFlags> featureFlags,
        IOptionsSnapshot<DatabaseSettings> databaseSettings)
    {
        _configuration = configuration;
        _featureFlags = featureFlags;
        _databaseSettings = databaseSettings;
    }

    [HttpGet("feature/{featureName}")]
    public IActionResult IsFeatureEnabled(string featureName)
    {
        var isEnabled = _configuration.GetValue<bool>($"FeatureFlags:{featureName}");
        return Ok(new { FeatureName = featureName, IsEnabled = isEnabled });
    }

    [HttpGet("settings")]
    public IActionResult GetSettings()
    {
        var settings = new
        {
            Environment = _configuration["ASPNETCORE_ENVIRONMENT"],
            DatabaseTimeout = _databaseSettings.Value.CommandTimeout,
            Features = _featureFlags.Value,
            CustomSetting = _configuration["CustomSettings:SomeValue"]
        };

        return Ok(settings);
    }
}

// appsettings.json
/*
{
  "Database": {
    "ConnectionString": "Server=localhost;Database=MyApp;Integrated Security=true;",
    "CommandTimeout": 30,
    "EnableRetryOnFailure": true,
    "MaxRetryCount": 3
  },
  "Jwt": {
    "SecretKey": "your-super-secret-key-here",
    "Issuer": "MyApp",
    "Audience": "MyApp-Users",
    "TokenLifetime": "01:00:00"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-password",
    "EnableSsl": true,
    "FromAddress": "noreply@myapp.com",
    "FromName": "My App"
  },
  "FeatureFlags": {
    "EnableNewUserInterface": true,
    "EnableAdvancedLogging": false,
    "EnableCaching": true
  },
  "CustomSettings": {
    "SomeValue": "configured-value"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
*/

// appsettings.Development.json
/*
{
  "Database": {
    "ConnectionString": "Server=localhost;Database=MyApp_Dev;Integrated Security=true;"
  },
  "FeatureFlags": {
    "EnableAdvancedLogging": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
*/

// Environment Variables Example
// DATABASE__CONNECTIONSTRING=Server=prod-server;Database=MyApp_Prod;...
// JWT__SECRETKEY=production-secret-key
// FEATUREFLAGS__ENABLENEWUSERINTERFACE=false
```

---

## Entity Framework Core

### Beginner Level

#### Q5: What is Entity Framework Core and how do you set it up?
**Answer:**
Entity Framework Core (EF Core) is a lightweight, extensible, open-source object-relational mapping (ORM) framework for .NET. It enables developers to work with databases using .NET objects.

**Example:**
```csharp
// Models
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public UserProfile? Profile { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    
    // Foreign key
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    // Navigation properties
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    // Foreign key
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}

public class UserProfile
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    
    // Foreign key (one-to-one)
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}

public enum OrderStatus
{
    Pending = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}

// DbContext
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasConversion<int>();
            
            // Relationship
            entity.HasOne(e => e.User)
                  .WithMany(e => e.Orders)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            
            // Relationship
            entity.HasOne(e => e.Order)
                  .WithMany(e => e.OrderItems)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // UserProfile configuration (one-to-one)
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            
            // One-to-one relationship
            entity.HasOne(e => e.User)
                  .WithOne(e => e.Profile)
                  .HasForeignKey<UserProfile>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                Name = "Jane Smith",
                Email = "jane@example.com",
                CreatedAt = DateTime.UtcNow
            }
        );
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-set UpdatedAt for modified entities
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is User && e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is User user)
            {
                user.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

// Program.cs - EF Core setup
var builder = WebApplication.CreateBuilder(args);

// Add EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    
    // Enable sensitive data logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Repository pattern
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var app = builder.Build();

// Auto-migrate on startup (development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}

// Repository interfaces
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task<bool> DeleteAsync(int id);
}

// Repository implementations
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Profile)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }
}

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        _context.Entry(order).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }
}
```

#### Q6: How do you handle Migrations in Entity Framework Core?
**Answer:**
Migrations allow you to evolve your database schema over time. EF Core can generate migrations based on changes to your model and apply them to the database.

**Example:**
```csharp
// Initial Migration Commands
// dotnet ef migrations add InitialCreate
// dotnet ef database update

// Adding a new entity - Product
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

// Updated DbContext
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Existing configurations...

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => new { e.CategoryId, e.IsActive });
            
            // Relationship with Category
            entity.HasOne(e => e.Category)
                  .WithMany(e => e.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Update OrderItem to reference Product
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            
            // Add ProductId foreign key
            entity.Property(e => e.ProductId).IsRequired(false); // Nullable for backward compatibility
            
            entity.HasOne(e => e.Order)
                  .WithMany(e => e.OrderItems)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Product)
                  .WithMany(e => e.OrderItems)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories" },
            new Category { Id = 2, Name = "Books", Description = "Books and educational materials" },
            new Category { Id = 3, Name = "Clothing", Description = "Apparel and fashion items" }
        );

        // Products
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = 999.99m,
                Stock = 50,
                CategoryId = 1,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "C# Programming Book",
                Description = "Learn C# programming",
                Price = 49.99m,
                Stock = 100,
                CategoryId = 2,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}

// Updated OrderItem model
public class OrderItem
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty; // Keep for legacy data
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    // Foreign keys
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    
    public int? ProductId { get; set; } // Nullable for backward compatibility
    public Product? Product { get; set; }
}

// Migration Management Service
public interface IMigrationService
{
    Task<bool> HasPendingMigrationsAsync();
    Task<IEnumerable<string>> GetPendingMigrationsAsync();
    Task ApplyMigrationsAsync();
    Task<bool> DatabaseExistsAsync();
    Task CreateDatabaseAsync();
}

public class MigrationService : IMigrationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MigrationService> _logger;

    public MigrationService(ApplicationDbContext context, ILogger<MigrationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> HasPendingMigrationsAsync()
    {
        var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
        return pendingMigrations.Any();
    }

    public async Task<IEnumerable<string>> GetPendingMigrationsAsync()
    {
        return await _context.Database.GetPendingMigrationsAsync();
    }

    public async Task ApplyMigrationsAsync()
    {
        try
        {
            _logger.LogInformation("Applying database migrations...");
            
            var pendingMigrations = await GetPendingMigrationsAsync();
            
            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Found {Count} pending migrations: {Migrations}", 
                    pendingMigrations.Count(), string.Join(", ", pendingMigrations));
                
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Database migrations applied successfully");
            }
            else
            {
                _logger.LogInformation("No pending migrations found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying database migrations");
            throw;
        }
    }

    public async Task<bool> DatabaseExistsAsync()
    {
        return await _context.Database.CanConnectAsync();
    }

    public async Task CreateDatabaseAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }
}

// Custom Migration Example
/*
dotnet ef migrations add AddProductCatalog

Generated migration file:
*/

public partial class AddProductCatalog : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Stock = table.Column<int>(type: "int", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CategoryId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
                table.ForeignKey(
                    name: "FK_Products_Categories_CategoryId",
                    column: x => x.CategoryId,
                    principalTable: "Categories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.AddColumn<int>(
            name: "ProductId",
            table: "OrderItems",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_OrderItems_ProductId",
            table: "OrderItems",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_Categories_Name",
            table: "Categories",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Products_CategoryId_IsActive",
            table: "Products",
            columns: new[] { "CategoryId", "IsActive" });

        migrationBuilder.CreateIndex(
            name: "IX_Products_Name",
            table: "Products",
            column: "Name");

        migrationBuilder.AddForeignKey(
            name: "FK_OrderItems_Products_ProductId",
            table: "OrderItems",
            column: "ProductId",
            principalTable: "Products",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        // Insert seed data
        migrationBuilder.InsertData(
            table: "Categories",
            columns: new[] { "Id", "Description", "Name" },
            values: new object[,]
            {
                { 1, "Electronic devices and accessories", "Electronics" },
                { 2, "Books and educational materials", "Books" },
                { 3, "Apparel and fashion items", "Clothing" }
            });

        migrationBuilder.InsertData(
            table: "Products",
            columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IsActive", "Name", "Price", "Stock" },
            values: new object[,]
            {
                { 1, 1, new DateTime(2024, 1, 1), "High-performance laptop", true, "Laptop", 999.99m, 50 },
                { 2, 2, new DateTime(2024, 1, 1), "Learn C# programming", true, "C# Programming Book", 49.99m, 100 }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_OrderItems_Products_ProductId", table: "OrderItems");
        migrationBuilder.DropTable(name: "Products");
        migrationBuilder.DropTable(name: "Categories");
        migrationBuilder.DropColumn(name: "ProductId", table: "OrderItems");
    }
}

// Migration Commands:
/*
# Create a new migration
dotnet ef migrations add AddProductCatalog

# Update database
dotnet ef database update

# Update to specific migration
dotnet ef database update AddProductCatalog

# Remove last migration (if not applied)
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script

# Generate SQL for specific migration range
dotnet ef migrations script PreviousMigration AddProductCatalog

# List migrations
dotnet ef migrations list

# Get database info
dotnet ef database drop --force
*/

// Startup Configuration with Migration
public static async Task Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    
    builder.Services.AddScoped<IMigrationService, MigrationService>();
    
    var app = builder.Build();
    
    // Apply migrations on startup
    await ApplyMigrationsAsync(app);
    
    app.Run();
}

private static async Task ApplyMigrationsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationService>();
    
    try
    {
        if (!await migrationService.DatabaseExistsAsync())
        {
            await migrationService.CreateDatabaseAsync();
        }
        
        if (await migrationService.HasPendingMigrationsAsync())
        {
            await migrationService.ApplyMigrationsAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
        throw;
    }
}
```

### Intermediate Level

#### Q7: How do you implement complex queries and relationships in Entity Framework Core?
**Answer:**
EF Core supports various types of relationships and provides multiple ways to query data including LINQ, raw SQL, and stored procedures.

**Example:**
```csharp
// Advanced Repository with Complex Queries
public interface IAdvancedRepository
{
    // Projections and Select
    Task<IEnumerable<UserSummaryDto>> GetUserSummariesAsync();
    Task<OrderReportDto> GetOrderReportAsync(DateTime fromDate, DateTime toDate);
    
    // Grouping and Aggregations
    Task<IEnumerable<CategorySalesDto>> GetCategorySalesAsync();
    Task<decimal> GetTotalRevenueAsync(int? userId = null);
    
    // Complex Joins
    Task<IEnumerable<UserOrderHistoryDto>> GetUserOrderHistoryAsync(int userId);
    
    // Raw SQL
    Task<IEnumerable<TopSellingProductDto>> GetTopSellingProductsAsync(int count);
    
    // Stored Procedures
    Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int year);
    
    // Include and ThenInclude
    Task<User> GetUserWithCompleteDataAsync(int userId);
    
    // Split Queries
    Task<IEnumerable<Order>> GetOrdersWithItemsAsync(int userId);
}

public class AdvancedRepository : IAdvancedRepository
{
    private readonly ApplicationDbContext _context;

    public AdvancedRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserSummaryDto>> GetUserSummariesAsync()
    {
        return await _context.Users
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                OrderCount = u.Orders.Count,
                TotalSpent = u.Orders.Sum(o => o.Amount),
                LastOrderDate = u.Orders.Max(o => (DateTime?)o.OrderDate),
                IsActive = u.Orders.Any(o => o.OrderDate > DateTime.UtcNow.AddMonths(-3))
            })
            .ToListAsync();
    }

    public async Task<OrderReportDto> GetOrderReportAsync(DateTime fromDate, DateTime toDate)
    {
        var orders = await _context.Orders
            .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
            .GroupBy(o => 1) // Group all records
            .Select(g => new OrderReportDto
            {
                TotalOrders = g.Count(),
                TotalRevenue = g.Sum(o => o.Amount),
                AverageOrderValue = g.Average(o => o.Amount),
                PendingOrders = g.Count(o => o.Status == OrderStatus.Pending),
                CompletedOrders = g.Count(o => o.Status == OrderStatus.Delivered)
            })
            .FirstOrDefaultAsync();

        return orders ?? new OrderReportDto();
    }

    public async Task<IEnumerable<CategorySalesDto>> GetCategorySalesAsync()
    {
        return await _context.Categories
            .Select(c => new CategorySalesDto
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                ProductCount = c.Products.Count,
                TotalRevenue = c.Products
                    .SelectMany(p => p.OrderItems)
                    .Sum(oi => oi.Price * oi.Quantity),
                TopSellingProduct = c.Products
                    .OrderByDescending(p => p.OrderItems.Sum(oi => oi.Quantity))
                    .Select(p => p.Name)
                    .FirstOrDefault()
            })
            .Where(c => c.ProductCount > 0)
            .OrderByDescending(c => c.TotalRevenue)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync(int? userId = null)
    {
        var query = _context.Orders.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        return await query.SumAsync(o => o.Amount);
    }

    public async Task<IEnumerable<UserOrderHistoryDto>> GetUserOrderHistoryAsync(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Join(_context.Users, o => o.UserId, u => u.Id, (o, u) => new { Order = o, User = u })
            .GroupJoin(
                _context.OrderItems,
                ou => ou.Order.Id,
                oi => oi.OrderId,
                (ou, items) => new UserOrderHistoryDto
                {
                    OrderId = ou.Order.Id,
                    UserName = ou.User.Name,
                    OrderDate = ou.Order.OrderDate,
                    Status = ou.Order.Status.ToString(),
                    TotalAmount = ou.Order.Amount,
                    ItemCount = items.Count(),
                    Items = items.Select(i => new OrderItemDto
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                })
            .OrderByDescending(h => h.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TopSellingProductDto>> GetTopSellingProductsAsync(int count)
    {
        // Using raw SQL for complex aggregation
        var sql = @"
            SELECT TOP(@count)
                p.Id,
                p.Name,
                SUM(oi.Quantity) as TotalQuantitySold,
                SUM(oi.Price * oi.Quantity) as TotalRevenue,
                COUNT(DISTINCT oi.OrderId) as OrderCount
            FROM Products p
            INNER JOIN OrderItems oi ON p.Id = oi.ProductId
            INNER JOIN Orders o ON oi.OrderId = o.Id
            WHERE o.Status = @status
            GROUP BY p.Id, p.Name
            ORDER BY TotalQuantitySold DESC";

        return await _context.Database
            .SqlQueryRaw<TopSellingProductDto>(sql, 
                new SqlParameter("@count", count),
                new SqlParameter("@status", (int)OrderStatus.Delivered))
            .ToListAsync();
    }

    public async Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int year)
    {
        // Using stored procedure
        var yearParam = new SqlParameter("@Year", year);
        
        return await _context.Database
            .SqlQueryRaw<MonthlyRevenueDto>("EXEC GetMonthlyRevenue @Year", yearParam)
            .ToListAsync();
    }

    public async Task<User> GetUserWithCompleteDataAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Orders.OrderByDescending(o => o.OrderDate))
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync(int userId)
    {
        // Using AsSplitQuery for better performance with multiple includes
        return await _context.Orders
            .AsSplitQuery()
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.User)
                .ThenInclude(u => u.Profile)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
}

// DTOs for complex queries
public class UserSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime? LastOrderDate { get; set; }
    public bool IsActive { get; set; }
}

public class OrderReportDto
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
}

public class CategorySalesDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public string? TopSellingProduct { get; set; }
}

public class UserOrderHistoryDto
{
    public int OrderId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class TopSellingProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public int OrderCount { get; set; }
}

public class MonthlyRevenueDto
{
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}

// Query Performance Optimization
public class OptimizedQueries
{
    private readonly ApplicationDbContext _context;

    public OptimizedQueries(ApplicationDbContext context)
    {
        _context = context;
    }

    // Using compiled queries for frequently used queries
    private static readonly Func<ApplicationDbContext, int, Task<User?>> GetUserByIdCompiled =
        EF.CompileAsyncQuery((ApplicationDbContext context, int id) =>
            context.Users
                .Include(u => u.Profile)
                .FirstOrDefault(u => u.Id == id));

    public async Task<User?> GetUserByIdOptimizedAsync(int id)
    {
        return await GetUserByIdCompiled(_context, id);
    }

    // Pagination with efficient counting
    public async Task<PagedResult<OrderDto>> GetOrdersPagedAsync(int page, int pageSize, string? status = null)
    {
        var query = _context.Orders.AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
        {
            query = query.Where(o => o.Status == orderStatus);
        }

        var totalCount = await query.CountAsync();
        
        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                UserName = o.User.Name,
                Amount = o.Amount,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                ItemCount = o.OrderItems.Count
            })
            .ToListAsync();

        return new PagedResult<OrderDto>
        {
            Data = orders,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    // Batch operations
    public async Task UpdateProductPricesAsync(Dictionary<int, decimal> productPrices)
    {
        // Using ExecuteUpdate for efficient bulk updates (EF Core 7+)
        foreach (var (productId, newPrice) in productPrices)
        {
            await _context.Products
                .Where(p => p.Id == productId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Price, newPrice)
                    .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));
        }
    }

    // No-tracking queries for read-only scenarios
    public async Task<IEnumerable<ProductDto>> GetProductCatalogAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryName = p.Category.Name,
                InStock = p.Stock > 0
            })
            .ToListAsync();
    }
}

// Supporting classes
public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public class OrderDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool InStock { get; set; }
}
```

---

## Web API Development

### Beginner Level

#### Q8: How do you create a RESTful Web API in .NET Core?
**Answer:**
ASP.NET Core Web API is a framework for building HTTP services that can be consumed by various clients. It follows REST principles and supports content negotiation, model binding, and validation.

**Example:**
```csharp
// Models
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> SuccessResult(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

// Request/Response DTOs
public class CreateUserRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? Phone { get; set; }

    [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
    public int? Age { get; set; }
}

public class UpdateUserRequest
{
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? Phone { get; set; }

    [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
    public int? Age { get; set; }
}

public class UserResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int? Age { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// Base Controller with common functionality
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult<ApiResponse<T>> Success<T>(T data, string message = "Success")
    {
        return Ok(ApiResponse<T>.SuccessResult(data, message));
    }

    protected ActionResult<ApiResponse<T>> Error<T>(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        var response = ApiResponse<T>.ErrorResult(message);
        return StatusCode((int)statusCode, response);
    }

    protected ActionResult<ApiResponse<T>> ValidationError<T>(ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        var response = ApiResponse<T>.ErrorResult("Validation failed", errors);
        return BadRequest(response);
    }
}

// Users Controller
[Route("api/[controller]")]
[ApiController]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        IMapper mapper,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all users with optional filtering and pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="search">Search term for name or email</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<UserResponse>>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var users = await _userService.GetUsersAsync(page, pageSize, search);
            var userResponses = _mapper.Map<PagedResult<UserResponse>>(users);

            _logger.LogInformation("Retrieved {Count} users for page {Page}", 
                userResponses.Data.Count, page);

            return Success(userResponses, "Users retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return Error<PagedResult<UserResponse>>("An error occurred while retrieving users", 
                HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    /// Get a specific user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return Error<UserResponse>("User not found", HttpStatusCode.NotFound);
            }

            var userResponse = _mapper.Map<UserResponse>(user);
            return Success(userResponse, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            return Error<UserResponse>("An error occurred while retrieving the user", 
                HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request">User creation data</param>
    /// <returns>Created user details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationError<UserResponse>(ModelState);
            }

            // Check if email already exists
            var existingUser = await _userService.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return Error<UserResponse>("A user with this email already exists", HttpStatusCode.Conflict);
            }

            var user = _mapper.Map<User>(request);
            var createdUser = await _userService.CreateAsync(user);
            var userResponse = _mapper.Map<UserResponse>(createdUser);

            _logger.LogInformation("Created user with ID {UserId}", createdUser.Id);

            return CreatedAtAction(
                nameof(GetUser),
                new { id = createdUser.Id },
                ApiResponse<UserResponse>.SuccessResult(userResponse, "User created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return Error<UserResponse>("An error occurred while creating the user", 
                HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">User update data</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateUser(
        int id, 
        [FromBody] UpdateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationError<UserResponse>(ModelState);
            }

            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return Error<UserResponse>("User not found", HttpStatusCode.NotFound);
            }

            // Check email uniqueness if email is being updated
            if (!string.IsNullOrEmpty(request.Email) && request.Email != existingUser.Email)
            {
                var emailExists = await _userService.GetByEmailAsync(request.Email);
                if (emailExists != null)
                {
                    return Error<UserResponse>("A user with this email already exists", HttpStatusCode.Conflict);
                }
            }

            // Apply partial updates
            if (!string.IsNullOrEmpty(request.Name))
                existingUser.Name = request.Name;
            
            if (!string.IsNullOrEmpty(request.Email))
                existingUser.Email = request.Email;

            if (!string.IsNullOrEmpty(request.Phone))
                existingUser.Phone = request.Phone;

            if (request.Age.HasValue)
                existingUser.Age = request.Age.Value;

            var updatedUser = await _userService.UpdateAsync(existingUser);
            var userResponse = _mapper.Map<UserResponse>(updatedUser);

            _logger.LogInformation("Updated user with ID {UserId}", id);

            return Success(userResponse, "User updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", id);
            return Error<UserResponse>("An error occurred while updating the user", 
                HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    /// Partially update a user using JSON Patch
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="patchDoc">JSON patch document</param>
    /// <returns>Updated user details</returns>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> PatchUser(
        int id,
        [FromBody] JsonPatchDocument<UpdateUserRequest> patchDoc)
    {
        try
        {
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return Error<UserResponse>("User not found", HttpStatusCode.NotFound);
            }

            var userToPatch = _mapper.Map<UpdateUserRequest>(existingUser);
            patchDoc.ApplyTo(userToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return ValidationError<UserResponse>(ModelState);
            }

            _mapper.Map(userToPatch, existingUser);
            var updatedUser = await _userService.UpdateAsync(existingUser);
            var userResponse = _mapper.Map<UserResponse>(updatedUser);

            _logger.LogInformation("Patched user with ID {UserId}", id);

            return Success(userResponse, "User updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error patching user with ID {UserId}", id);
            return Error<UserResponse>("An error occurred while updating the user", 
                HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Deletion confirmation</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
    {
        try
        {
            var exists = await _userService.ExistsAsync(id);
            if (!exists)
            {
                return Error<object>("User not found", HttpStatusCode.NotFound);
            }

            var deleted = await _userService.DeleteAsync(id);
            if (!deleted)
            {
                return Error<object>("Failed to delete user", HttpStatusCode.InternalServerError);
            }

            _logger.LogInformation("Deleted user with ID {UserId}", id);

            return Success<object>(null, "User deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            return Error<object>("An error occurred while deleting the user", 
                HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    /// Get user statistics
    /// </summary>
    /// <returns>User statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<UserStatistics>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserStatistics>>> GetUserStatistics()
    {
        try
        {
            var stats = await _userService.GetStatisticsAsync();
            return Success(stats, "User statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user statistics");
            return Error<UserStatistics>("An error occurred while retrieving statistics", 
                HttpStatusCode.InternalServerError);
        }
    }
}

// AutoMapper Profile
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateUserRequest, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<User, UserResponse>();

        CreateMap<User, UpdateUserRequest>();

        CreateMap<PagedResult<User>, PagedResult<UserResponse>>();
    }
}

// Program.cs Configuration
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddNewtonsoftJson(); // For JSON Patch support

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader("version"),
        new HeaderApiVersionReader("X-Version")
    );
});

// Add versioned API explorer
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(UserMappingProfile));

// Add custom services
builder.Services.AddScoped<IUserService, UserService>();

// Add response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors("Development");
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseCors();
}

app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Supporting Models
public class UserStatistics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
    public double AverageAge { get; set; }
    public string MostCommonDomain { get; set; } = string.Empty;
}

// Updated User model with additional properties
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int? Age { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public UserProfile? Profile { get; set; }
}
```

#### Q9: How do you implement Model Binding and Validation in Web API?
**Answer:**
Model binding automatically maps HTTP request data to action parameters, while model validation ensures data integrity using data annotations and custom validators.

**Example:**
```csharp
// Custom Validation Attributes
public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string email || string.IsNullOrEmpty(email))
        {
            return ValidationResult.Success; // Let [Required] handle null/empty
        }

        var userService = validationContext.GetService<IUserService>();
        if (userService == null)
        {
            return new ValidationResult("Unable to validate email uniqueness");
        }

        // Note: In real scenarios, consider async validation
        var existingUser = userService.GetByEmailAsync(email).GetAwaiter().GetResult();
        
        return existingUser != null 
            ? new ValidationResult("Email address is already in use")
            : ValidationResult.Success;
    }
}

public class MinimumAgeAttribute : ValidationAttribute
{
    private readonly int _minimumAge;

    public MinimumAgeAttribute(int minimumAge)
    {
        _minimumAge = minimumAge;
        ErrorMessage = $"Must be at least {minimumAge} years old";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateOfBirth)
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                age--;

            return age >= _minimumAge
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}

public class StrongPasswordAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string password || string.IsNullOrEmpty(password))
            return false;

        return password.Length >= 8 &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit) &&
               password.Any(ch => !char.IsLetterOrDigit(ch));
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be at least 8 characters long and contain uppercase, lowercase, numeric, and special characters.";
    }
}

// Complex Request Models with Validation
public class CreateUserRequest : IValidatableObject
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can only contain letters and spaces")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can only contain letters and spaces")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    [UniqueEmail]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format")]
    [RegularExpression(@"^\+?[\d\s\-\(\)]+$", ErrorMessage = "Phone number contains invalid characters")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Date of birth is required")]
    [MinimumAge(18)]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StrongPassword]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    [EnumDataType(typeof(Gender), ErrorMessage = "Invalid gender value")]
    public Gender Gender { get; set; }

    [Range(0, 5, ErrorMessage = "Experience years must be between 0 and 5")]
    public int? ExperienceYears { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    public string? Website { get; set; }

    public List<string> Skills { get; set; } = new();

    [Required(ErrorMessage = "Terms acceptance is required")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions")]
    public bool AcceptTerms { get; set; }

    // Custom validation logic
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // Custom business logic validation
        if (DateOfBirth > DateTime.Today.AddYears(-16))
        {
            results.Add(new ValidationResult(
                "Users must be at least 16 years old",
                new[] { nameof(DateOfBirth) }));
        }

        if (Skills.Count > 10)
        {
            results.Add(new ValidationResult(
                "Maximum 10 skills allowed",
                new[] { nameof(Skills) }));
        }

        if (Skills.Any(skill => string.IsNullOrWhiteSpace(skill)))
        {
            results.Add(new ValidationResult(
                "Skills cannot be empty",
                new[] { nameof(Skills) }));
        }

        // Cross-field validation
        if (!string.IsNullOrEmpty(Website) && Website.Contains(Email.Split('@')[0]))
        {
            results.Add(new ValidationResult(
                "Website URL should not contain email username",
                new[] { nameof(Website) }));
        }

        return results;
    }
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3,
    PreferNotToSay = 4
}

// Model Binder for Complex Types
public class CustomDateTimeModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelType != typeof(DateTime) && bindingContext.ModelType != typeof(DateTime?))
        {
            return Task.CompletedTask;
        }

        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            if (bindingContext.ModelType == typeof(DateTime?))
            {
                bindingContext.Result = ModelBindingResult.Successful(null);
            }
            return Task.CompletedTask;
        }

        // Try to parse different date formats
        var formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-ddTHH:mm:ss" };
        
        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                bindingContext.Result = ModelBindingResult.Successful(date);
                return Task.CompletedTask;
            }
        }

        bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid date format");
        bindingContext.Result = ModelBindingResult.Failed();
        
        return Task.CompletedTask;
    }
}

public class CustomDateTimeModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?))
        {
            return new CustomDateTimeModelBinder();
        }

        return null;
    }
}

// Advanced Controller with Comprehensive Validation
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserRequest> _createUserValidator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        IValidator<CreateUserRequest> createUserValidator,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _createUserValidator = createUserValidator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new user with comprehensive validation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateUser(
        [FromBody] CreateUserRequest request)
    {
        try
        {
            // Manual model validation
            if (!ModelState.IsValid)
            {
                return ValidationError<UserResponse>(ModelState);
            }

            // FluentValidation (alternative approach)
            var validationResult = await _createUserValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<UserResponse>.ErrorResult("Validation failed", errors));
            }

            var user = await _userService.CreateUserAsync(request);
            var response = _mapper.Map<UserResponse>(user);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, 
                ApiResponse<UserResponse>.SuccessResult(response, "User created successfully"));
        }
        catch (DuplicateEmailException ex)
        {
            return Error<UserResponse>(ex.Message, HttpStatusCode.Conflict);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return Error<UserResponse>("An error occurred while creating the user", 
                HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    /// Advanced search with multiple binding sources
    /// </summary>
    [HttpGet("search")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserResponse>>>> SearchUsers(
        [FromQuery] string? name,
        [FromQuery] string? email,
        [FromQuery] Gender? gender,
        [FromQuery] int? minAge,
        [FromQuery] int? maxAge,
        [FromQuery] List<string>? skills,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromHeader(Name = "X-Sort-By")] string? sortBy = null,
        [FromHeader(Name = "X-Sort-Direction")] string? sortDirection = "asc")
    {
        var searchCriteria = new UserSearchCriteria
        {
            Name = name,
            Email = email,
            Gender = gender,
            MinAge = minAge,
            MaxAge = maxAge,
            Skills = skills ?? new List<string>(),
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection
        };

        // Validate search criteria
        if (searchCriteria.MinAge.HasValue && searchCriteria.MaxAge.HasValue && 
            searchCriteria.MinAge > searchCriteria.MaxAge)
        {
            return Error<PagedResult<UserResponse>>("MinAge cannot be greater than MaxAge");
        }

        var users = await _userService.SearchUsersAsync(searchCriteria);
        var response = _mapper.Map<PagedResult<UserResponse>>(users);

        return Success(response);
    }

    /// <summary>
    /// Bulk create users with file upload
    /// </summary>
    [HttpPost("bulk")]
    [RequestSizeLimit(5_000_000)] // 5MB limit
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<BulkCreateResult>>> BulkCreateUsers(
        IFormFile file,
        [FromForm] bool skipDuplicates = false)
    {
        if (file == null || file.Length == 0)
        {
            return Error<BulkCreateResult>("File is required");
        }

        if (!file.ContentType.Equals("text/csv", StringComparison.OrdinalIgnoreCase))
        {
            return Error<BulkCreateResult>("Only CSV files are supported");
        }

        try
        {
            var result = await _userService.BulkCreateUsersFromCsvAsync(file, skipDuplicates);
            return Success(result, $"Processed {result.TotalProcessed} users");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing bulk user creation");
            return Error<BulkCreateResult>("An error occurred while processing the file", 
                HttpStatusCode.InternalServerError);
        }
    }
}

// FluentValidation Example
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private readonly IUserService _userService;

    public CreateUserRequestValidator(IUserService userService)
    {
        _userService = userService;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(2, 50).WithMessage("First name must be between 2 and 50 characters")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name can only contain letters and spaces");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MustAsync(BeUniqueEmail).WithMessage("Email address is already in use");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .Must(BeValidAge).WithMessage("Must be at least 18 years old");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .Must(BeStrongPassword).WithMessage("Password must meet strength requirements");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.Skills)
            .Must(skills => skills.Count <= 10).WithMessage("Maximum 10 skills allowed")
            .Must(skills => skills.All(skill => !string.IsNullOrWhiteSpace(skill)))
            .WithMessage("Skills cannot be empty");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var existingUser = await _userService.GetByEmailAsync(email);
        return existingUser == null;
    }

    private bool BeValidAge(DateTime dateOfBirth)
    {
        var age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
            age--;
        
        return age >= 18;
    }

    private bool BeStrongPassword(string password)
    {
        return password.Length >= 8 &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit) &&
               password.Any(ch => !char.IsLetterOrDigit(ch));
    }
}

// Supporting Models
public class UserSearchCriteria
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public Gender? Gender { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public List<string> Skills { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
}

public class BulkCreateResult
{
    public int TotalProcessed { get; set; }
    public int SuccessfulCreations { get; set; }
    public int Duplicates { get; set; }
    public int Errors { get; set; }
    public List<string> ErrorDetails { get; set; } = new();
}

// Program.cs Configuration for Validation
var builder = WebApplication.CreateBuilder(args);

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

// Add custom model binder
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new CustomDateTimeModelBinderProvider());
    
    // Configure validation behavior
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

// Configure validation error responses
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        var response = ApiResponse<object>.ErrorResult("Validation failed", errors);
        return new BadRequestObjectResult(response);
    };
});

var app = builder.Build();
```

---

## Authentication & Authorization

### Beginner Level

#### Q10: How do you implement JWT authentication in .NET Core?
**Answer:**
JWT (JSON Web Token) authentication is a stateless authentication mechanism that's ideal for distributed applications and APIs. It provides secure token-based authentication without requiring server-side session storage.

**JWT Implementation Example:**

```csharp
// JWT Configuration and Setup
public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(7);
}

// JWT Service Interface and Implementation
public interface IJwtService
{
    string GenerateToken(User user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
}

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<JwtService> _logger;

    public JwtService(
        IOptions<JwtSettings> jwtSettings,
        UserManager<User> userManager,
        ILogger<JwtService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _userManager = userManager;
        _logger = logger;
    }

    public string GenerateToken(User user, IList<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Add role claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Add custom claims if needed
        if (!string.IsNullOrEmpty(user.Department))
        {
            claims.Add(new Claim("department", user.Department));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        _logger.LogInformation("JWT token generated for user {UserId}", user.Id);
        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = false // Don't validate lifetime for refresh
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            
            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to validate expired token");
            return null;
        }
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(token);
        if (principal == null)
        {
            return AuthenticationResult.Failure("Invalid token");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return AuthenticationResult.Failure("Invalid token claims");
        }

        var user = await _userManager.FindByIdAsync(userIdClaim.Value);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return AuthenticationResult.Failure("Invalid refresh token");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newToken = GenerateToken(user, roles);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.Add(_jwtSettings.RefreshTokenLifetime);
        
        await _userManager.UpdateAsync(user);

        return AuthenticationResult.Success(newToken, newRefreshToken);
    }
}

// Authentication Result Models
public class AuthenticationResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Errors { get; set; } = new();

    public static AuthenticationResult Success(string token, string refreshToken)
    {
        return new AuthenticationResult
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public static AuthenticationResult Failure(string errorMessage)
    {
        return new AuthenticationResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}

// Extended User Model with Refresh Token
public class User : IdentityUser<int>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Department { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Refresh token properties
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}

// Authentication Controller
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtService jwtService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<LoginResponse>.ErrorResult("Invalid request data"));
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Login attempt with invalid email: {Email}", request.Email);
                return Unauthorized(ApiResponse<LoginResponse>.ErrorResult("Invalid credentials"));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked: {Email}", request.Email);
                    return Unauthorized(ApiResponse<LoginResponse>.ErrorResult("Account is locked due to multiple failed attempts"));
                }

                _logger.LogWarning("Invalid login attempt for user: {Email}", request.Email);
                return Unauthorized(ApiResponse<LoginResponse>.ErrorResult("Invalid credentials"));
            }

            // Generate tokens
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Update user with refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.LastLoginAt = DateTime.UtcNow;
            
            await _userManager.UpdateAsync(user);

            var response = new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new UserInfoResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList()
                }
            };

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);
            return Ok(ApiResponse<LoginResponse>.SuccessResult(response, "Login successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Email}", request.Email);
            return StatusCode(500, ApiResponse<LoginResponse>.ErrorResult("An error occurred during login"));
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _jwtService.RefreshTokenAsync(request.Token, request.RefreshToken);
            
            if (!result.Success)
            {
                return Unauthorized(ApiResponse<LoginResponse>.ErrorResult(result.ErrorMessage ?? "Invalid token"));
            }

            var response = new LoginResponse
            {
                Token = result.Token!,
                RefreshToken = result.RefreshToken!,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            return Ok(ApiResponse<LoginResponse>.SuccessResult(response, "Token refreshed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, ApiResponse<LoginResponse>.ErrorResult("An error occurred while refreshing token"));
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Logout()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                var user = await _userManager.FindByIdAsync(userIdClaim.Value);
                if (user != null)
                {
                    // Invalidate refresh token
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "Logout successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred during logout"));
        }
    }
}

// Request/Response Models
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public class RefreshTokenRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserInfoResponse? User { get; set; }
}

public class UserInfoResponse
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public List<string> Roles { get; set; } = new();
}

// Program.cs Configuration
var builder = WebApplication.CreateBuilder(args);

// Configure JWT settings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

// Add Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? "")),
        ClockSkew = TimeSpan.Zero // Remove delay of token when expire
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

// Register services
builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

// Configure pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
```

#### Q11: How do you implement role-based and policy-based authorization?
**Answer:**
.NET Core provides flexible authorization mechanisms including role-based authorization, claims-based authorization, and policy-based authorization for fine-grained access control.

**Authorization Implementation Example:**

```csharp
// Authorization Policies Configuration
public static class AuthorizationPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string ManagerOrAbove = "ManagerOrAbove";
    public const string SameDepartment = "SameDepartment";
    public const string MinimumAge = "MinimumAge";
    public const string ResourceOwner = "ResourceOwner";
}

// Custom Claims
public static class CustomClaims
{
    public const string Department = "department";
    public const string EmployeeLevel = "employee_level";
    public const string DateOfBirth = "date_of_birth";
    public const string Permission = "permission";
}

// Permission-based Authorization
public static class Permissions
{
    public const string ViewUsers = "users.view";
    public const string CreateUsers = "users.create";
    public const string EditUsers = "users.edit";
    public const string DeleteUsers = "users.delete";
    
    public const string ViewOrders = "orders.view";
    public const string CreateOrders = "orders.create";
    public const string ProcessOrders = "orders.process";
    public const string CancelOrders = "orders.cancel";
    
    public const string ViewReports = "reports.view";
    public const string CreateReports = "reports.create";
    public const string ExportData = "data.export";
}

// Authorization Requirements
public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }

    public MinimumAgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }
}

public class SameDepartmentRequirement : IAuthorizationRequirement { }

public class ResourceOwnerRequirement : IAuthorizationRequirement
{
    public string ResourceIdParameter { get; }

    public ResourceOwnerRequirement(string resourceIdParameter = "id")
    {
        ResourceIdParameter = resourceIdParameter;
    }
}

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

// Authorization Handlers
public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MinimumAgeRequirement requirement)
    {
        var dateOfBirthClaim = context.User.FindFirst(CustomClaims.DateOfBirth);
        
        if (dateOfBirthClaim != null && DateTime.TryParse(dateOfBirthClaim.Value, out var dateOfBirth))
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                age--;

            if (age >= requirement.MinimumAge)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}

public class SameDepartmentHandler : AuthorizationHandler<SameDepartmentRequirement, User>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameDepartmentRequirement requirement,
        User resource)
    {
        var userDepartment = context.User.FindFirst(CustomClaims.Department)?.Value;
        
        if (userDepartment != null && userDepartment == resource.Department)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class ResourceOwnerHandler : AuthorizationHandler<ResourceOwnerRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ResourceOwnerHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOwnerRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return Task.CompletedTask;
        }

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Task.CompletedTask;
        }

        // Get resource ID from route parameters
        var resourceId = httpContext.Request.RouteValues[requirement.ResourceIdParameter]?.ToString();
        
        if (resourceId != null && userIdClaim.Value == resourceId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permissions = context.User.FindAll(CustomClaims.Permission).Select(c => c.Value);
        
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

// Permission Attribute for easier use
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base($"permission.{permission}")
    {
    }
}

// Controllers with Different Authorization Approaches
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires authentication for all actions
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthorizationService _authorizationService;

    public UsersController(IUserService userService, IAuthorizationService authorizationService)
    {
        _userService = userService;
        _authorizationService = authorizationService;
    }

    // Role-based authorization
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // Policy-based authorization
    [HttpGet("managers")]
    [Authorize(Policy = AuthorizationPolicies.ManagerOrAbove)]
    public async Task<ActionResult<IEnumerable<User>>> GetManagers()
    {
        var managers = await _userService.GetUsersByRoleAsync("Manager");
        return Ok(managers);
    }

    // Permission-based authorization using attribute
    [HttpPost]
    [HasPermission(Permissions.CreateUsers)]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // Resource-based authorization with manual check
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Check if user can access this resource
        var authResult = await _authorizationService.AuthorizeAsync(User, user, AuthorizationPolicies.SameDepartment);
        
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        return Ok(user);
    }

    // Multiple authorization requirements
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [HasPermission(Permissions.DeleteUsers)]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var success = await _userService.DeleteUserAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    // Age-based authorization
    [HttpGet("sensitive-data")]
    [Authorize(Policy = AuthorizationPolicies.MinimumAge)]
    public async Task<ActionResult<object>> GetSensitiveData()
    {
        // Only users 18+ can access this endpoint
        var data = await _userService.GetSensitiveDataAsync();
        return Ok(data);
    }
}

// Orders Controller with Resource-based Authorization
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IAuthorizationService _authorizationService;

    public OrdersController(IOrderService orderService, IAuthorizationService authorizationService)
    {
        _orderService = orderService;
        _authorizationService = authorizationService;
    }

    [HttpGet("my-orders")]
    [Authorize(Policy = AuthorizationPolicies.ResourceOwner)]
    public async Task<ActionResult<IEnumerable<Order>>> GetMyOrders()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var orders = await _orderService.GetOrdersByUserIdAsync(int.Parse(userId));
        return Ok(orders);
    }

    [HttpPost("{orderId}/process")]
    [HasPermission(Permissions.ProcessOrders)]
    public async Task<ActionResult> ProcessOrder(int orderId)
    {
        var result = await _orderService.ProcessOrderAsync(orderId);
        return result ? Ok() : BadRequest("Failed to process order");
    }

    [HttpPost("{orderId}/cancel")]
    public async Task<ActionResult> CancelOrder(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return NotFound();
        }

        // Check if user can cancel this order (owner or admin)
        var isOwner = order.UserId.ToString() == User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");
        var hasPermission = User.HasClaim(CustomClaims.Permission, Permissions.CancelOrders);

        if (!isOwner && !isAdmin && !hasPermission)
        {
            return Forbid();
        }

        var result = await _orderService.CancelOrderAsync(orderId);
        return result ? Ok() : BadRequest("Failed to cancel order");
    }
}

// Program.cs Authorization Configuration
var builder = WebApplication.CreateBuilder(args);

// Add authorization services
builder.Services.AddAuthorization(options =>
{
    // Role-based policies
    options.AddPolicy(AuthorizationPolicies.AdminOnly, policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy(AuthorizationPolicies.ManagerOrAbove, policy =>
        policy.RequireRole("Admin", "Manager"));

    // Custom requirement policies
    options.AddPolicy(AuthorizationPolicies.MinimumAge, policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(18)));

    options.AddPolicy(AuthorizationPolicies.SameDepartment, policy =>
        policy.Requirements.Add(new SameDepartmentRequirement()));

    options.AddPolicy(AuthorizationPolicies.ResourceOwner, policy =>
        policy.Requirements.Add(new ResourceOwnerRequirement()));

    // Permission-based policies
    foreach (var permission in GetAllPermissions())
    {
        options.AddPolicy($"permission.{permission}", policy =>
            policy.Requirements.Add(new PermissionRequirement(permission)));
    }
});

// Register authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeHandler>();
builder.Services.AddScoped<IAuthorizationHandler, SameDepartmentHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ResourceOwnerHandler>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddHttpContextAccessor();

static string[] GetAllPermissions()
{
    return new[]
    {
        Permissions.ViewUsers, Permissions.CreateUsers, Permissions.EditUsers, Permissions.DeleteUsers,
        Permissions.ViewOrders, Permissions.CreateOrders, Permissions.ProcessOrders, Permissions.CancelOrders,
        Permissions.ViewReports, Permissions.CreateReports, Permissions.ExportData
    };
}

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

// Enhanced User Service with Claims and Permissions
public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public UserService(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<User> CreateUserWithPermissionsAsync(CreateUserRequest request)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Department = request.Department,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (result.Succeeded)
        {
            // Add to role
            await _userManager.AddToRoleAsync(user, request.Role);

            // Add custom claims
            var claims = new List<Claim>
            {
                new(CustomClaims.Department, request.Department),
                new(CustomClaims.EmployeeLevel, request.EmployeeLevel),
                new(CustomClaims.DateOfBirth, request.DateOfBirth.ToString("yyyy-MM-dd"))
            };

            // Add permissions based on role
            var permissions = GetPermissionsForRole(request.Role);
            claims.AddRange(permissions.Select(p => new Claim(CustomClaims.Permission, p)));

            await _userManager.AddClaimsAsync(user, claims);
        }

        return user;
    }

    private string[] GetPermissionsForRole(string role)
    {
        return role switch
        {
            "Admin" => new[]
            {
                Permissions.ViewUsers, Permissions.CreateUsers, Permissions.EditUsers, Permissions.DeleteUsers,
                Permissions.ViewOrders, Permissions.CreateOrders, Permissions.ProcessOrders, Permissions.CancelOrders,
                Permissions.ViewReports, Permissions.CreateReports, Permissions.ExportData
            },
            "Manager" => new[]
            {
                Permissions.ViewUsers, Permissions.CreateUsers, Permissions.EditUsers,
                Permissions.ViewOrders, Permissions.ProcessOrders,
                Permissions.ViewReports, Permissions.CreateReports
            },
            "Employee" => new[]
            {
                Permissions.ViewUsers, Permissions.ViewOrders, Permissions.CreateOrders
            },
            _ => Array.Empty<string>()
        };
    }
}
```

### Intermediate Level

#### Q12: How do you implement OAuth 2.0 and OpenID Connect integration?
**Answer:**
OAuth 2.0 and OpenID Connect provide secure authentication and authorization using external identity providers like Google, Microsoft, or custom identity servers.

**OAuth 2.0 / OpenID Connect Implementation:**

```csharp
// OAuth Configuration Models
public class OAuthSettings
{
    public GoogleOAuthSettings Google { get; set; } = new();
    public MicrosoftOAuthSettings Microsoft { get; set; } = new();
    public CustomOAuthSettings Custom { get; set; } = new();
}

public class GoogleOAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = { "openid", "profile", "email" };
}

public class MicrosoftOAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = { "openid", "profile", "email" };
}

public class CustomOAuthSettings
{
    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = { "openid", "profile", "email" };
}

// External Authentication Service
public interface IExternalAuthService
{
    Task<ExternalAuthResult> AuthenticateAsync(string provider, string code, string? state = null);
    Task<User> CreateOrUpdateUserFromExternalAsync(ExternalUserInfo externalUser, string provider);
    string GetAuthorizationUrl(string provider, string redirectUri, string? state = null);
}

public class ExternalAuthService : IExternalAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExternalAuthService> _logger;
    private readonly HttpClient _httpClient;

    public ExternalAuthService(
        UserManager<User> userManager,
        IJwtService jwtService,
        IConfiguration configuration,
        ILogger<ExternalAuthService> logger,
        HttpClient httpClient)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
    }

    public string GetAuthorizationUrl(string provider, string redirectUri, string? state = null)
    {
        return provider.ToLower() switch
        {
            "google" => GetGoogleAuthUrl(redirectUri, state),
            "microsoft" => GetMicrosoftAuthUrl(redirectUri, state),
            "custom" => GetCustomAuthUrl(redirectUri, state),
            _ => throw new ArgumentException($"Unsupported provider: {provider}")
        };
    }

    public async Task<ExternalAuthResult> AuthenticateAsync(string provider, string code, string? state = null)
    {
        try
        {
            var externalUser = provider.ToLower() switch
            {
                "google" => await AuthenticateWithGoogleAsync(code),
                "microsoft" => await AuthenticateWithMicrosoftAsync(code),
                "custom" => await AuthenticateWithCustomProviderAsync(code),
                _ => throw new ArgumentException($"Unsupported provider: {provider}")
            };

            if (externalUser == null)
            {
                return ExternalAuthResult.Failure("Failed to authenticate with external provider");
            }

            var user = await CreateOrUpdateUserFromExternalAsync(externalUser, provider);
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles);

            return ExternalAuthResult.Success(token, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during external authentication with provider {Provider}", provider);
            return ExternalAuthResult.Failure("External authentication failed");
        }
    }

    private string GetGoogleAuthUrl(string redirectUri, string? state)
    {
        var googleSettings = _configuration.GetSection("OAuth:Google").Get<GoogleOAuthSettings>();
        
        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = googleSettings!.ClientId,
            ["redirect_uri"] = redirectUri,
            ["scope"] = string.Join(" ", googleSettings.Scopes),
            ["response_type"] = "code",
            ["access_type"] = "offline",
            ["prompt"] = "consent"
        };

        if (!string.IsNullOrEmpty(state))
        {
            parameters["state"] = state;
        }

        var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
        return $"https://accounts.google.com/o/oauth2/v2/auth?{queryString}";
    }

    private async Task<ExternalUserInfo?> AuthenticateWithGoogleAsync(string code)
    {
        var googleSettings = _configuration.GetSection("OAuth:Google").Get<GoogleOAuthSettings>();
        
        // Exchange code for access token
        var tokenRequest = new Dictionary<string, string>
        {
            ["client_id"] = googleSettings!.ClientId,
            ["client_secret"] = googleSettings.ClientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = _configuration["OAuth:RedirectUri"] ?? ""
        };

        var tokenResponse = await _httpClient.PostAsync(
            "https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(tokenRequest));

        if (!tokenResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to exchange code for token with Google: {StatusCode}", tokenResponse.StatusCode);
            return null;
        }

        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<GoogleTokenResponse>();
        if (tokenData?.AccessToken == null)
        {
            return null;
        }

        // Get user info
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenData.AccessToken);

        var userInfoResponse = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
        if (!userInfoResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var googleUser = await userInfoResponse.Content.ReadFromJsonAsync<GoogleUserInfo>();
        if (googleUser == null)
        {
            return null;
        }

        return new ExternalUserInfo
        {
            Id = googleUser.Id,
            Email = googleUser.Email,
            FirstName = googleUser.GivenName,
            LastName = googleUser.FamilyName,
            Picture = googleUser.Picture,
            EmailVerified = googleUser.VerifiedEmail
        };
    }

    private async Task<ExternalUserInfo?> AuthenticateWithMicrosoftAsync(string code)
    {
        var microsoftSettings = _configuration.GetSection("OAuth:Microsoft").Get<MicrosoftOAuthSettings>();
        
        var tokenRequest = new Dictionary<string, string>
        {
            ["client_id"] = microsoftSettings!.ClientId,
            ["client_secret"] = microsoftSettings.ClientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = _configuration["OAuth:RedirectUri"] ?? "",
            ["scope"] = string.Join(" ", microsoftSettings.Scopes)
        };

        var tokenResponse = await _httpClient.PostAsync(
            $"https://login.microsoftonline.com/{microsoftSettings.TenantId}/oauth2/v2.0/token",
            new FormUrlEncodedContent(tokenRequest));

        if (!tokenResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<MicrosoftTokenResponse>();
        if (tokenData?.AccessToken == null)
        {
            return null;
        }

        // Get user info from Microsoft Graph
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenData.AccessToken);

        var userInfoResponse = await _httpClient.GetAsync("https://graph.microsoft.com/v1.0/me");
        if (!userInfoResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var microsoftUser = await userInfoResponse.Content.ReadFromJsonAsync<MicrosoftUserInfo>();
        if (microsoftUser == null)
        {
            return null;
        }

        return new ExternalUserInfo
        {
            Id = microsoftUser.Id,
            Email = microsoftUser.Mail ?? microsoftUser.UserPrincipalName,
            FirstName = microsoftUser.GivenName,
            LastName = microsoftUser.Surname,
            EmailVerified = true // Microsoft accounts are typically verified
        };
    }

    public async Task<User> CreateOrUpdateUserFromExternalAsync(ExternalUserInfo externalUser, string provider)
    {
        var user = await _userManager.FindByEmailAsync(externalUser.Email);
        
        if (user == null)
        {
            // Create new user
            user = new User
            {
                UserName = externalUser.Email,
                Email = externalUser.Email,
                FirstName = externalUser.FirstName,
                LastName = externalUser.LastName,
                EmailConfirmed = externalUser.EmailVerified,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            // Add default role
            await _userManager.AddToRoleAsync(user, "User");

            _logger.LogInformation("Created new user from external provider {Provider}: {Email}", provider, externalUser.Email);
        }
        else
        {
            // Update existing user info
            var updated = false;
            
            if (user.FirstName != externalUser.FirstName)
            {
                user.FirstName = externalUser.FirstName;
                updated = true;
            }
            
            if (user.LastName != externalUser.LastName)
            {
                user.LastName = externalUser.LastName;
                updated = true;
            }

            if (updated)
            {
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("Updated user info from external provider {Provider}: {Email}", provider, externalUser.Email);
            }
        }

        // Add or update external login info
        var existingLogin = await _userManager.FindLoginAsync(user, provider, externalUser.Id);
        if (existingLogin.User == null)
        {
            var loginInfo = new UserLoginInfo(provider, externalUser.Id, provider);
            await _userManager.AddLoginAsync(user, loginInfo);
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return user;
    }

    private async Task<ExternalUserInfo?> AuthenticateWithCustomProviderAsync(string code)
    {
        // Implementation for custom OIDC provider (like IdentityServer)
        var customSettings = _configuration.GetSection("OAuth:Custom").Get<CustomOAuthSettings>();
        
        // Discover endpoints
        var discoveryDoc = await _httpClient.GetFromJsonAsync<OpenIdConnectDiscoveryDocument>(
            $"{customSettings!.Authority}/.well-known/openid_configuration");
        
        if (discoveryDoc?.TokenEndpoint == null)
        {
            return null;
        }

        // Exchange code for tokens
        var tokenRequest = new Dictionary<string, string>
        {
            ["client_id"] = customSettings.ClientId,
            ["client_secret"] = customSettings.ClientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = _configuration["OAuth:RedirectUri"] ?? ""
        };

        var tokenResponse = await _httpClient.PostAsync(
            discoveryDoc.TokenEndpoint,
            new FormUrlEncodedContent(tokenRequest));

        if (!tokenResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<CustomTokenResponse>();
        if (tokenData?.AccessToken == null)
        {
            return null;
        }

        // Get user info
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenData.AccessToken);

        var userInfoResponse = await _httpClient.GetAsync(discoveryDoc.UserinfoEndpoint);
        if (!userInfoResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var customUser = await userInfoResponse.Content.ReadFromJsonAsync<CustomUserInfo>();
        if (customUser == null)
        {
            return null;
        }

        return new ExternalUserInfo
        {
            Id = customUser.Sub,
            Email = customUser.Email,
            FirstName = customUser.GivenName,
            LastName = customUser.FamilyName,
            EmailVerified = customUser.EmailVerified
        };
    }
}

// External Authentication Controller
[ApiController]
[Route("api/[controller]")]
public class ExternalAuthController : ControllerBase
{
    private readonly IExternalAuthService _externalAuthService;
    private readonly ILogger<ExternalAuthController> _logger;

    public ExternalAuthController(
        IExternalAuthService externalAuthService,
        ILogger<ExternalAuthController> logger)
    {
        _externalAuthService = externalAuthService;
        _logger = logger;
    }

    [HttpGet("{provider}/authorize")]
    public ActionResult GetAuthorizationUrl(string provider, [FromQuery] string? returnUrl = null)
    {
        try
        {
            var redirectUri = $"{Request.Scheme}://{Request.Host}/api/externalauth/{provider}/callback";
            var state = !string.IsNullOrEmpty(returnUrl) ? Base64UrlEncoder.Encode(returnUrl) : null;
            
            var authUrl = _externalAuthService.GetAuthorizationUrl(provider, redirectUri, state);
            
            return Ok(new { AuthorizationUrl = authUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("{provider}/callback")]
    public async Task<ActionResult> HandleCallback(
        string provider,
        [FromQuery] string? code,
        [FromQuery] string? state,
        [FromQuery] string? error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            _logger.LogWarning("OAuth error from provider {Provider}: {Error}", provider, error);
            return BadRequest(new { Error = $"OAuth error: {error}" });
        }

        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(new { Error = "Authorization code is required" });
        }

        try
        {
            var result = await _externalAuthService.AuthenticateAsync(provider, code, state);
            
            if (!result.Success)
            {
                return BadRequest(new { Error = result.ErrorMessage });
            }

            var returnUrl = "/dashboard"; // Default return URL
            if (!string.IsNullOrEmpty(state))
            {
                try
                {
                    returnUrl = Base64UrlEncoder.Decode(state);
                }
                catch
                {
                    // Invalid state, use default
                }
            }

            // Redirect to frontend with token
            var frontendUrl = $"{returnUrl}?token={result.Token}&refreshToken={result.RefreshToken}";
            return Redirect(frontendUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling OAuth callback for provider {Provider}", provider);
            return StatusCode(500, new { Error = "Authentication failed" });
        }
    }

    [HttpPost("{provider}/authenticate")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> AuthenticateWithCode(
        string provider,
        [FromBody] ExternalAuthRequest request)
    {
        try
        {
            var result = await _externalAuthService.AuthenticateAsync(provider, request.Code);
            
            if (!result.Success)
            {
                return BadRequest(ApiResponse<LoginResponse>.ErrorResult(result.ErrorMessage ?? "Authentication failed"));
            }

            var response = new LoginResponse
            {
                Token = result.Token!,
                RefreshToken = result.RefreshToken!,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new UserInfoResponse
                {
                    Id = result.User!.Id,
                    Email = result.User.Email,
                    FirstName = result.User.FirstName,
                    LastName = result.User.LastName
                }
            };

            return Ok(ApiResponse<LoginResponse>.SuccessResult(response, "Authentication successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during external authentication with provider {Provider}", provider);
            return StatusCode(500, ApiResponse<LoginResponse>.ErrorResult("Authentication failed"));
        }
    }
}

// Supporting Models and DTOs
public class ExternalAuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public User? User { get; set; }
    public string? ErrorMessage { get; set; }

    public static ExternalAuthResult Success(string token, User user)
    {
        return new ExternalAuthResult
        {
            Success = true,
            Token = token,
            User = user
        };
    }

    public static ExternalAuthResult Failure(string errorMessage)
    {
        return new ExternalAuthResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}

public class ExternalUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Picture { get; set; }
    public bool EmailVerified { get; set; }
}

public class ExternalAuthRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
    
    public string? State { get; set; }
}

// Provider-specific response models
public class GoogleTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}

public class GoogleUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("verified_email")]
    public bool VerifiedEmail { get; set; }
    
    [JsonPropertyName("given_name")]
    public string GivenName { get; set; } = string.Empty;
    
    [JsonPropertyName("family_name")]
    public string FamilyName { get; set; } = string.Empty;
    
    public string Picture { get; set; } = string.Empty;
}

// Program.cs Configuration for OAuth
var builder = WebApplication.CreateBuilder(args);

// Configure OAuth settings
builder.Services.Configure<OAuthSettings>(
    builder.Configuration.GetSection("OAuth"));

// Add external authentication services
builder.Services.AddHttpClient<IExternalAuthService, ExternalAuthService>();
builder.Services.AddScoped<IExternalAuthService, ExternalAuthService>();

// Add Google authentication (alternative approach using built-in provider)
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["OAuth:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["OAuth:Google:ClientSecret"]!;
        options.CallbackPath = "/api/auth/google/callback";
        options.SaveTokens = true;
        
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        
        options.Events.OnCreatingTicket = async context =>
        {
            // Additional processing when creating authentication ticket
            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
            // Process user creation/update logic here
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

---

## LINQ (Language Integrated Query)

### Beginner Level

#### Q13: What is LINQ and what are its main advantages?
**Answer:**
LINQ (Language Integrated Query) is a powerful feature in .NET that provides a uniform way to query data from different data sources using a consistent syntax. It integrates query capabilities directly into C# and VB.NET languages.

**Key Advantages:**
- **Type Safety**: Compile-time checking of queries
- **IntelliSense Support**: Auto-completion and syntax checking
- **Unified Syntax**: Same syntax for different data sources
- **Deferred Execution**: Queries execute only when enumerated
- **Composability**: Queries can be built incrementally

**Basic LINQ Examples:**

```csharp
// Sample Data Models
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public int? ManagerId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Budget { get; set; }
}

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int> EmployeeIds { get; set; } = new();
}

// Basic LINQ Query Examples
public class BasicLinqExamples
{
    private readonly List<Employee> _employees;
    private readonly List<Department> _departments;
    private readonly List<Project> _projects;

    public BasicLinqExamples()
    {
        _employees = GetSampleEmployees();
        _departments = GetSampleDepartments();
        _projects = GetSampleProjects();
    }

    // 1. Filtering with Where
    public IEnumerable<Employee> GetActiveEmployees()
    {
        // Method Syntax
        return _employees.Where(e => e.IsActive);
        
        // Query Syntax (equivalent)
        // return from e in _employees
        //        where e.IsActive
        //        select e;
    }

    public IEnumerable<Employee> GetHighEarners(decimal minimumSalary)
    {
        return _employees
            .Where(e => e.Salary >= minimumSalary && e.IsActive)
            .OrderByDescending(e => e.Salary);
    }

    // 2. Projection with Select
    public IEnumerable<string> GetEmployeeNames()
    {
        return _employees
            .Where(e => e.IsActive)
            .Select(e => e.Name)
            .OrderBy(name => name);
    }

    public IEnumerable<object> GetEmployeeSummary()
    {
        return _employees
            .Where(e => e.IsActive)
            .Select(e => new
            {
                e.Id,
                e.Name,
                e.Department,
                FormattedSalary = e.Salary.ToString("C"),
                YearsOfService = DateTime.Now.Year - e.HireDate.Year
            });
    }

    // 3. Grouping with GroupBy
    public IEnumerable<IGrouping<string, Employee>> GroupByDepartment()
    {
        return _employees
            .Where(e => e.IsActive)
            .GroupBy(e => e.Department);
    }

    public IEnumerable<object> GetDepartmentStatistics()
    {
        return _employees
            .Where(e => e.IsActive)
            .GroupBy(e => e.Department)
            .Select(g => new
            {
                Department = g.Key,
                EmployeeCount = g.Count(),
                AverageSalary = g.Average(e => e.Salary),
                TotalSalary = g.Sum(e => e.Salary),
                HighestPaid = g.Max(e => e.Salary),
                LowestPaid = g.Min(e => e.Salary)
            })
            .OrderByDescending(stat => stat.AverageSalary);
    }

    // 4. Joining Data
    public IEnumerable<object> GetEmployeesWithDepartmentInfo()
    {
        return from emp in _employees
               join dept in _departments on emp.Department equals dept.Name
               where emp.IsActive
               select new
               {
                   emp.Name,
                   emp.Salary,
                   DepartmentName = dept.Name,
                   dept.Location,
                   dept.Budget
               };
    }

    // 5. Aggregation Operations
    public object GetSalaryStatistics()
    {
        var activeEmployees = _employees.Where(e => e.IsActive);
        
        return new
        {
            TotalEmployees = activeEmployees.Count(),
            AverageSalary = activeEmployees.Average(e => e.Salary),
            TotalPayroll = activeEmployees.Sum(e => e.Salary),
            HighestSalary = activeEmployees.Max(e => e.Salary),
            LowestSalary = activeEmployees.Min(e => e.Salary),
            MedianSalary = GetMedianSalary(activeEmployees)
        };
    }

    private decimal GetMedianSalary(IEnumerable<Employee> employees)
    {
        var sortedSalaries = employees.Select(e => e.Salary).OrderBy(s => s).ToList();
        var count = sortedSalaries.Count;
        
        if (count % 2 == 0)
        {
            return (sortedSalaries[count / 2 - 1] + sortedSalaries[count / 2]) / 2;
        }
        else
        {
            return sortedSalaries[count / 2];
        }
    }

    // 6. Set Operations
    public IEnumerable<string> GetUniqueDepartments()
    {
        return _employees
            .Select(e => e.Department)
            .Distinct()
            .OrderBy(dept => dept);
    }

    public IEnumerable<Employee> GetEmployeesInSpecificDepartments(string[] departments)
    {
        return _employees
            .Where(e => e.IsActive && departments.Contains(e.Department))
            .OrderBy(e => e.Department)
            .ThenBy(e => e.Name);
    }

    // 7. Quantifiers
    public bool HasHighEarners(decimal threshold)
    {
        return _employees.Any(e => e.IsActive && e.Salary > threshold);
    }

    public bool AllEmployeesHaveManagers()
    {
        return _employees
            .Where(e => e.IsActive)
            .All(e => e.ManagerId.HasValue);
    }

    // 8. Element Operations
    public Employee? GetEmployeeById(int id)
    {
        return _employees.FirstOrDefault(e => e.Id == id && e.IsActive);
    }

    public Employee GetHighestPaidEmployee()
    {
        return _employees
            .Where(e => e.IsActive)
            .OrderByDescending(e => e.Salary)
            .First(); // Throws if no elements; use FirstOrDefault for safety
    }

    // Sample Data
    private List<Employee> GetSampleEmployees()
    {
        return new List<Employee>
        {
            new() { Id = 1, Name = "John Doe", Department = "Engineering", Salary = 85000, HireDate = new DateTime(2020, 1, 15), ManagerId = 5 },
            new() { Id = 2, Name = "Jane Smith", Department = "Marketing", Salary = 65000, HireDate = new DateTime(2019, 3, 10), ManagerId = 6 },
            new() { Id = 3, Name = "Mike Johnson", Department = "Engineering", Salary = 92000, HireDate = new DateTime(2018, 7, 22), ManagerId = 5 },
            new() { Id = 4, Name = "Sarah Wilson", Department = "HR", Salary = 58000, HireDate = new DateTime(2021, 2, 5), ManagerId = 7 },
            new() { Id = 5, Name = "David Brown", Department = "Engineering", Salary = 120000, HireDate = new DateTime(2015, 6, 8), ManagerId = null },
            new() { Id = 6, Name = "Lisa Davis", Department = "Marketing", Salary = 95000, HireDate = new DateTime(2017, 9, 12), ManagerId = null },
            new() { Id = 7, Name = "Robert Taylor", Department = "HR", Salary = 75000, HireDate = new DateTime(2016, 11, 3), ManagerId = null },
            new() { Id = 8, Name = "Emily Clark", Department = "Engineering", Salary = 78000, HireDate = new DateTime(2022, 1, 10), ManagerId = 5, IsActive = false }
        };
    }

    private List<Department> GetSampleDepartments()
    {
        return new List<Department>
        {
            new() { Id = 1, Name = "Engineering", Location = "San Francisco", Budget = 2000000 },
            new() { Id = 2, Name = "Marketing", Location = "New York", Budget = 500000 },
            new() { Id = 3, Name = "HR", Location = "Chicago", Budget = 300000 }
        };
    }

    private List<Project> GetSampleProjects()
    {
        return new List<Project>
        {
            new() { Id = 1, Name = "Project Alpha", StartDate = new DateTime(2023, 1, 1), EmployeeIds = new List<int> { 1, 3, 5 } },
            new() { Id = 2, Name = "Project Beta", StartDate = new DateTime(2023, 3, 15), EmployeeIds = new List<int> { 2, 6 } },
            new() { Id = 3, Name = "Project Gamma", StartDate = new DateTime(2023, 6, 1), EndDate = new DateTime(2023, 12, 31), EmployeeIds = new List<int> { 1, 4, 7 } }
        };
    }
}
```

#### Q14: What is the difference between IEnumerable and IQueryable in LINQ?
**Answer:**
`IEnumerable` and `IQueryable` are both interfaces used for querying data, but they differ in how and where queries are executed.

**Key Differences:**

```csharp
// IEnumerable vs IQueryable Examples
public class QueryableVsEnumerableExamples
{
    private readonly ApplicationDbContext _context;

    public QueryableVsEnumerableExamples(ApplicationDbContext context)
    {
        _context = context;
    }

    // IEnumerable - LINQ to Objects (In-Memory)
    public void IEnumerableExample()
    {
        List<Employee> employees = GetEmployeesFromMemory();

        // This executes in memory using LINQ to Objects
        var highEarners = employees
            .Where(e => e.Salary > 80000)  // Executed in C# code
            .Select(e => new { e.Name, e.Salary })
            .OrderBy(e => e.Name)
            .ToList();

        Console.WriteLine($"Found {highEarners.Count} high earners");
        
        // Deferred execution example
        var query = employees.Where(e => e.IsActive); // Not executed yet
        
        employees.Add(new Employee { Id = 100, Name = "New Employee", IsActive = true });
        
        var count = query.Count(); // Now executes and includes the new employee
    }

    // IQueryable - LINQ to SQL/EF (Database)
    public async Task IQueryableExample()
    {
        // This builds an expression tree and executes in the database
        var highEarners = await _context.Employees
            .Where(e => e.Salary > 80000)      // Translated to SQL WHERE
            .Select(e => new { e.Name, e.Salary })  // Translated to SQL SELECT
            .OrderBy(e => e.Name)              // Translated to SQL ORDER BY
            .ToListAsync();                    // Executes the SQL query

        Console.WriteLine($"Found {highEarners.Count} high earners");
        
        // Generated SQL will be something like:
        // SELECT [e].[Name], [e].[Salary]
        // FROM [Employees] AS [e]
        // WHERE [e].[Salary] > 80000
        // ORDER BY [e].[Name]
    }

    // Performance Comparison
    public async Task PerformanceComparison()
    {
        // BAD: Loads all employees into memory first, then filters
        var allEmployees = await _context.Employees.ToListAsync(); // Loads everything
        var expensiveApproach = allEmployees
            .Where(e => e.Salary > 100000)
            .ToList(); // Filters in memory

        // GOOD: Filtering happens in database
        var efficientApproach = await _context.Employees
            .Where(e => e.Salary > 100000)  // Executed in database
            .ToListAsync(); // Only loads filtered results

        // BAD: Multiple database calls
        var inefficient = await _context.Employees
            .Where(e => e.IsActive)
            .ToListAsync(); // First call to get employees
        
        var managersInefficient = new List<Employee>();
        foreach (var emp in inefficient)
        {
            // This generates N+1 queries!
            if (emp.ManagerId.HasValue)
            {
                var manager = await _context.Employees
                    .FirstOrDefaultAsync(m => m.Id == emp.ManagerId.Value);
                if (manager != null)
                    managersInefficient.Add(manager);
            }
        }

        // GOOD: Single database call with join
        var efficient = await _context.Employees
            .Where(e => e.IsActive && e.ManagerId.HasValue)
            .Join(_context.Employees,
                  emp => emp.ManagerId,
                  mgr => mgr.Id,
                  (emp, mgr) => mgr)
            .Distinct()
            .ToListAsync();
    }

    // Custom IQueryable Extension
    public static class EmployeeQueryExtensions
    {
        public static IQueryable<Employee> IsActive(this IQueryable<Employee> query)
        {
            return query.Where(e => e.IsActive);
        }

        public static IQueryable<Employee> InDepartment(this IQueryable<Employee> query, string department)
        {
            return query.Where(e => e.Department == department);
        }

        public static IQueryable<Employee> WithSalaryRange(this IQueryable<Employee> query, decimal min, decimal max)
        {
            return query.Where(e => e.Salary >= min && e.Salary <= max);
        }

        public static IQueryable<Employee> HiredAfter(this IQueryable<Employee> query, DateTime date)
        {
            return query.Where(e => e.HireDate > date);
        }
    }

    // Using Custom Extensions
    public async Task<List<Employee>> GetFilteredEmployees()
    {
        return await _context.Employees
            .IsActive()
            .InDepartment("Engineering")
            .WithSalaryRange(70000, 150000)
            .HiredAfter(DateTime.Now.AddYears(-5))
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    // Expression Tree Example
    public void ExpressionTreeExample()
    {
        // This creates an expression tree that can be analyzed and translated
        Expression<Func<Employee, bool>> predicate = e => e.Salary > 50000 && e.IsActive;
        
        // We can examine the expression tree
        Console.WriteLine($"Expression: {predicate}");
        Console.WriteLine($"Body: {predicate.Body}");
        Console.WriteLine($"Parameters: {string.Join(", ", predicate.Parameters)}");
        
        // Compile and use as a regular delegate
        var compiled = predicate.Compile();
        var employees = GetEmployeesFromMemory();
        var filtered = employees.Where(compiled).ToList();
    }

    // Combining IQueryable and IEnumerable
    public async Task<List<object>> CombinedExample()
    {
        // Start with IQueryable (database query)
        var dbQuery = _context.Employees
            .Where(e => e.IsActive && e.Department == "Engineering")
            .Select(e => new { e.Id, e.Name, e.Salary });

        // Execute database query and switch to IEnumerable
        var dbResults = await dbQuery.ToListAsync();

        // Continue with LINQ to Objects for complex operations not supported by EF
        var finalResults = dbResults
            .Where(e => IsEligibleForBonus(e.Salary)) // Custom method not translatable to SQL
            .Select(e => new
            {
                e.Name,
                e.Salary,
                BonusAmount = CalculateBonus(e.Salary), // Custom calculation
                FormattedSalary = e.Salary.ToString("C")
            })
            .OrderBy(e => e.Name)
            .ToList();

        return finalResults.Cast<object>().ToList();
    }

    private bool IsEligibleForBonus(decimal salary)
    {
        // Complex business logic that can't be translated to SQL
        return salary > 75000 && DateTime.Now.Month == 12;
    }

    private decimal CalculateBonus(decimal salary)
    {
        return salary * 0.1m; // 10% bonus
    }

    private List<Employee> GetEmployeesFromMemory()
    {
        // Sample data for demonstration
        return new List<Employee>
        {
            new() { Id = 1, Name = "John", Salary = 85000, IsActive = true, Department = "Engineering" },
            new() { Id = 2, Name = "Jane", Salary = 65000, IsActive = true, Department = "Marketing" },
            new() { Id = 3, Name = "Mike", Salary = 92000, IsActive = true, Department = "Engineering" }
        };
    }
}

// Comparison Table
/*
┌─────────────────┬─────────────────┬─────────────────────┐
│ Aspect          │ IEnumerable<T>  │ IQueryable<T>       │
├─────────────────┼─────────────────┼─────────────────────┤
│ Namespace       │ System.Linq     │ System.Linq        │
├─────────────────┼─────────────────┼─────────────────────┤
│ Usage           │ In-memory data  │ Database queries    │
├─────────────────┼─────────────────┼─────────────────────┤
│ Execution       │ LINQ to Objects │ LINQ to SQL/EF      │
├─────────────────┼─────────────────┼─────────────────────┤
│ Query Building  │ Delegates       │ Expression Trees    │
├─────────────────┼─────────────────┼─────────────────────┤
│ Performance     │ All data loaded │ Only filtered data  │
├─────────────────┼─────────────────┼─────────────────────┤
│ Remote Queries  │ No              │ Yes                 │
├─────────────────┼─────────────────┼─────────────────────┤
│ Deferred Exec.  │ Yes             │ Yes                 │
└─────────────────┴─────────────────┴─────────────────────┘
*/
```

### Intermediate Level

#### Q15: How do you optimize LINQ queries for performance?
**Answer:**
LINQ query optimization involves understanding execution patterns, avoiding common pitfalls, and leveraging database capabilities effectively.

**Performance Optimization Techniques:**

```csharp
public class LinqPerformanceOptimization
{
    private readonly ApplicationDbContext _context;

    public LinqPerformanceOptimization(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. Projection and Select Optimization
    public async Task<List<object>> OptimizedProjection()
    {
        // BAD: Loads all columns, then projects
        var inefficient = await _context.Employees
            .Where(e => e.IsActive)
            .ToListAsync(); // Loads all columns
        
        var projected = inefficient
            .Select(e => new { e.Name, e.Salary })
            .ToList();

        // GOOD: Projects at database level
        var efficient = await _context.Employees
            .Where(e => e.IsActive)
            .Select(e => new { e.Name, e.Salary }) // Only these columns selected
            .ToListAsync();

        return efficient.Cast<object>().ToList();
    }

    // 2. Avoiding N+1 Queries with Include and Join
    public async Task<List<object>> AvoidN1Queries()
    {
        // BAD: N+1 Query Pattern
        var employeesWithN1 = await _context.Employees
            .Where(e => e.IsActive)
            .ToListAsync();

        var inefficientResult = new List<object>();
        foreach (var emp in employeesWithN1)
        {
            // This executes a separate query for each employee!
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Name == emp.Department);
            
            inefficientResult.Add(new { emp.Name, DepartmentLocation = department?.Location });
        }

        // GOOD: Using Include (for navigation properties)
        var efficientWithInclude = await _context.Employees
            .Include(e => e.DepartmentNavigation) // Assuming navigation property exists
            .Where(e => e.IsActive)
            .Select(e => new
            {
                e.Name,
                DepartmentLocation = e.DepartmentNavigation!.Location
            })
            .ToListAsync();

        // GOOD: Using Join (for non-navigation relationships)
        var efficientWithJoin = await _context.Employees
            .Where(e => e.IsActive)
            .Join(_context.Departments,
                  emp => emp.Department,
                  dept => dept.Name,
                  (emp, dept) => new
                  {
                      emp.Name,
                      DepartmentLocation = dept.Location
                  })
            .ToListAsync();

        return efficientWithJoin.Cast<object>().ToList();
    }

    // 3. Filtering Before Joins and Aggregations
    public async Task<object> OptimizedFiltering()
    {
        // BAD: Joins first, then filters
        var inefficient = await (
            from emp in _context.Employees
            join dept in _context.Departments on emp.Department equals dept.Name
            where emp.Salary > 80000 // Filtering after join
            select new { emp.Name, emp.Salary, dept.Location }
        ).ToListAsync();

        // GOOD: Filters first, then joins
        var efficient = await (
            from emp in _context.Employees.Where(e => e.Salary > 80000) // Filter first
            join dept in _context.Departments on emp.Department equals dept.Name
            select new { emp.Name, emp.Salary, dept.Location }
        ).ToListAsync();

        // GOOD: Filter both sides before joining
        var optimized = await (
            from emp in _context.Employees.Where(e => e.Salary > 80000 && e.IsActive)
            join dept in _context.Departments.Where(d => d.Budget > 100000) on emp.Department equals dept.Name
            select new { emp.Name, emp.Salary, dept.Location }
        ).ToListAsync();

        return new { Inefficient = inefficient.Count, Efficient = efficient.Count, Optimized = optimized.Count };
    }

    // 4. Pagination Optimization
    public async Task<(List<object> Data, int TotalCount)> OptimizedPagination(int page, int pageSize)
    {
        var baseQuery = _context.Employees
            .Where(e => e.IsActive)
            .OrderBy(e => e.Name);

        // BAD: Loads all records to count
        var allRecords = await baseQuery.ToListAsync();
        var inefficientCount = allRecords.Count;
        var inefficientPage = allRecords
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new { e.Name, e.Salary })
            .ToList();

        // GOOD: Separate optimized queries
        var totalCount = await baseQuery.CountAsync(); // Count at database level
        
        var pageData = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new { e.Name, e.Salary })
            .ToListAsync();

        return (pageData.Cast<object>().ToList(), totalCount);
    }

    // 5. Compiled Queries for Repeated Operations
    private static readonly Func<ApplicationDbContext, string, IAsyncEnumerable<Employee>> GetEmployeesByDepartmentQuery =
        EF.CompileAsyncQuery((ApplicationDbContext context, string department) =>
            context.Employees.Where(e => e.Department == department && e.IsActive));

    private static readonly Func<ApplicationDbContext, decimal, decimal, IAsyncEnumerable<Employee>> GetEmployeesBySalaryRangeQuery =
        EF.CompileAsyncQuery((ApplicationDbContext context, decimal minSalary, decimal maxSalary) =>
            context.Employees.Where(e => e.Salary >= minSalary && e.Salary <= maxSalary && e.IsActive));

    public async Task<List<Employee>> GetEmployeesUsingCompiledQuery(string department)
    {
        var employees = new List<Employee>();
        await foreach (var employee in GetEmployeesByDepartmentQuery(_context, department))
        {
            employees.Add(employee);
        }
        return employees;
    }

    // 6. Batch Operations
    public async Task OptimizedBatchOperations()
    {
        var employeesToUpdate = await _context.Employees
            .Where(e => e.Department == "Engineering" && e.IsActive)
            .ToListAsync();

        // BAD: Individual updates
        foreach (var emp in employeesToUpdate)
        {
            emp.Salary *= 1.1m; // 10% raise
            await _context.SaveChangesAsync(); // Separate database call for each
        }

        // GOOD: Batch update
        foreach (var emp in employeesToUpdate)
        {
            emp.Salary *= 1.1m; // 10% raise
        }
        await _context.SaveChangesAsync(); // Single database call

        // BETTER: Bulk operations (using EF Extensions or raw SQL)
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE Employees SET Salary = Salary * 1.1 WHERE Department = 'Engineering' AND IsActive = 1");
    }

    // 7. Caching Frequently Used Data
    private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions
    {
        SizeLimit = 1000
    });

    public async Task<List<Department>> GetDepartmentsWithCaching()
    {
        const string cacheKey = "all_departments";
        
        if (_cache.TryGetValue(cacheKey, out List<Department>? departments))
        {
            return departments!;
        }

        departments = await _context.Departments.ToListAsync();
        
        _cache.Set(cacheKey, departments, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            Size = departments.Count
        });

        return departments;
    }

    // 8. Query Splitting for Complex Includes
    public async Task<List<Employee>> OptimizedComplexIncludes()
    {
        // BAD: Cartesian explosion with multiple includes
        var inefficient = await _context.Employees
            .Include(e => e.Projects)
            .Include(e => e.Skills)
            .Include(e => e.DepartmentNavigation)
            .ToListAsync(); // Can result in massive join with duplicated data

        // GOOD: Split queries to avoid cartesian explosion
        var efficient = await _context.Employees
            .AsSplitQuery() // Generates separate queries for each Include
            .Include(e => e.Projects)
            .Include(e => e.Skills)
            .Include(e => e.DepartmentNavigation)
            .ToListAsync();

        return efficient;
    }

    // 9. Raw SQL for Complex Operations
    public async Task<List<object>> ComplexQueryWithRawSQL()
    {
        // When LINQ becomes too complex or inefficient, use raw SQL
        var result = await _context.Database
            .SqlQueryRaw<EmployeeSalaryRank>("""
                WITH SalaryRanks AS (
                    SELECT 
                        Id,
                        Name,
                        Department,
                        Salary,
                        ROW_NUMBER() OVER (PARTITION BY Department ORDER BY Salary DESC) as DeptRank,
                        RANK() OVER (ORDER BY Salary DESC) as OverallRank
                    FROM Employees
                    WHERE IsActive = 1
                )
                SELECT * FROM SalaryRanks WHERE DeptRank <= 3
                """)
            .ToListAsync();

        return result.Cast<object>().ToList();
    }

    // 10. Async Enumerable for Large Datasets
    public async IAsyncEnumerable<Employee> StreamLargeDataset(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Process large datasets without loading everything into memory
        await foreach (var employee in _context.Employees
                       .Where(e => e.IsActive)
                       .AsAsyncEnumerable()
                       .WithCancellation(cancellationToken))
        {
            // Process each employee individually
            yield return employee;
        }
    }

    // Performance Testing Utility
    public async Task<TimeSpan> MeasureQueryPerformance<T>(Func<Task<T>> queryFunc)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await queryFunc();
        }
        finally
        {
            stopwatch.Stop();
        }

        return stopwatch.Elapsed;
    }

    // Usage example
    public async Task CompareQueryPerformance()
    {
        var inefficientTime = await MeasureQueryPerformance(async () =>
        {
            var all = await _context.Employees.ToListAsync();
            return all.Where(e => e.Salary > 80000).ToList();
        });

        var efficientTime = await MeasureQueryPerformance(async () =>
        {
            return await _context.Employees
                .Where(e => e.Salary > 80000)
                .ToListAsync();
        });

        Console.WriteLine($"Inefficient query: {inefficientTime.TotalMilliseconds}ms");
        Console.WriteLine($"Efficient query: {efficientTime.TotalMilliseconds}ms");
    }
}

// Supporting models
public class EmployeeSalaryRank
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public int DeptRank { get; set; }
    public int OverallRank { get; set; }
}

// Performance Best Practices Summary:
/*
1. ✅ Use projection (Select) to load only needed columns
2. ✅ Filter early with Where clauses
3. ✅ Use Include or Join to avoid N+1 queries
4. ✅ Implement proper pagination with Skip/Take
5. ✅ Use compiled queries for repeated operations
6. ✅ Batch database operations
7. ✅ Cache frequently accessed static data
8. ✅ Use AsSplitQuery() for complex includes
9. ✅ Consider raw SQL for very complex queries
10. ✅ Use AsAsyncEnumerable for large datasets
11. ✅ Avoid ToList() unless necessary
12. ✅ Use CountAsync() instead of Count() for async operations
*/
```

---

## GraphQL

### Beginner Level

#### Q16: What is GraphQL and how do you implement it in .NET Core?
**Answer:**
GraphQL is a query language and runtime for APIs that provides a more efficient, powerful, and flexible alternative to REST. It allows clients to request exactly the data they need in a single request.

**Key Advantages:**
- **Single Endpoint**: One URL for all operations
- **Precise Data Fetching**: Clients specify exactly what data they need
- **Strong Type System**: Schema-first approach with type safety
- **Real-time Subscriptions**: Built-in support for real-time updates
- **Introspection**: Self-documenting API

**GraphQL Implementation in .NET Core:**

```csharp
// Install NuGet packages:
// - HotChocolate.AspNetCore
// - HotChocolate.Data.EntityFramework

// 1. GraphQL Schema Definition
// Data Models
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    
    // Navigation properties
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public List<Review> Reviews { get; set; } = new();
}

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Biography { get; set; } = string.Empty;
    
    // Navigation properties
    public List<Book> Books { get; set; } = new();
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Navigation properties
    public List<Book> Books { get; set; } = new();
}

public class Review
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5 stars
    public DateTime CreatedDate { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    
    // Navigation properties
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
}

// 2. GraphQL Types (ObjectType)
public class BookType : ObjectType<Book>
{
    protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
    {
        descriptor.Description("Represents a book in the library");

        descriptor
            .Field(b => b.Id)
            .Description("The unique identifier of the book");

        descriptor
            .Field(b => b.Title)
            .Description("The title of the book");

        descriptor
            .Field(b => b.Description)
            .Description("A brief description of the book");

        descriptor
            .Field(b => b.Price)
            .Description("The price of the book in USD");

        descriptor
            .Field(b => b.PublishedDate)
            .Description("The date when the book was published");

        descriptor
            .Field(b => b.Author)
            .Description("The author of the book")
            .Type<AuthorType>();

        descriptor
            .Field(b => b.Category)
            .Description("The category of the book")
            .Type<CategoryType>();

        descriptor
            .Field(b => b.Reviews)
            .Description("Reviews for this book")
            .Type<ListType<ReviewType>>();

        // Computed field
        descriptor
            .Field("averageRating")
            .Description("The average rating of the book")
            .Type<FloatType>()
            .Resolve(context =>
            {
                var book = context.Parent<Book>();
                return book.Reviews.Any() 
                    ? book.Reviews.Average(r => r.Rating)
                    : 0.0;
            });

        // Field with arguments
        descriptor
            .Field("reviewsWithRating")
            .Description("Get reviews with a specific rating")
            .Type<ListType<ReviewType>>()
            .Argument("rating", a => a.Type<IntType>().Description("Filter by rating"))
            .Resolve(context =>
            {
                var book = context.Parent<Book>();
                var rating = context.ArgumentValue<int?>("rating");
                
                return rating.HasValue
                    ? book.Reviews.Where(r => r.Rating == rating.Value)
                    : book.Reviews;
            });
    }
}

public class AuthorType : ObjectType<Author>
{
    protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
    {
        descriptor.Description("Represents an author");

        descriptor
            .Field("fullName")
            .Description("The full name of the author")
            .Type<StringType>()
            .Resolve(context =>
            {
                var author = context.Parent<Author>();
                return $"{author.FirstName} {author.LastName}";
            });

        descriptor
            .Field(a => a.Books)
            .Description("Books written by this author")
            .Type<ListType<BookType>>();

        descriptor
            .Field("bookCount")
            .Description("Number of books written by the author")
            .Type<IntType>()
            .Resolve(context =>
            {
                var author = context.Parent<Author>();
                return author.Books.Count;
            });
    }
}

// 3. Query Type
public class Query
{
    // Get all books with filtering and pagination
    [UseDbContext(typeof(LibraryDbContext))]
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Book> GetBooks([ScopedService] LibraryDbContext context)
    {
        return context.Books.Where(b => b.IsAvailable);
    }

    // Get book by ID
    [UseDbContext(typeof(LibraryDbContext))]
    [UseProjection]
    public async Task<Book?> GetBookById(
        int id,
        [ScopedService] LibraryDbContext context,
        CancellationToken cancellationToken)
    {
        return await context.Books
            .FirstOrDefaultAsync(b => b.Id == id && b.IsAvailable, cancellationToken);
    }

    // Get books by author
    [UseDbContext(typeof(LibraryDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Book> GetBooksByAuthor(
        int authorId,
        [ScopedService] LibraryDbContext context)
    {
        return context.Books.Where(b => b.AuthorId == authorId && b.IsAvailable);
    }

    // Search books
    [UseDbContext(typeof(LibraryDbContext))]
    [UseProjection]
    [UsePaging]
    public IQueryable<Book> SearchBooks(
        string searchTerm,
        [ScopedService] LibraryDbContext context)
    {
        return context.Books.Where(b => 
            b.IsAvailable && 
            (b.Title.Contains(searchTerm) || 
             b.Description.Contains(searchTerm) ||
             b.Author.FirstName.Contains(searchTerm) ||
             b.Author.LastName.Contains(searchTerm)));
    }

    // Get all authors
    [UseDbContext(typeof(LibraryDbContext))]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Author> GetAuthors([ScopedService] LibraryDbContext context)
    {
        return context.Authors;
    }

    // Get categories
    [UseDbContext(typeof(LibraryDbContext))]
    [UseProjection]
    public IQueryable<Category> GetCategories([ScopedService] LibraryDbContext context)
    {
        return context.Categories;
    }

    // Custom resolver with complex logic
    public async Task<object> GetBookStatistics(
        [ScopedService] LibraryDbContext context,
        CancellationToken cancellationToken)
    {
        var totalBooks = await context.Books.CountAsync(cancellationToken);
        var availableBooks = await context.Books.CountAsync(b => b.IsAvailable, cancellationToken);
        var averagePrice = await context.Books
            .Where(b => b.IsAvailable)
            .AverageAsync(b => b.Price, cancellationToken);
        
        var categoryCounts = await context.Books
            .Where(b => b.IsAvailable)
            .GroupBy(b => b.Category.Name)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return new
        {
            TotalBooks = totalBooks,
            AvailableBooks = availableBooks,
            AveragePrice = Math.Round(averagePrice, 2),
            CategoryDistribution = categoryCounts
        };
    }
}

// 4. Mutation Type
public class Mutation
{
    [UseDbContext(typeof(LibraryDbContext))]
    public async Task<Book> AddBook(
        AddBookInput input,
        [ScopedService] LibraryDbContext context,
        CancellationToken cancellationToken)
    {
        var book = new Book
        {
            Title = input.Title,
            Description = input.Description,
            Price = input.Price,
            PublishedDate = input.PublishedDate,
            AuthorId = input.AuthorId,
            CategoryId = input.CategoryId,
            IsAvailable = true
        };

        context.Books.Add(book);
        await context.SaveChangesAsync(cancellationToken);

        return book;
    }

    [UseDbContext(typeof(LibraryDbContext))]
    public async Task<Book> UpdateBook(
        UpdateBookInput input,
        [ScopedService] LibraryDbContext context,
        CancellationToken cancellationToken)
    {
        var book = await context.Books
            .FirstOrDefaultAsync(b => b.Id == input.Id, cancellationToken);

        if (book == null)
        {
            throw new GraphQLException($"Book with ID {input.Id} not found");
        }

        if (!string.IsNullOrEmpty(input.Title))
            book.Title = input.Title;
        
        if (!string.IsNullOrEmpty(input.Description))
            book.Description = input.Description;
        
        if (input.Price.HasValue)
            book.Price = input.Price.Value;

        if (input.AuthorId.HasValue)
            book.AuthorId = input.AuthorId.Value;

        if (input.CategoryId.HasValue)
            book.CategoryId = input.CategoryId.Value;

        await context.SaveChangesAsync(cancellationToken);
        return book;
    }

    [UseDbContext(typeof(LibraryDbContext))]
    public async Task<bool> DeleteBook(
        int id,
        [ScopedService] LibraryDbContext context,
        CancellationToken cancellationToken)
    {
        var book = await context.Books
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (book == null)
        {
            return false;
        }

        // Soft delete
        book.IsAvailable = false;
        await context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    [UseDbContext(typeof(LibraryDbContext))]
    public async Task<Review> AddReview(
        AddReviewInput input,
        [ScopedService] LibraryDbContext context,
        CancellationToken cancellationToken)
    {
        // Validate that book exists
        var bookExists = await context.Books
            .AnyAsync(b => b.Id == input.BookId && b.IsAvailable, cancellationToken);

        if (!bookExists)
        {
            throw new GraphQLException($"Book with ID {input.BookId} not found");
        }

        var review = new Review
        {
            Content = input.Content,
            Rating = input.Rating,
            ReviewerName = input.ReviewerName,
            BookId = input.BookId,
            CreatedDate = DateTime.UtcNow
        };

        context.Reviews.Add(review);
        await context.SaveChangesAsync(cancellationToken);

        return review;
    }
}

// 5. Input Types
public class AddBookInput
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime PublishedDate { get; set; }
    public int AuthorId { get; set; }
    public int CategoryId { get; set; }
}

public class UpdateBookInput
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? AuthorId { get; set; }
    public int? CategoryId { get; set; }
}

public class AddReviewInput
{
    public string Content { get; set; } = string.Empty;
    
    [Range(1, 5)]
    public int Rating { get; set; }
    
    public string ReviewerName { get; set; } = string.Empty;
    public int BookId { get; set; }
}

// 6. Subscription Type (Real-time updates)
public class Subscription
{
    [Subscribe]
    [Topic]
    public Book OnBookAdded([EventMessage] Book book) => book;

    [Subscribe]
    [Topic]
    public Review OnReviewAdded([EventMessage] Review review) => review;
}

// 7. DbContext
public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships and constraints
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId);

        // Seed data
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Fiction", Description = "Fictional books" },
            new Category { Id = 2, Name = "Science", Description = "Scientific books" },
            new Category { Id = 3, Name = "Biography", Description = "Biographical books" }
        );

        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", BirthDate = new DateTime(1980, 1, 1) },
            new Author { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", BirthDate = new DateTime(1975, 5, 15) }
        );
    }
}

// 8. Program.cs Configuration
var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddType<BookType>()
    .AddType<AuthorType>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddInMemorySubscriptions()
    .AddAuthorization();

// Add GraphQL subscription support
builder.Services.AddWebSockets();

var app = builder.Build();

// Configure GraphQL pipeline
app.UseWebSockets();
app.MapGraphQL();

app.Run();
```

#### Q17: How do you handle complex GraphQL queries with DataLoader for N+1 problem prevention?
**Answer:**
DataLoader is a utility for batching and caching data loading operations to solve the N+1 query problem common in GraphQL resolvers.

**DataLoader Implementation:**

```csharp
// 1. DataLoader Interfaces and Implementations
public interface IAuthorDataLoader : IDataLoader<int, Author> { }
public interface ICategoryDataLoader : IDataLoader<int, Category> { }
public interface IReviewsByBookDataLoader : IDataLoader<int, Review[]> { }

// Author DataLoader
public class AuthorDataLoader : BatchDataLoader<int, Author>, IAuthorDataLoader
{
    private readonly IDbContextFactory<LibraryDbContext> _dbContextFactory;

    public AuthorDataLoader(
        IDbContextFactory<LibraryDbContext> dbContextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<int, Author>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        var authors = await context.Authors
            .Where(a => keys.Contains(a.Id))
            .ToListAsync(cancellationToken);

        return authors.ToDictionary(a => a.Id);
    }
}

// Category DataLoader
public class CategoryDataLoader : BatchDataLoader<int, Category>, ICategoryDataLoader
{
    private readonly IDbContextFactory<LibraryDbContext> _dbContextFactory;

    public CategoryDataLoader(
        IDbContextFactory<LibraryDbContext> dbContextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<int, Category>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        var categories = await context.Categories
            .Where(c => keys.Contains(c.Id))
            .ToListAsync(cancellationToken);

        return categories.ToDictionary(c => c.Id);
    }
}

// Reviews by Book DataLoader (Group DataLoader)
public class ReviewsByBookDataLoader : GroupedDataLoader<int, Review>, IReviewsByBookDataLoader
{
    private readonly IDbContextFactory<LibraryDbContext> _dbContextFactory;

    public ReviewsByBookDataLoader(
        IDbContextFactory<LibraryDbContext> dbContextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<ILookup<int, Review>> LoadGroupedBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        var reviews = await context.Reviews
            .Where(r => keys.Contains(r.BookId))
            .ToListAsync(cancellationToken);

        return reviews.ToLookup(r => r.BookId);
    }
}

// 2. Updated GraphQL Types with DataLoaders
public class BookTypeWithDataLoader : ObjectType<Book>
{
    protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
    {
        descriptor.Description("Represents a book in the library");

        descriptor
            .Field(b => b.Author)
            .ResolveWith<BookResolvers>(r => r.GetAuthor(default!, default!, default))
            .Type<AuthorType>()
            .Description("The author of the book");

        descriptor
            .Field(b => b.Category)
            .ResolveWith<BookResolvers>(r => r.GetCategory(default!, default!, default))
            .Type<CategoryType>()
            .Description("The category of the book");

        descriptor
            .Field(b => b.Reviews)
            .ResolveWith<BookResolvers>(r => r.GetReviews(default!, default!, default))
            .Type<ListType<ReviewType>>()
            .Description("Reviews for this book");

        // Computed field with DataLoader
        descriptor
            .Field("averageRating")
            .ResolveWith<BookResolvers>(r => r.GetAverageRating(default!, default!, default))
            .Type<FloatType>()
            .Description("The average rating of the book");
    }
}

// 3. Resolvers using DataLoaders
public class BookResolvers
{
    public async Task<Author> GetAuthor(
        [Parent] Book book,
        IAuthorDataLoader authorDataLoader,
        CancellationToken cancellationToken)
    {
        return await authorDataLoader.LoadAsync(book.AuthorId, cancellationToken);
    }

    public async Task<Category> GetCategory(
        [Parent] Book book,
        ICategoryDataLoader categoryDataLoader,
        CancellationToken cancellationToken)
    {
        return await categoryDataLoader.LoadAsync(book.CategoryId, cancellationToken);
    }

    public async Task<Review[]> GetReviews(
        [Parent] Book book,
        IReviewsByBookDataLoader reviewsDataLoader,
        CancellationToken cancellationToken)
    {
        return await reviewsDataLoader.LoadAsync(book.Id, cancellationToken);
    }

    public async Task<double> GetAverageRating(
        [Parent] Book book,
        IReviewsByBookDataLoader reviewsDataLoader,
        CancellationToken cancellationToken)
    {
        var reviews = await reviewsDataLoader.LoadAsync(book.Id, cancellationToken);
        return reviews.Any() ? reviews.Average(r => r.Rating) : 0.0;
    }
}

// 4. Custom DataLoader for Complex Scenarios
public interface IBookStatisticsDataLoader : IDataLoader<int, BookStatistics> { }

public class BookStatistics
{
    public int BookId { get; set; }
    public int ReviewCount { get; set; }
    public double AverageRating { get; set; }
    public int TotalSales { get; set; }
    public DateTime LastReviewDate { get; set; }
}

public class BookStatisticsDataLoader : BatchDataLoader<int, BookStatistics>, IBookStatisticsDataLoader
{
    private readonly IDbContextFactory<LibraryDbContext> _dbContextFactory;

    public BookStatisticsDataLoader(
        IDbContextFactory<LibraryDbContext> dbContextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<int, BookStatistics>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        var statistics = await context.Reviews
            .Where(r => keys.Contains(r.BookId))
            .GroupBy(r => r.BookId)
            .Select(g => new BookStatistics
            {
                BookId = g.Key,
                ReviewCount = g.Count(),
                AverageRating = g.Average(r => r.Rating),
                LastReviewDate = g.Max(r => r.CreatedDate),
                TotalSales = 0 // Would be calculated from sales table
            })
            .ToListAsync(cancellationToken);

        // Fill in books with no reviews
        var result = keys.ToDictionary(id => id, id => new BookStatistics
        {
            BookId = id,
            ReviewCount = 0,
            AverageRating = 0,
            LastReviewDate = DateTime.MinValue,
            TotalSales = 0
        });

        foreach (var stat in statistics)
        {
            result[stat.BookId] = stat;
        }

        return result;
    }
}

// 5. Advanced DataLoader Patterns
// Caching DataLoader with Redis
public class CachedAuthorDataLoader : BatchDataLoader<int, Author>, IAuthorDataLoader
{
    private readonly IDbContextFactory<LibraryDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(15);

    public CachedAuthorDataLoader(
        IDbContextFactory<LibraryDbContext> dbContextFactory,
        IMemoryCache cache,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
    }

    protected override async Task<IReadOnlyDictionary<int, Author>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<int, Author>();
        var uncachedKeys = new List<int>();

        // Check cache first
        foreach (var key in keys)
        {
            if (_cache.TryGetValue($"author:{key}", out Author? cachedAuthor) && cachedAuthor != null)
            {
                result[key] = cachedAuthor;
            }
            else
            {
                uncachedKeys.Add(key);
            }
        }

        // Load uncached items from database
        if (uncachedKeys.Count > 0)
        {
            using var context = _dbContextFactory.CreateDbContext();
            
            var authors = await context.Authors
                .Where(a => uncachedKeys.Contains(a.Id))
                .ToListAsync(cancellationToken);

            foreach (var author in authors)
            {
                result[author.Id] = author;
                
                // Cache the result
                _cache.Set($"author:{author.Id}", author, _cacheExpiration);
            }
        }

        return result;
    }
}

// 6. DataLoader Factory Pattern
public interface IDataLoaderFactory
{
    IAuthorDataLoader CreateAuthorDataLoader();
    ICategoryDataLoader CreateCategoryDataLoader();
    IReviewsByBookDataLoader CreateReviewsByBookDataLoader();
}

public class DataLoaderFactory : IDataLoaderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DataLoaderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IAuthorDataLoader CreateAuthorDataLoader()
    {
        return _serviceProvider.GetRequiredService<IAuthorDataLoader>();
    }

    public ICategoryDataLoader CreateCategoryDataLoader()
    {
        return _serviceProvider.GetRequiredService<ICategoryDataLoader>();
    }

    public IReviewsByBookDataLoader CreateReviewsByBookDataLoader()
    {
        return _serviceProvider.GetRequiredService<IReviewsByBookDataLoader>();
    }
}

// 7. Program.cs Configuration with DataLoaders
var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework with factory pattern for DataLoaders
builder.Services.AddDbContextFactory<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register DataLoaders
builder.Services.AddScoped<IAuthorDataLoader, AuthorDataLoader>();
builder.Services.AddScoped<ICategoryDataLoader, CategoryDataLoader>();
builder.Services.AddScoped<IReviewsByBookDataLoader, ReviewsByBookDataLoader>();
builder.Services.AddScoped<IBookStatisticsDataLoader, BookStatisticsDataLoader>();
builder.Services.AddScoped<IDataLoaderFactory, DataLoaderFactory>();

// Add memory cache for caching DataLoader
builder.Services.AddMemoryCache();

// Configure GraphQL with DataLoaders
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<BookTypeWithDataLoader>()
    .AddType<AuthorType>()
    .AddType<CategoryType>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddDataLoader<IAuthorDataLoader, AuthorDataLoader>()
    .AddDataLoader<ICategoryDataLoader, CategoryDataLoader>()
    .AddDataLoader<IReviewsByBookDataLoader, ReviewsByBookDataLoader>()
    .AddDataLoader<IBookStatisticsDataLoader, BookStatisticsDataLoader>();

var app = builder.Build();

// 8. Example GraphQL Query that benefits from DataLoaders
/*
query GetBooksWithDetails {
  books(first: 10) {
    nodes {
      id
      title
      price
      author {          # DataLoader batches all author requests
        fullName
        email
      }
      category {        # DataLoader batches all category requests
        name
      }
      reviews {         # DataLoader batches all review requests
        rating
        content
        reviewerName
      }
      averageRating     # Uses batched review data
    }
  }
}

Without DataLoaders: 1 + N + N + N queries (1 for books, N for authors, N for categories, N for reviews)
With DataLoaders: 4 queries total (1 for books, 1 batched for all authors, 1 batched for all categories, 1 batched for all reviews)
*/

// Performance monitoring for DataLoaders
public class DataLoaderDiagnostics : IDataLoaderDiagnosticEventListener
{
    private readonly ILogger<DataLoaderDiagnostics> _logger;

    public DataLoaderDiagnostics(ILogger<DataLoaderDiagnostics> logger)
    {
        _logger = logger;
    }

    public void BatchRequest<TKey>(IDataLoader dataLoader, IReadOnlyList<TKey> keys)
    {
        _logger.LogDebug("DataLoader {DataLoaderName} batch request for {KeyCount} keys",
            dataLoader.GetType().Name, keys.Count);
    }

    public void BatchResult<TKey, TValue>(IDataLoader dataLoader, 
        IReadOnlyList<TKey> keys, 
        IReadOnlyDictionary<TKey, TValue> values)
    {
        _logger.LogDebug("DataLoader {DataLoaderName} batch completed. Requested: {RequestedCount}, Found: {FoundCount}",
            dataLoader.GetType().Name, keys.Count, values.Count);
    }

    public void ResolvedTaskFromCache(IDataLoader dataLoader, object key, Task task)
    {
        _logger.LogDebug("DataLoader {DataLoaderName} cache hit for key {Key}",
            dataLoader.GetType().Name, key);
    }
}

app.MapGraphQL();
app.Run();
```

---

*Continuing with Microservices section...*