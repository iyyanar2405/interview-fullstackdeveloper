namespace AI.DependencyInjection.Patterns;

/// <summary>
/// Service factory interface for creating services dynamically
/// </summary>
/// <typeparam name="T">Service type</typeparam>
public interface IServiceFactory<T>
{
    T Create();
    T Create(string key);
    T Create<TImplementation>() where TImplementation : class, T;
}

/// <summary>
/// Generic service factory implementation
/// </summary>
/// <typeparam name="T">Service type</typeparam>
public class ServiceFactory<T> : IServiceFactory<T>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _implementations;

    public ServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _implementations = new Dictionary<string, Type>();
    }

    public T Create()
    {
        var service = _serviceProvider.GetService<T>();
        if (service == null)
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered");
        
        return service;
    }

    public T Create(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        if (!_implementations.TryGetValue(key, out var implementationType))
            throw new InvalidOperationException($"No implementation registered for key: {key}");

        var service = (T?)_serviceProvider.GetService(implementationType);
        if (service == null)
            throw new InvalidOperationException($"Service of type {implementationType.Name} is not registered");

        return service;
    }

    public T Create<TImplementation>() where TImplementation : class, T
    {
        var service = _serviceProvider.GetService<TImplementation>();
        if (service == null)
            throw new InvalidOperationException($"Service of type {typeof(TImplementation).Name} is not registered");

        return service;
    }

    public void RegisterImplementation<TImplementation>(string key) where TImplementation : class, T
    {
        _implementations[key] = typeof(TImplementation);
    }
}

/// <summary>
/// Named service resolver for resolving services by name
/// </summary>
/// <typeparam name="T">Service type</typeparam>
public interface INamedServiceResolver<T>
{
    T Resolve(string name);
    IEnumerable<T> ResolveAll();
    bool TryResolve(string name, out T? service);
}

/// <summary>
/// Named service resolver implementation
/// </summary>
/// <typeparam name="T">Service type</typeparam>
public class NamedServiceResolver<T> : INamedServiceResolver<T>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Func<IServiceProvider, T>> _factories;

    public NamedServiceResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _factories = new Dictionary<string, Func<IServiceProvider, T>>();
    }

    public T Resolve(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        if (!_factories.TryGetValue(name, out var factory))
            throw new InvalidOperationException($"No service registered with name: {name}");

        return factory(_serviceProvider);
    }

    public IEnumerable<T> ResolveAll()
    {
        return _factories.Values.Select(factory => factory(_serviceProvider));
    }

    public bool TryResolve(string name, out T? service)
    {
        service = default;
        
        if (string.IsNullOrWhiteSpace(name) || !_factories.TryGetValue(name, out var factory))
            return false;

        try
        {
            service = factory(_serviceProvider);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Register(string name, Func<IServiceProvider, T> factory)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        
        _factories[name] = factory ?? throw new ArgumentNullException(nameof(factory));
    }
}

/// <summary>
/// Lazy service wrapper for deferred service creation
/// </summary>
/// <typeparam name="T">Service type</typeparam>
public interface ILazyService<T>
{
    T Value { get; }
    bool IsValueCreated { get; }
}

/// <summary>
/// Lazy service implementation
/// </summary>
/// <typeparam name="T">Service type</typeparam>
public class LazyService<T> : ILazyService<T>
{
    private readonly Lazy<T> _lazyService;

    public LazyService(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        _lazyService = new Lazy<T>(() =>
        {
            var service = serviceProvider.GetService<T>();
            if (service == null)
                throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered");
            return service;
        });
    }

    public T Value => _lazyService.Value;
    public bool IsValueCreated => _lazyService.IsValueCreated;
}

/// <summary>
/// Service decorator factory for creating decorated services
/// </summary>
/// <typeparam name="T">Service type</typeparam>
public interface IServiceDecoratorFactory<T>
{
    T CreateDecorated(params Type[] decoratorTypes);
    T CreateDecorated<TDecorator>() where TDecorator : class, T;
    T CreateDecorated<TDecorator1, TDecorator2>() 
        where TDecorator1 : class, T 
        where TDecorator2 : class, T;
}

/// <summary>
/// Service decorator factory implementation
/// </summary>
/// <typeparam name="T">Service type</typeparam>
public class ServiceDecoratorFactory<T> : IServiceDecoratorFactory<T>
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceDecoratorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public T CreateDecorated(params Type[] decoratorTypes)
    {
        if (decoratorTypes == null || decoratorTypes.Length == 0)
            throw new ArgumentException("At least one decorator type must be provided", nameof(decoratorTypes));

        // Get the base service
        var baseService = _serviceProvider.GetRequiredService<T>();
        T decoratedService = baseService;

        // Apply decorators in order
        foreach (var decoratorType in decoratorTypes)
        {
            if (!typeof(T).IsAssignableFrom(decoratorType))
                throw new ArgumentException($"Type {decoratorType.Name} does not implement {typeof(T).Name}");

            // Create decorator instance with the current service as dependency
            var constructor = decoratorType.GetConstructors()
                .FirstOrDefault(c => c.GetParameters().Any(p => typeof(T).IsAssignableFrom(p.ParameterType)));

            if (constructor == null)
                throw new InvalidOperationException($"No suitable constructor found for decorator {decoratorType.Name}");

            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                if (typeof(T).IsAssignableFrom(paramType))
                {
                    args[i] = decoratedService;
                }
                else
                {
                    args[i] = _serviceProvider.GetRequiredService(paramType);
                }
            }

            decoratedService = (T)Activator.CreateInstance(decoratorType, args)!;
        }

        return decoratedService;
    }

    public T CreateDecorated<TDecorator>() where TDecorator : class, T
    {
        return CreateDecorated(typeof(TDecorator));
    }

    public T CreateDecorated<TDecorator1, TDecorator2>() 
        where TDecorator1 : class, T 
        where TDecorator2 : class, T
    {
        return CreateDecorated(typeof(TDecorator1), typeof(TDecorator2));
    }
}

