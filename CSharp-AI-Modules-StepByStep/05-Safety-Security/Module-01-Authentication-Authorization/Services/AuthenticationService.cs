using BCrypt.Net;
using Module_01_Authentication_Authorization.Models;

namespace Module_01_Authentication_Authorization.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent);
    Task<AuthenticationResponse> RegisterAsync(RegisterRequest request);
    Task<AuthenticationResponse> RefreshTokenAsync(string refreshToken, string? ipAddress, string? userAgent);
    Task<bool> LogoutAsync(Guid userId, string? refreshToken);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<bool> RequestPasswordResetAsync(string email);
    Task<bool> ResetPasswordAsync(PasswordResetConfirm request);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMfaService _mfaService;
    private readonly List<User> _users; // In production, use database
    private readonly List<EmailVerificationToken> _verificationTokens;

    public AuthenticationService(
        ILogger<AuthenticationService> logger,
        IJwtTokenService jwtTokenService,
        IMfaService mfaService)
    {
        _logger = logger;
        _jwtTokenService = jwtTokenService;
        _mfaService = mfaService;
        _users = new List<User>();
        _verificationTokens = new List<EmailVerificationToken>();
    }

    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent)
    {
        try
        {
            var user = await GetUserByUsernameAsync(request.Username);

            if (user == null)
            {
                _logger.LogWarning("Login attempt for non-existent user: {Username}", request.Username);
                return new AuthenticationResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Check if account is locked
            if (user.IsLocked && user.LockoutEnd > DateTime.UtcNow)
            {
                _logger.LogWarning("Login attempt for locked account: {UserId}", user.Id);
                return new AuthenticationResponse
                {
                    Success = false,
                    Message = $"Account is locked until {user.LockoutEnd}"
                };
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;
                
                // Lock account after 5 failed attempts
                if (user.FailedLoginAttempts >= 5)
                {
                    user.IsLocked = true;
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
                    _logger.LogWarning("Account locked due to failed login attempts: {UserId}", user.Id);
                }

                return new AuthenticationResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Reset failed attempts on successful password verification
            user.FailedLoginAttempts = 0;
            user.IsLocked = false;
            user.LockoutEnd = null;

            // Check if MFA is required
            if (user.MfaEnabled)
            {
                if (string.IsNullOrEmpty(request.MfaCode))
                {
                    var mfaToken = _jwtTokenService.GenerateSecureToken(32);
                    
                    return new AuthenticationResponse
                    {
                        Success = false,
                        RequiresMfa = true,
                        MfaToken = mfaToken,
                        Message = "MFA code required"
                    };
                }

                var mfaValid = await _mfaService.VerifyTotpCodeAsync(user.Id, request.MfaCode);
                if (!mfaValid)
                {
                    return new AuthenticationResponse
                    {
                        Success = false,
                        Message = "Invalid MFA code"
                    };
                }
            }

            // Generate tokens
            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(user, ipAddress, userAgent);

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;

            _logger.LogInformation("User logged in successfully: {UserId}", user.Id);

            return new AuthenticationResponse
            {
                Success = true,
                Message = "Login successful",
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token,
                ExpiresAt = accessToken.ExpiresAt,
                User = MapToUserInfo(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Username}", request.Username);
            return new AuthenticationResponse
            {
                Success = false,
                Message = "An error occurred during login"
            };
        }
    }

    public async Task<AuthenticationResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if username exists
            if (await GetUserByUsernameAsync(request.Username) != null)
            {
                return new AuthenticationResponse
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }

            // Check if email exists
            if (await GetUserByEmailAsync(request.Email) != null)
            {
                return new AuthenticationResponse
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }

            // Create new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                IsEmailVerified = false,
                Roles = new List<string> { "User" },
                CreatedAt = DateTime.UtcNow
            };

            _users.Add(user);

            // Generate email verification token
            var verificationToken = new EmailVerificationToken
            {
                UserId = user.Id,
                Token = _jwtTokenService.GenerateSecureToken(32),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
            _verificationTokens.Add(verificationToken);

            _logger.LogInformation("User registered successfully: {UserId}", user.Id);

            // Generate tokens for immediate login
            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(user, null, null);

            return new AuthenticationResponse
            {
                Success = true,
                Message = "Registration successful. Please verify your email.",
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token,
                ExpiresAt = accessToken.ExpiresAt,
                User = MapToUserInfo(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return new AuthenticationResponse
            {
                Success = false,
                Message = "An error occurred during registration"
            };
        }
    }

    public async Task<AuthenticationResponse> RefreshTokenAsync(string refreshToken, string? ipAddress, string? userAgent)
    {
        try
        {
            var validToken = await _jwtTokenService.ValidateRefreshTokenAsync(refreshToken);
            
            if (validToken == null)
            {
                return new AuthenticationResponse
                {
                    Success = false,
                    Message = "Invalid or expired refresh token"
                };
            }

            var user = await GetUserByIdAsync(validToken.UserId);
            
            if (user == null || !user.IsActive)
            {
                return new AuthenticationResponse
                {
                    Success = false,
                    Message = "User not found or inactive"
                };
            }

            // Generate new tokens
            var newAccessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var newRefreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(user, ipAddress, userAgent);

            // Revoke old refresh token
            await _jwtTokenService.RevokeRefreshTokenAsync(refreshToken);

            _logger.LogInformation("Tokens refreshed for user: {UserId}", user.Id);

            return new AuthenticationResponse
            {
                Success = true,
                Message = "Tokens refreshed successfully",
                AccessToken = newAccessToken.Token,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = newAccessToken.ExpiresAt,
                User = MapToUserInfo(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing tokens");
            return new AuthenticationResponse
            {
                Success = false,
                Message = "An error occurred while refreshing tokens"
            };
        }
    }

    public async Task<bool> LogoutAsync(Guid userId, string? refreshToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _jwtTokenService.RevokeRefreshTokenAsync(refreshToken);
            }
            else
            {
                await _jwtTokenService.RevokeAllUserTokensAsync(userId);
            }

            _logger.LogInformation("User logged out: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        try
        {
            var user = await GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return false;
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                _logger.LogWarning("Failed password change attempt for user: {UserId}", userId);
                return false;
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // Revoke all refresh tokens for security
            await _jwtTokenService.RevokeAllUserTokensAsync(userId);

            _logger.LogInformation("Password changed successfully for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        try
        {
            var user = await GetUserByEmailAsync(email);
            
            if (user == null)
            {
                // Don't reveal if email exists
                _logger.LogWarning("Password reset requested for non-existent email");
                return true;
            }

            var resetToken = new EmailVerificationToken
            {
                UserId = user.Id,
                Token = _jwtTokenService.GenerateSecureToken(32),
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
            _verificationTokens.Add(resetToken);

            _logger.LogInformation("Password reset requested for user: {UserId}", user.Id);
            // In production, send email with reset link
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting password reset");
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(PasswordResetConfirm request)
    {
        try
        {
            var token = _verificationTokens.FirstOrDefault(t =>
                t.Token == request.Token &&
                !t.IsUsed &&
                t.ExpiresAt > DateTime.UtcNow);

            if (token == null)
            {
                _logger.LogWarning("Invalid or expired password reset token");
                return false;
            }

            var user = await GetUserByIdAsync(token.UserId);
            
            if (user == null)
            {
                return false;
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // Mark token as used
            token.IsUsed = true;

            // Revoke all refresh tokens
            await _jwtTokenService.RevokeAllUserTokensAsync(user.Id);

            _logger.LogInformation("Password reset successfully for user: {UserId}", user.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return false;
        }
    }

    public Task<User?> GetUserByIdAsync(Guid userId)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId);
        return Task.FromResult(user);
    }

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        var user = _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        var user = _users.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    private UserInfo MapToUserInfo(User user)
    {
        return new UserInfo
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = user.Roles,
            Permissions = user.Permissions,
            MfaEnabled = user.MfaEnabled
        };
    }
}
