using AI.DependencyInjection.Patterns;
using AI.DependencyInjection.Services;

namespace AI.DependencyInjection.Extensions;

/// <summary>
/// Service collection extensions for dependency injection patterns
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add all dependency injection services and patterns
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddDependencyInjectionModule(this IServiceCollection services)
    {
        // Add core services
        services.AddCoreServices();
        
        // Add infrastructure services
        services.AddInfrastructureServices();
        
        // Add DI patterns
        services.AddDIPatterns();
        
        // Add decorators
        services.AddDecorators();

        return services;
    }

    /// <summary>
    /// Add core domain services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Register core services with appropriate lifetimes
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        services.AddScoped<INotificationService, EmailNotificationService>();
        services.AddSingleton<ICacheService, InMemoryCacheService>();
        services.AddSingleton<IEmailProvider, MockEmailProvider>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }

    /// <summary>
    /// Add infrastructure services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Add HTTP client for external services
        services.AddHttpClient();
        
        // Add memory cache
        services.AddMemoryCache();
        
        // Add background services
        services.AddHostedService<CacheCleanupService>();

        return services;
    }

    /// <summary>
    /// Add dependency injection patterns
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddDIPatterns(this IServiceCollection services)
    {
        // Service Factory pattern
        services.AddSingleton(typeof(IServiceFactory<>), typeof(ServiceFactory<>));
        
        // Named Service Resolver pattern
        services.AddSingleton(typeof(INamedServiceResolver<>), typeof(NamedServiceResolver<>));
        
        // Lazy Service pattern
        services.AddTransient(typeof(ILazyService<>), typeof(LazyService<>));
        
        // Service Decorator Factory pattern
        services.AddTransient(typeof(IServiceDecoratorFactory<>), typeof(ServiceDecoratorFactory<>));
        
        // Custom Service Scope Factory pattern
        services.AddSingleton<ICustomServiceScopeFactory>(provider =>
            new CustomServiceScopeFactory(
                provider.GetRequiredService<IServiceScopeFactory>(),
                services));
        
        // Service Validator pattern
        services.AddSingleton<IServiceValidator, ServiceValidator>();

        // Configure named services
        services.AddSingleton<INamedServiceResolver<INotificationService>>(provider =>
        {
            var resolver = new NamedServiceResolver<INotificationService>(provider);
            resolver.Register("email", sp => sp.GetRequiredService<EmailNotificationService>());
            resolver.Register("default", sp => sp.GetRequiredService<INotificationService>());
            return resolver;
        });

        return services;
    }

    /// <summary>
    /// Add decorator services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddDecorators(this IServiceCollection services)
    {
        // Register decorators
        services.AddScoped<CachedUserService>();
        
        // You can add more decorators here
        // services.AddScoped<LoggedUserService>();
        // services.AddScoped<ValidatedUserService>();

        return services;
    }

    /// <summary>
    /// Add service lifetime validation
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddServiceLifetimeValidation(this IServiceCollection services)
    {
        services.AddSingleton<IServiceLifetimeValidator, ServiceLifetimeValidator>();
        return services;
    }

    /// <summary>
    /// Configure service options
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection ConfigureServiceOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure options
        services.Configure<UserServiceOptions>(
            configuration.GetSection("UserService"));
        
        services.Configure<CacheOptions>(
            configuration.GetSection("Cache"));
        
        services.Configure<NotificationOptions>(
            configuration.GetSection("Notification"));

        return services;
    }
}

/// <summary>
/// Application builder extensions for dependency injection
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Use dependency injection validation middleware
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder</returns>
    public static IApplicationBuilder UseDependencyInjectionValidation(this IApplicationBuilder app)
    {
        // Validate services at startup
        using var scope = app.ApplicationServices.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<IServiceValidator>();
        
        var validationResult = validator.ValidateAllServices();
        if (!validationResult.IsValid)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IServiceValidator>>();
            foreach (var error in validationResult.Errors)
            {
                logger.LogError("Service validation error: {Error}", error);
            }
            
            foreach (var warning in validationResult.Warnings)
            {
                logger.LogWarning("Service validation warning: {Warning}", warning);
            }
            
            if (validationResult.Errors.Any())
            {
                throw new InvalidOperationException("Service validation failed. Check logs for details.");
            }
        }

        return app;
    }

    /// <summary>
    /// Use service information middleware
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder</returns>
    public static IApplicationBuilder UseServiceInformation(this IApplicationBuilder app)
    {
        app.UseMiddleware<ServiceInformationMiddleware>();
        return app;
    }
}

/// <summary>
/// Service lifetime validator interface
/// </summary>
public interface IServiceLifetimeValidator
{
    void ValidateLifetimes();
    List<string> GetLifetimeWarnings();
}

/// <summary>
/// Service lifetime validator implementation
/// </summary>
public class ServiceLifetimeValidator : IServiceLifetimeValidator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ServiceLifetimeValidator> _logger;

    public ServiceLifetimeValidator(
        IServiceProvider serviceProvider,
        ILogger<ServiceLifetimeValidator> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void ValidateLifetimes()
    {
        var warnings = GetLifetimeWarnings();
        foreach (var warning in warnings)
        {
            _logger.LogWarning("Service lifetime warning: {Warning}", warning);
        }
    }

    public List<string> GetLifetimeWarnings()
    {
        var warnings = new List<string>();
        
        // Add lifetime validation logic here
        // Example: Check for singleton services depending on scoped services
        
        return warnings;
    }
}

/// <summary>
/// Cache cleanup background service
/// </summary>
public class CacheCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CacheCleanupService> _logger;

    public CacheCleanupService(
        IServiceProvider serviceProvider,
        ILogger<CacheCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cache cleanup service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                
                // Perform cache cleanup operations
                // This is a placeholder - actual implementation would vary
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                
                _logger.LogDebug("Cache cleanup cycle completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in cache cleanup service");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("Cache cleanup service stopped");
    }
}

/// <summary>
/// Service information middleware
/// </summary>
public class ServiceInformationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ServiceInformationMiddleware> _logger;

    public ServiceInformationMiddleware(
        RequestDelegate next,
        ILogger<ServiceInformationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/dependencyinjection"))
        {
            var serviceType = context.RequestServices.GetType();
            _logger.LogDebug("DI request using service provider: {ServiceProviderType}", serviceType.Name);
        }

        await _next(context);
    }
}

/// <summary>
/// Service configuration options
/// </summary>
public class UserServiceOptions
{
    public int MaxUsersPerRequest { get; set; } = 100;
    public bool EnableValidation { get; set; } = true;
    public TimeSpan CacheExpiry { get; set; } = TimeSpan.FromMinutes(15);
}

/// <summary>
/// Cache configuration options
/// </summary>
public class CacheOptions
{
    public TimeSpan DefaultExpiry { get; set; } = TimeSpan.FromMinutes(15);
    public int MaxItems { get; set; } = 1000;
    public bool EnableCleanup { get; set; } = true;
}

/// <summary>
/// Notification configuration options
/// </summary>
public class NotificationOptions
{
    public bool EnableEmail { get; set; } = true;
    public bool EnableSms { get; set; } = false;
    public string DefaultSender { get; set; } = "noreply@example.com";
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetries { get; set; } = 3;
}