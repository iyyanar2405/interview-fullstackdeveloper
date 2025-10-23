using AI.DependencyInjection.Models;
using AI.DependencyInjection.Patterns;

namespace AI.DependencyInjection.Services;

/// <summary>
/// User service interface
/// </summary>
public interface IUserService
{
    Task<User> GetUserAsync(int id);
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task<User> UpdateUserAsync(int id, UpdateUserRequest request);
    Task<bool> DeleteUserAsync(int id);
    Task<List<User>> GetAllUsersAsync();
    Task<bool> ValidateUserAsync(User user);
}

/// <summary>
/// User service implementation
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<UserService> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserService(
        IUserRepository userRepository,
        INotificationService notificationService,
        ILogger<UserService> logger,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    public async Task<User> GetUserAsync(int id)
    {
        _logger.LogInformation("Getting user with ID: {UserId}", id);

        if (id <= 0)
            throw new ArgumentException("User ID must be positive", nameof(id));

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            throw new UserNotFoundException($"User with ID {id} not found");
        }

        _logger.LogInformation("Successfully retrieved user: {UserId}", id);
        return user;
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        _logger.LogInformation("Creating new user with email: {Email}", request.Email);

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new UserAlreadyExistsException($"User with email {request.Email} already exists");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Age = request.Age,
            IsActive = true,
            CreatedAt = _dateTimeProvider.UtcNow,
            UpdatedAt = _dateTimeProvider.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(user);

        // Send welcome notification
        await _notificationService.SendWelcomeNotificationAsync(createdUser);

        _logger.LogInformation("Successfully created user: {UserId}", createdUser.Id);
        return createdUser;
    }

    public async Task<User> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        _logger.LogInformation("Updating user with ID: {UserId}", id);

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var user = await GetUserAsync(id);

        user.Name = request.Name ?? user.Name;
        user.Email = request.Email ?? user.Email;
        user.Age = request.Age ?? user.Age;
        user.UpdatedAt = _dateTimeProvider.UtcNow;

        var updatedUser = await _userRepository.UpdateAsync(user);

        _logger.LogInformation("Successfully updated user: {UserId}", id);
        return updatedUser;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        _logger.LogInformation("Deleting user with ID: {UserId}", id);

        var user = await GetUserAsync(id);
        var result = await _userRepository.DeleteAsync(id);

        if (result)
        {
            // Send farewell notification
            await _notificationService.SendFarewellNotificationAsync(user);
            _logger.LogInformation("Successfully deleted user: {UserId}", id);
        }

        return result;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        _logger.LogInformation("Getting all users");

        var users = await _userRepository.GetAllAsync();

        _logger.LogInformation("Retrieved {Count} users", users.Count);
        return users;
    }

    public async Task<bool> ValidateUserAsync(User user)
    {
        if (user == null)
            return false;

        // Basic validation
        if (string.IsNullOrWhiteSpace(user.Name) || 
            string.IsNullOrWhiteSpace(user.Email) ||
            user.Age <= 0)
        {
            return false;
        }

        // Check for duplicate email
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        return existingUser == null || existingUser.Id == user.Id;
    }
}

/// <summary>
/// Cached user service decorator
/// </summary>
public class CachedUserService : IUserService
{
    private readonly IUserService _innerService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedUserService> _logger;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(15);

    public CachedUserService(
        IUserService innerService,
        ICacheService cacheService,
        ILogger<CachedUserService> logger)
    {
        _innerService = innerService ?? throw new ArgumentNullException(nameof(innerService));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<User> GetUserAsync(int id)
    {
        var cacheKey = $"user:{id}";
        
        var cachedUser = await _cacheService.GetAsync<User>(cacheKey);
        if (cachedUser != null)
        {
            _logger.LogDebug("Cache hit for user: {UserId}", id);
            return cachedUser;
        }

        _logger.LogDebug("Cache miss for user: {UserId}", id);
        var user = await _innerService.GetUserAsync(id);
        
        await _cacheService.SetAsync(cacheKey, user, _cacheExpiry);
        return user;
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        var user = await _innerService.CreateUserAsync(request);
        
        // Cache the new user
        var cacheKey = $"user:{user.Id}";
        await _cacheService.SetAsync(cacheKey, user, _cacheExpiry);
        
        // Invalidate users list cache
        await _cacheService.RemoveAsync("users:all");
        
        return user;
    }

    public async Task<User> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        var user = await _innerService.UpdateUserAsync(id, request);
        
        // Update cache
        var cacheKey = $"user:{id}";
        await _cacheService.SetAsync(cacheKey, user, _cacheExpiry);
        
        // Invalidate users list cache
        await _cacheService.RemoveAsync("users:all");
        
        return user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var result = await _innerService.DeleteUserAsync(id);
        
        if (result)
        {
            // Remove from cache
            var cacheKey = $"user:{id}";
            await _cacheService.RemoveAsync(cacheKey);
            
            // Invalidate users list cache
            await _cacheService.RemoveAsync("users:all");
        }
        
        return result;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        const string cacheKey = "users:all";
        
        var cachedUsers = await _cacheService.GetAsync<List<User>>(cacheKey);
        if (cachedUsers != null)
        {
            _logger.LogDebug("Cache hit for all users");
            return cachedUsers;
        }

        _logger.LogDebug("Cache miss for all users");
        var users = await _innerService.GetAllUsersAsync();
        
        await _cacheService.SetAsync(cacheKey, users, _cacheExpiry);
        return users;
    }

    public async Task<bool> ValidateUserAsync(User user)
    {
        return await _innerService.ValidateUserAsync(user);
    }
}

/// <summary>
/// User not found exception
/// </summary>
public class UserNotFoundException : Exception
{
    public UserNotFoundException(string message) : base(message) { }
    public UserNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// User already exists exception
/// </summary>
public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string message) : base(message) { }
    public UserAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}