using AI.DependencyInjection.Models;
using AI.DependencyInjection.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.DependencyInjection.Controllers;

/// <summary>
/// Users API controller demonstrating dependency injection patterns
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<User>), 200)]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        _logger.LogInformation("Getting all users");
        
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, new { message = "An error occurred while retrieving users" });
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        _logger.LogInformation("Getting user with ID: {UserId}", id);
        
        try
        {
            var user = await _userService.GetUserAsync(id);
            return Ok(user);
        }
        catch (UserNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found: {UserId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid user ID: {UserId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user: {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the user" });
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request">Create user request</param>
    /// <returns>Created user</returns>
    [HttpPost]
    [ProducesResponseType(typeof(User), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)
    {
        _logger.LogInformation("Creating new user with email: {Email}", request?.Email);
        
        try
        {
            if (request == null)
                return BadRequest(new { message = "Request body is required" });

            var user = await _userService.CreateUserAsync(request);
            
            return CreatedAtAction(
                nameof(GetUser), 
                new { id = user.Id }, 
                user);
        }
        catch (UserAlreadyExistsException ex)
        {
            _logger.LogWarning(ex, "User already exists: {Email}", request?.Email);
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid create user request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, new { message = "An error occurred while creating the user" });
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Update user request</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        _logger.LogInformation("Updating user with ID: {UserId}", id);
        
        try
        {
            if (request == null)
                return BadRequest(new { message = "Request body is required" });

            var user = await _userService.UpdateUserAsync(id, request);
            return Ok(user);
        }
        catch (UserNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found: {UserId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid update user request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the user" });
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        _logger.LogInformation("Deleting user with ID: {UserId}", id);
        
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            
            if (!result)
                return NotFound(new { message = $"User with ID {id} not found" });

            return NoContent();
        }
        catch (UserNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found: {UserId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid user ID: {UserId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the user" });
        }
    }

    /// <summary>
    /// Validate user data
    /// </summary>
    /// <param name="user">User to validate</param>
    /// <returns>Validation result</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> ValidateUser([FromBody] User user)
    {
        _logger.LogInformation("Validating user data");
        
        try
        {
            if (user == null)
                return BadRequest(new { message = "User data is required" });

            var isValid = await _userService.ValidateUserAsync(user);
            
            return Ok(new 
            { 
                isValid, 
                message = isValid ? "User data is valid" : "User data is invalid",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user");
            return StatusCode(500, new { message = "An error occurred while validating the user" });
        }
    }
}