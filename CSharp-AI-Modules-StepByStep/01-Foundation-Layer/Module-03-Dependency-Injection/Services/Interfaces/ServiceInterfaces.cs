namespace AI.DependencyInjection.Services.Interfaces;

/// <summary>
/// User service interface for managing user operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<User?> GetUserByIdAsync(int userId);

    /// <summary>
    /// Get all users
    /// </summary>
    Task<IEnumerable<User>> GetAllUsersAsync();

    /// <summary>
    /// Create a new user
    /// </summary>
    Task<User> CreateUserAsync(CreateUserRequest request);

    /// <summary>
    /// Update an existing user
    /// </summary>
    Task<User?> UpdateUserAsync(int userId, UpdateUserRequest request);

    /// <summary>
    /// Delete a user
    /// </summary>
    Task<bool> DeleteUserAsync(int userId);

    /// <summary>
    /// Search users by criteria
    /// </summary>
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
}

/// <summary>
/// Email service interface for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send an email message
    /// </summary>
    Task<bool> SendEmailAsync(EmailMessage message);

    /// <summary>
    /// Send bulk emails
    /// </summary>
    Task<BulkEmailResult> SendBulkEmailAsync(IEnumerable<EmailMessage> messages);

    /// <summary>
    /// Validate email address
    /// </summary>
    bool IsValidEmailAddress(string email);

    /// <summary>
    /// Get email sending statistics
    /// </summary>
    Task<EmailStatistics> GetStatisticsAsync();
}

/// <summary>
/// Logger service interface for logging operations
/// </summary>
public interface ILoggerService
{
    /// <summary>
    /// Log information message
    /// </summary>
    void LogInformation(string message);

    /// <summary>
    /// Log warning message
    /// </summary>
    void LogWarning(string message);

    /// <summary>
    /// Log error message
    /// </summary>
    void LogError(string message, Exception? exception = null);

    /// <summary>
    /// Log debug message
    /// </summary>
    void LogDebug(string message);

    /// <summary>
    /// Log with structured data
    /// </summary>
    void LogStructured(LogLevel level, string message, object? data = null);
}

/// <summary>
/// Data service interface for data operations
/// </summary>
public interface IDataService
{
    /// <summary>
    /// Get data by key
    /// </summary>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Set data with key
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Delete data by key
    /// </summary>
    Task<bool> DeleteAsync(string key);

    /// <summary>
    /// Check if key exists
    /// </summary>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Get all keys matching pattern
    /// </summary>
    Task<IEnumerable<string>> GetKeysAsync(string pattern = "*");

    /// <summary>
    /// Clear all data
    /// </summary>
    Task ClearAsync();
}

/// <summary>
/// Cache service interface extending data service
/// </summary>
public interface ICacheService : IDataService
{
    /// <summary>
    /// Get or set cached value
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Invalidate cache entries by tags
    /// </summary>
    Task InvalidateByTagAsync(string tag);

    /// <summary>
    /// Get cache statistics
    /// </summary>
    Task<CacheStatistics> GetStatisticsAsync();
}

/// <summary>
/// Repository interface for generic data access
/// </summary>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get entity by ID
    /// </summary>
    Task<T?> GetByIdAsync(object id);

    /// <summary>
    /// Get all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Add new entity
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Update existing entity
    /// </summary>
    Task<T?> UpdateAsync(T entity);

    /// <summary>
    /// Delete entity
    /// </summary>
    Task<bool> DeleteAsync(object id);

    /// <summary>
    /// Find entities by expression
    /// </summary>
    Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
}

/// <summary>
/// Notification service interface
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Send notification to user
    /// </summary>
    Task<bool> SendNotificationAsync(int userId, string message, NotificationType type = NotificationType.Info);

    /// <summary>
    /// Send bulk notifications
    /// </summary>
    Task<BulkNotificationResult> SendBulkNotificationAsync(IEnumerable<NotificationRequest> requests);

    /// <summary>
    /// Get user's notification preferences
    /// </summary>
    Task<NotificationPreferences> GetPreferencesAsync(int userId);

    /// <summary>
    /// Update user's notification preferences
    /// </summary>
    Task<bool> UpdatePreferencesAsync(int userId, NotificationPreferences preferences);
}