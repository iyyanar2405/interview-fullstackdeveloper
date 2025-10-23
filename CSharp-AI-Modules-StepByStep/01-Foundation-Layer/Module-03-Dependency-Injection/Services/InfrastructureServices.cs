using AI.DependencyInjection.Models;

namespace AI.DependencyInjection.Services;

/// <summary>
/// Repository interface for user data access
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

/// <summary>
/// In-memory user repository implementation
/// </summary>
public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private readonly ILogger<InMemoryUserRepository> _logger;
    private int _nextId = 1;

    public InMemoryUserRepository(ILogger<InMemoryUserRepository> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        SeedData();
    }

    public Task<User?> GetByIdAsync(int id)
    {
        _logger.LogDebug("Getting user by ID: {UserId}", id);
        
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        _logger.LogDebug("Getting user by email: {Email}", email);
        
        if (string.IsNullOrWhiteSpace(email))
            return Task.FromResult<User?>(null);

        var user = _users.FirstOrDefault(u => 
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        
        return Task.FromResult(user);
    }

    public Task<List<User>> GetAllAsync()
    {
        _logger.LogDebug("Getting all users. Count: {Count}", _users.Count);
        
        return Task.FromResult(_users.ToList());
    }

    public Task<User> CreateAsync(User user)
    {
        _logger.LogDebug("Creating new user: {Email}", user.Email);
        
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.Id = _nextId++;
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        
        _users.Add(user);
        
        _logger.LogInformation("Created user with ID: {UserId}", user.Id);
        return Task.FromResult(user);
    }

    public Task<User> UpdateAsync(User user)
    {
        _logger.LogDebug("Updating user: {UserId}", user.Id);
        
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser == null)
            throw new InvalidOperationException($"User with ID {user.Id} not found");

        var index = _users.IndexOf(existingUser);
        user.UpdatedAt = DateTime.UtcNow;
        _users[index] = user;
        
        _logger.LogInformation("Updated user: {UserId}", user.Id);
        return Task.FromResult(user);
    }

    public Task<bool> DeleteAsync(int id)
    {
        _logger.LogDebug("Deleting user: {UserId}", id);
        
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return Task.FromResult(false);

        var removed = _users.Remove(user);
        
        if (removed)
            _logger.LogInformation("Deleted user: {UserId}", id);
        
        return Task.FromResult(removed);
    }

    public Task<bool> ExistsAsync(int id)
    {
        var exists = _users.Any(u => u.Id == id);
        return Task.FromResult(exists);
    }

    private void SeedData()
    {
        _users.AddRange(new[]
        {
            new User
            {
                Id = _nextId++,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Age = 30,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new User
            {
                Id = _nextId++,
                Name = "Jane Smith",
                Email = "jane.smith@example.com",
                Age = 25,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new User
            {
                Id = _nextId++,
                Name = "Bob Johnson",
                Email = "bob.johnson@example.com",
                Age = 35,
                IsActive = false,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            }
        });

        _logger.LogInformation("Seeded {Count} users", _users.Count);
    }
}

/// <summary>
/// Notification service interface
/// </summary>
public interface INotificationService
{
    Task SendWelcomeNotificationAsync(User user);
    Task SendFarewellNotificationAsync(User user);
    Task SendEmailAsync(string to, string subject, string body);
    Task SendSmsAsync(string phoneNumber, string message);
}

/// <summary>
/// Email notification service
/// </summary>
public class EmailNotificationService : INotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly IEmailProvider _emailProvider;

    public EmailNotificationService(
        ILogger<EmailNotificationService> logger,
        IEmailProvider emailProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailProvider = emailProvider ?? throw new ArgumentNullException(nameof(emailProvider));
    }

    public async Task SendWelcomeNotificationAsync(User user)
    {
        _logger.LogInformation("Sending welcome notification to user: {UserId}", user.Id);
        
        var subject = "Welcome to our platform!";
        var body = $"Hi {user.Name},\n\nWelcome to our platform! We're excited to have you on board.\n\nBest regards,\nThe Team";
        
        await SendEmailAsync(user.Email, subject, body);
    }

    public async Task SendFarewellNotificationAsync(User user)
    {
        _logger.LogInformation("Sending farewell notification to user: {UserId}", user.Id);
        
        var subject = "Sorry to see you go";
        var body = $"Hi {user.Name},\n\nWe're sorry to see you leave our platform. If you have any feedback, please let us know.\n\nBest regards,\nThe Team";
        
        await SendEmailAsync(user.Email, subject, body);
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogDebug("Sending email to: {To}, Subject: {Subject}", to, subject);
        
        await _emailProvider.SendEmailAsync(to, subject, body);
        
        _logger.LogInformation("Email sent successfully to: {To}", to);
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        _logger.LogInformation("SMS functionality not implemented in email service");
        await Task.CompletedTask;
    }
}

/// <summary>
/// Cache service interface
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
    Task ClearAsync();
}

/// <summary>
/// In-memory cache service
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private readonly Dictionary<string, CacheItem> _cache = new();
    private readonly ILogger<InMemoryCacheService> _logger;
    private readonly object _lock = new();

    public InMemoryCacheService(ILogger<InMemoryCacheService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<T?> GetAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Task.FromResult<T?>(default);

        lock (_lock)
        {
            if (!_cache.TryGetValue(key, out var item))
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return Task.FromResult<T?>(default);
            }

            if (item.IsExpired)
            {
                _cache.Remove(key);
                _logger.LogDebug("Cache expired for key: {Key}", key);
                return Task.FromResult<T?>(default);
            }

            _logger.LogDebug("Cache hit for key: {Key}", key);
            return Task.FromResult((T?)item.Value);
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Task.CompletedTask;

        lock (_lock)
        {
            var expiryTime = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : (DateTime?)null;
            
            _cache[key] = new CacheItem
            {
                Value = value,
                ExpiryTime = expiryTime
            };

            _logger.LogDebug("Cache set for key: {Key}, Expiry: {Expiry}", key, expiryTime);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Task.CompletedTask;

        lock (_lock)
        {
            var removed = _cache.Remove(key);
            if (removed)
                _logger.LogDebug("Cache removed for key: {Key}", key);
        }

        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        lock (_lock)
        {
            var count = _cache.Count;
            _cache.Clear();
            _logger.LogInformation("Cache cleared. Removed {Count} items", count);
        }

        return Task.CompletedTask;
    }

    private class CacheItem
    {
        public object? Value { get; set; }
        public DateTime? ExpiryTime { get; set; }
        public bool IsExpired => ExpiryTime.HasValue && DateTime.UtcNow > ExpiryTime.Value;
    }
}

/// <summary>
/// Email provider interface
/// </summary>
public interface IEmailProvider
{
    Task SendEmailAsync(string to, string subject, string body);
}

/// <summary>
/// Mock email provider for demonstration
/// </summary>
public class MockEmailProvider : IEmailProvider
{
    private readonly ILogger<MockEmailProvider> _logger;

    public MockEmailProvider(ILogger<MockEmailProvider> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("MOCK EMAIL - To: {To}, Subject: {Subject}", to, subject);
        _logger.LogDebug("MOCK EMAIL - Body: {Body}", body);
        
        // Simulate email sending delay
        await Task.Delay(100);
        
        _logger.LogInformation("MOCK EMAIL - Email sent successfully");
    }
}