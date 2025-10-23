using Module_01_Authentication_Authorization.Models;

namespace Module_01_Authentication_Authorization.Services;

public interface IRbacService
{
    Task<bool> AssignRoleToUserAsync(Guid userId, string roleName);
    Task<bool> RemoveRoleFromUserAsync(Guid userId, string roleName);
    Task<bool> AssignPermissionToRoleAsync(string roleName, string permission);
    Task<bool> RemovePermissionFromRoleAsync(string roleName, string permission);
    Task<bool> AssignPermissionToUserAsync(Guid userId, string permission);
    Task<bool> RemovePermissionFromUserAsync(Guid userId, string permission);
    Task<PermissionCheck> CheckPermissionAsync(Guid userId, string permission);
    Task<PermissionCheck> CheckMultiplePermissionsAsync(Guid userId, List<string> permissions, bool requireAll = true);
    Task<bool> HasRoleAsync(Guid userId, string roleName);
    Task<List<Role>> GetAllRolesAsync();
    Task<Role?> GetRoleByNameAsync(string roleName);
    Task<bool> CreateRoleAsync(Role role);
    Task<bool> UpdateRoleAsync(Role role);
    Task<bool> DeleteRoleAsync(string roleName);
}

public class RbacService : IRbacService
{
    private readonly ILogger<RbacService> _logger;
    private readonly IAuthenticationService _authService;
    private readonly List<Role> _roles;

    public RbacService(
        ILogger<RbacService> logger,
        IAuthenticationService authService)
    {
        _logger = logger;
        _authService = authService;
        _roles = InitializeDefaultRoles();
    }