/// <summary>
/// Service scope factory for creating isolated service scopes
/// </summary>
public interface ICustomServiceScopeFactory
{
    IServiceScope CreateScope();
    IServiceScope CreateScope(Action<IServiceCollection> configureServices);
    Task<TResult> ExecuteInScopeAsync<TResult>(Func<IServiceProvider, Task<TResult>> operation);
    Task ExecuteInScopeAsync(Func<IServiceProvider, Task> operation);
}

/// <summary>
/// Custom service scope factory implementation
/// </summary>
public class CustomServiceScopeFactory : ICustomServiceScopeFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IServiceCollection _baseServices;

    public CustomServiceScopeFactory(
        IServiceScopeFactory serviceScopeFactory,
        IServiceCollection baseServices)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _baseServices = baseServices ?? throw new ArgumentNullException(nameof(baseServices));
    }

    public IServiceScope CreateScope()
    {
        return _serviceScopeFactory.CreateScope();
    }

    public IServiceScope CreateScope(Action<IServiceCollection> configureServices)
    {
        if (configureServices == null)
            throw new ArgumentNullException(nameof(configureServices));

        // Create a new service collection with base services
        var services = new ServiceCollection();
        foreach (var service in _baseServices)
        {
            services.Add(service);
        }

        // Apply custom configuration
        configureServices(services);

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();
        
        return serviceProvider.CreateScope();
    }

    public async Task<TResult> ExecuteInScopeAsync<TResult>(Func<IServiceProvider, Task<TResult>> operation)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        using var scope = CreateScope();
        return await operation(scope.ServiceProvider);
    }

    public async Task ExecuteInScopeAsync(Func<IServiceProvider, Task> operation)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        using var scope = CreateScope();
        await operation(scope.ServiceProvider);
    }
}

/// <summary>
/// Service validation helper for validating service registrations
/// </summary>
public interface IServiceValidator
{
    ValidationResult ValidateService<T>();
    ValidationResult ValidateService(Type serviceType);
    ValidationResult ValidateAllServices();
}

/// <summary>
/// Service validator implementation
/// </summary>
public class ServiceValidator : IServiceValidator
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceValidator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public ValidationResult ValidateService<T>()
    {
        return ValidateService(typeof(T));
    }

    public ValidationResult ValidateService(Type serviceType)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        try
        {
            // Try to resolve the service
            var service = _serviceProvider.GetService(serviceType);
            if (service == null)
            {
                errors.Add($"Service {serviceType.Name} is not registered");
            }
            else
            {
                // Validate dependencies
                var constructors = serviceType.GetConstructors();
                foreach (var constructor in constructors)
                {
                    foreach (var parameter in constructor.GetParameters())
                    {
                        var dependency = _serviceProvider.GetService(parameter.ParameterType);
                        if (dependency == null)
                        {
                            warnings.Add($"Dependency {parameter.ParameterType.Name} for {serviceType.Name} is not registered");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            errors.Add($"Error validating service {serviceType.Name}: {ex.Message}");
        }

        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors,
            Warnings = warnings
        };
    }

    public ValidationResult ValidateAllServices()
    {
        var allErrors = new List<string>();
        var allWarnings = new List<string>();

        // This would require access to all registered services
        // For demonstration, we'll validate common service types
        var commonServiceTypes = new[]
        {
            typeof(IUserService),
            typeof(IUserRepository),
            typeof(INotificationService),
            typeof(ICacheService)
        };

        foreach (var serviceType in commonServiceTypes)
        {
            var result = ValidateService(serviceType);
            allErrors.AddRange(result.Errors);
            allWarnings.AddRange(result.Warnings);
        }

        return new ValidationResult
        {
            IsValid = !allErrors.Any(),
            Errors = allErrors,
            Warnings = allWarnings
        };
    }
}

/// <summary>
/// Validation result model
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Date time provider interface for testability
/// </summary>
public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateTimeOffset OffsetNow { get; }
    DateTimeOffset OffsetUtcNow { get; }
}

/// <summary>
/// System date time provider implementation
/// </summary>
public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTimeOffset OffsetNow => DateTimeOffset.Now;
    public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
}