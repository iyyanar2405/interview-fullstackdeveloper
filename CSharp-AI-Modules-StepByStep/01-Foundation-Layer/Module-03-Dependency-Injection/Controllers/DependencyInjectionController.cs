using AI.DependencyInjection.Patterns;
using AI.DependencyInjection.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.DependencyInjection.Controllers;

/// <summary>
/// Dependency injection patterns demonstration controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DependencyInjectionController : ControllerBase
{
    private readonly IServiceFactory<IUserService> _userServiceFactory;
    private readonly INamedServiceResolver<INotificationService> _notificationResolver;
    private readonly ILazyService<ICacheService> _lazyCacheService;
    private readonly IServiceDecoratorFactory<IUserService> _decoratorFactory;
    private readonly ICustomServiceScopeFactory _scopeFactory;
    private readonly IServiceValidator _serviceValidator;
    private readonly ILogger<DependencyInjectionController> _logger;

    public DependencyInjectionController(
        IServiceFactory<IUserService> userServiceFactory,
        INamedServiceResolver<INotificationService> notificationResolver,
        ILazyService<ICacheService> lazyCacheService,
        IServiceDecoratorFactory<IUserService> decoratorFactory,
        ICustomServiceScopeFactory scopeFactory,
        IServiceValidator serviceValidator,
        ILogger<DependencyInjectionController> logger)
    {
        _userServiceFactory = userServiceFactory;
        _notificationResolver = notificationResolver;
        _lazyCacheService = lazyCacheService;
        _decoratorFactory = decoratorFactory;
        _scopeFactory = scopeFactory;
        _serviceValidator = serviceValidator;
        _logger = logger;
    }

    /// <summary>
    /// Demonstrate service factory pattern
    /// </summary>
    /// <returns>Factory demonstration result</returns>
    [HttpGet("factory-demo")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<ActionResult> DemonstrateFactory()
    {
        _logger.LogInformation("Demonstrating service factory pattern");

        try
        {
            // Create service using factory
            var userService = _userServiceFactory.Create();
            var users = await userService.GetAllUsersAsync();

            return Ok(new
            {
                pattern = "Service Factory",
                description = "Services created dynamically using factory pattern",
                usersCount = users.Count,
                factoryType = _userServiceFactory.GetType().Name,
                serviceType = userService.GetType().Name,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in factory demonstration");
            return StatusCode(500, new { message = "Factory demonstration failed" });
        }
    }

    /// <summary>
    /// Demonstrate named service resolver pattern
    /// </summary>
    /// <param name="serviceName">Service name to resolve</param>
    /// <returns>Named resolver demonstration result</returns>
    [HttpGet("named-resolver-demo/{serviceName}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(404)]
    public ActionResult DemonstrateNamedResolver(string serviceName)
    {
        _logger.LogInformation("Demonstrating named service resolver for: {ServiceName}", serviceName);

        try
        {
            if (_notificationResolver.TryResolve(serviceName, out var service))
            {
                return Ok(new
                {
                    pattern = "Named Service Resolver",
                    description = "Services resolved by name",
                    serviceName,
                    serviceType = service?.GetType().Name,
                    resolved = true,
                    timestamp = DateTime.UtcNow
                });
            }
            else
            {
                return NotFound(new
                {
                    pattern = "Named Service Resolver",
                    serviceName,
                    resolved = false,
                    message = $"Service '{serviceName}' not found"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in named resolver demonstration");
            return StatusCode(500, new { message = "Named resolver demonstration failed" });
        }
    }

    /// <summary>
    /// Demonstrate lazy service pattern
    /// </summary>
    /// <returns>Lazy service demonstration result</returns>
    [HttpGet("lazy-demo")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<ActionResult> DemonstrateLazyService()
    {
        _logger.LogInformation("Demonstrating lazy service pattern");

        try
        {
            var beforeAccess = _lazyCacheService.IsValueCreated;
            
            // Access the lazy service (this will create it)
            var cacheService = _lazyCacheService.Value;
            await cacheService.SetAsync("demo-key", "demo-value", TimeSpan.FromMinutes(5));
            
            var afterAccess = _lazyCacheService.IsValueCreated;

            return Ok(new
            {
                pattern = "Lazy Service",
                description = "Services created only when first accessed",
                wasCreatedBefore = beforeAccess,
                wasCreatedAfter = afterAccess,
                serviceType = cacheService.GetType().Name,
                demonstration = "Cache service was lazily initialized",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in lazy service demonstration");
            return StatusCode(500, new { message = "Lazy service demonstration failed" });
        }
    }

    /// <summary>
    /// Demonstrate decorator pattern
    /// </summary>
    /// <returns>Decorator demonstration result</returns>
    [HttpGet("decorator-demo")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<ActionResult> DemonstrateDecorator()
    {
        _logger.LogInformation("Demonstrating decorator pattern");

        try
        {
            // Create a decorated service (UserService wrapped with CachedUserService)
            var decoratedService = _decoratorFactory.CreateDecorated<CachedUserService>();
            var users = await decoratedService.GetAllUsersAsync();

            return Ok(new
            {
                pattern = "Decorator Pattern",
                description = "Services enhanced with additional functionality",
                decoratedServiceType = decoratedService.GetType().Name,
                usersCount = users.Count,
                decorators = new[] { "CachedUserService" },
                demonstration = "User service decorated with caching functionality",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in decorator demonstration");
            return StatusCode(500, new { message = "Decorator demonstration failed" });
        }
    }

    /// <summary>
    /// Demonstrate custom scope pattern
    /// </summary>
    /// <returns>Scope demonstration result</returns>
    [HttpGet("scope-demo")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<ActionResult> DemonstrateScope()
    {
        _logger.LogInformation("Demonstrating custom scope pattern");

        try
        {
            var result = await _scopeFactory.ExecuteInScopeAsync(async serviceProvider =>
            {
                var userService = serviceProvider.GetRequiredService<IUserService>();
                var users = await userService.GetAllUsersAsync();
                
                return new
                {
                    usersCount = users.Count,
                    scopeId = Guid.NewGuid().ToString(),
                    serviceType = userService.GetType().Name
                };
            });

            return Ok(new
            {
                pattern = "Custom Scope",
                description = "Operations executed in isolated service scope",
                result,
                demonstration = "User service accessed within custom scope",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in scope demonstration");
            return StatusCode(500, new { message = "Scope demonstration failed" });
        }
    }

    /// <summary>
    /// Demonstrate service validation pattern
    /// </summary>
    /// <returns>Validation demonstration result</returns>
    [HttpGet("validation-demo")]
    [ProducesResponseType(typeof(object), 200)]
    public ActionResult DemonstrateValidation()
    {
        _logger.LogInformation("Demonstrating service validation pattern");

        try
        {
            var userServiceValidation = _serviceValidator.ValidateService<IUserService>();
            var allServicesValidation = _serviceValidator.ValidateAllServices();

            return Ok(new
            {
                pattern = "Service Validation",
                description = "Validation of service registrations and dependencies",
                userServiceValidation = new
                {
                    isValid = userServiceValidation.IsValid,
                    errors = userServiceValidation.Errors,
                    warnings = userServiceValidation.Warnings
                },
                allServicesValidation = new
                {
                    isValid = allServicesValidation.IsValid,
                    errorCount = allServicesValidation.Errors.Count,
                    warningCount = allServicesValidation.Warnings.Count,
                    errors = allServicesValidation.Errors.Take(5), // Limit for demo
                    warnings = allServicesValidation.Warnings.Take(5) // Limit for demo
                },
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in validation demonstration");
            return StatusCode(500, new { message = "Validation demonstration failed" });
        }
    }

    /// <summary>
    /// Get information about all DI patterns
    /// </summary>
    /// <returns>DI patterns information</returns>
    [HttpGet("patterns-info")]
    [ProducesResponseType(typeof(object), 200)]
    public ActionResult GetPatternsInfo()
    {
        _logger.LogInformation("Getting DI patterns information");

        var patterns = new[]
        {
            new
            {
                name = "Service Factory",
                description = "Creates services dynamically based on runtime conditions",
                endpoint = "/api/dependencyinjection/factory-demo",
                useCases = new[] { "Plugin systems", "Strategy pattern", "Multi-tenant applications" }
            },
            new
            {
                name = "Named Service Resolver",
                description = "Resolves services by name or key",
                endpoint = "/api/dependencyinjection/named-resolver-demo/{serviceName}",
                useCases = new[] { "Multiple implementations", "Feature flags", "A/B testing" }
            },
            new
            {
                name = "Lazy Service",
                description = "Defers service creation until first access",
                endpoint = "/api/dependencyinjection/lazy-demo",
                useCases = new[] { "Expensive services", "Optional dependencies", "Performance optimization" }
            },
            new
            {
                name = "Decorator Pattern",
                description = "Enhances services with additional functionality",
                endpoint = "/api/dependencyinjection/decorator-demo",
                useCases = new[] { "Caching", "Logging", "Validation", "Security" }
            },
            new
            {
                name = "Custom Scope",
                description = "Creates isolated service scopes for operations",
                endpoint = "/api/dependencyinjection/scope-demo",
                useCases = new[] { "Isolated operations", "Testing", "Background tasks" }
            },
            new
            {
                name = "Service Validation",
                description = "Validates service registrations and dependencies",
                endpoint = "/api/dependencyinjection/validation-demo",
                useCases = new[] { "Startup validation", "Health checks", "Dependency analysis" }
            }
        };

        return Ok(new
        {
            title = "Dependency Injection Patterns",
            description = "Advanced DI patterns for enterprise applications",
            patterns,
            totalPatterns = patterns.Length,
            timestamp = DateTime.UtcNow
        });
    }
}