    public async Task<bool> AssignRoleToUserAsync(Guid userId, string roleName)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                _logger.LogWarning("Cannot assign role - user not found: {UserId}", userId);
                return false;
            }

            var role = await GetRoleByNameAsync(roleName);
            
            if (role == null)
            {
                _logger.LogWarning("Cannot assign role - role not found: {RoleName}", roleName);
                return false;
            }

            if (user.Roles.Contains(roleName))
            {
                _logger.LogInformation("User {UserId} already has role {RoleName}", userId, roleName);
                return true;
            }

            user.Roles.Add(roleName);

            // Add role permissions to user
            foreach (var permission in role.Permissions)
            {
                if (!user.Permissions.Contains(permission))
                {
                    user.Permissions.Add(permission);
                }
            }

            _logger.LogInformation("Role {RoleName} assigned to user {UserId}", roleName, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleName} to user {UserId}", roleName, userId);
            return false;
        }
    }

    public async Task<bool> RemoveRoleFromUserAsync(Guid userId, string roleName)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return false;
            }

            if (!user.Roles.Contains(roleName))
            {
                return true;
            }

            user.Roles.Remove(roleName);

            // Recalculate permissions after removing role
            await RecalculateUserPermissionsAsync(user);

            _logger.LogInformation("Role {RoleName} removed from user {UserId}", roleName, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleName} from user {UserId}", roleName, userId);
            return false;
        }
    }

    public async Task<bool> AssignPermissionToRoleAsync(string roleName, string permission)
    {
        try
        {
            var role = await GetRoleByNameAsync(roleName);
            
            if (role == null)
            {
                return false;
            }

            if (role.Permissions.Contains(permission))
            {
                return true;
            }

            role.Permissions.Add(permission);

            _logger.LogInformation("Permission {Permission} assigned to role {RoleName}", permission, roleName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permission {Permission} to role {RoleName}", permission, roleName);
            return false;
        }
    }

    public async Task<bool> RemovePermissionFromRoleAsync(string roleName, string permission)
    {
        try
        {
            var role = await GetRoleByNameAsync(roleName);
            
            if (role == null)
            {
                return false;
            }

            if (!role.Permissions.Contains(permission))
            {
                return true;
            }

            role.Permissions.Remove(permission);

            _logger.LogInformation("Permission {Permission} removed from role {RoleName}", permission, roleName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing permission {Permission} from role {RoleName}", permission, roleName);
            return false;
        }
    }

    public async Task<bool> AssignPermissionToUserAsync(Guid userId, string permission)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return false;
            }

            if (user.Permissions.Contains(permission))
            {
                return true;
            }

            user.Permissions.Add(permission);

            _logger.LogInformation("Permission {Permission} assigned to user {UserId}", permission, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permission {Permission} to user {UserId}", permission, userId);
            return false;
        }
    }

    public async Task<bool> RemovePermissionFromUserAsync(Guid userId, string permission)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return false;
            }

            if (!user.Permissions.Contains(permission))
            {
                return true;
            }

            user.Permissions.Remove(permission);

            _logger.LogInformation("Permission {Permission} removed from user {UserId}", permission, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing permission {Permission} from user {UserId}", permission, userId);
            return false;
        }
    }

    public async Task<PermissionCheck> CheckPermissionAsync(Guid userId, string permission)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return new PermissionCheck
                {
                    HasPermission = false,
                    Message = "User not found"
                };
            }

            if (!user.IsActive)
            {
                return new PermissionCheck
                {
                    HasPermission = false,
                    Message = "User account is not active"
                };
            }

            var hasPermission = user.Permissions.Contains(permission);

            return new PermissionCheck
            {
                HasPermission = hasPermission,
                Message = hasPermission ? "Permission granted" : "Permission denied",
                UserId = userId,
                Permission = permission,
                CheckedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission {Permission} for user {UserId}", permission, userId);
            return new PermissionCheck
            {
                HasPermission = false,
                Message = "Error checking permission"
            };
        }
    }

    public async Task<PermissionCheck> CheckMultiplePermissionsAsync(Guid userId, List<string> permissions, bool requireAll = true)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return new PermissionCheck
                {
                    HasPermission = false,
                    Message = "User not found"
                };
            }

            if (!user.IsActive)
            {
                return new PermissionCheck
                {
                    HasPermission = false,
                    Message = "User account is not active"
                };
            }

            bool hasPermission;
            string message;

            if (requireAll)
            {
                hasPermission = permissions.All(p => user.Permissions.Contains(p));
                message = hasPermission ? "All permissions granted" : "Some required permissions are missing";
            }
            else
            {
                hasPermission = permissions.Any(p => user.Permissions.Contains(p));
                message = hasPermission ? "At least one permission granted" : "No required permissions found";
            }

            return new PermissionCheck
            {
                HasPermission = hasPermission,
                Message = message,
                UserId = userId,
                CheckedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking multiple permissions for user {UserId}", userId);
            return new PermissionCheck
            {
                HasPermission = false,
                Message = "Error checking permissions"
            };
        }
    }

    public async Task<bool> HasRoleAsync(Guid userId, string roleName)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            return user != null && user.Roles.Contains(roleName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role {RoleName} for user {UserId}", roleName, userId);
            return false;
        }
    }

    public Task<List<Role>> GetAllRolesAsync()
    {
        return Task.FromResult(_roles.ToList());
    }

    public Task<Role?> GetRoleByNameAsync(string roleName)
    {
        var role = _roles.FirstOrDefault(r =>
            r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(role);
    }

    public Task<bool> CreateRoleAsync(Role role)
    {
        try
        {
            if (_roles.Any(r => r.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Role already exists: {RoleName}", role.Name);
                return Task.FromResult(false);
            }

            _roles.Add(role);
            _logger.LogInformation("Role created: {RoleName}", role.Name);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role {RoleName}", role.Name);
            return Task.FromResult(false);
        }
    }

    public Task<bool> UpdateRoleAsync(Role role)
    {
        try
        {
            var existingRole = _roles.FirstOrDefault(r =>
                r.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase));

            if (existingRole == null)
            {
                _logger.LogWarning("Role not found for update: {RoleName}", role.Name);
                return Task.FromResult(false);
            }

            existingRole.Description = role.Description;
            existingRole.Permissions = role.Permissions;

            _logger.LogInformation("Role updated: {RoleName}", role.Name);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleName}", role.Name);
            return Task.FromResult(false);
        }
    }

    public Task<bool> DeleteRoleAsync(string roleName)
    {
        try
        {
            var role = _roles.FirstOrDefault(r =>
                r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));

            if (role == null)
            {
                return Task.FromResult(true);
            }

            _roles.Remove(role);
            _logger.LogInformation("Role deleted: {RoleName}", roleName);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleName}", roleName);
            return Task.FromResult(false);
        }
    }

    private async Task RecalculateUserPermissionsAsync(User user)
    {
        var permissions = new HashSet<string>();

        // Add permissions from roles
        foreach (var roleName in user.Roles)
        {
            var role = await GetRoleByNameAsync(roleName);
            if (role != null)
            {
                foreach (var permission in role.Permissions)
                {
                    permissions.Add(permission);
                }
            }
        }

        user.Permissions = permissions.ToList();
    }

    private List<Role> InitializeDefaultRoles()
    {
        return new List<Role>
        {
            new Role
            {
                Name = "Admin",
                Description = "Administrator with full access",
                Permissions = new List<string>
                {
                    "users.read", "users.write", "users.delete",
                    "roles.read", "roles.write", "roles.delete",
                    "settings.read", "settings.write",
                    "audit.read", "system.manage"
                }
            },
            new Role
            {
                Name = "User",
                Description = "Standard user with basic access",
                Permissions = new List<string>
                {
                    "profile.read", "profile.write",
                    "content.read"
                }
            },
            new Role
            {
                Name = "Manager",
                Description = "Manager with elevated permissions",
                Permissions = new List<string>
                {
                    "users.read", "users.write",
                    "content.read", "content.write",
                    "reports.read"
                }
            },
            new Role
            {
                Name = "Guest",
                Description = "Guest user with read-only access",
                Permissions = new List<string>
                {
                    "content.read"
                }
            }
        };
    }
}
