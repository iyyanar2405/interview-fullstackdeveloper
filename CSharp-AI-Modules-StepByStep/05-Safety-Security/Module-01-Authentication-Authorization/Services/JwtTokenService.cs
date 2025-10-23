using Microsoft.IdentityModel.Tokens;
using Module_01_Authentication_Authorization.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Module_01_Authentication_Authorization.Services;

public interface IJwtTokenService
{
    Task<JwtToken> GenerateAccessTokenAsync(User user);
    Task<RefreshToken> GenerateRefreshTokenAsync(User user, string? ipAddress, string? userAgent);
    Task<TokenValidationResult> ValidateTokenAsync(string token);
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
    Task<bool> RevokeRefreshTokenAsync(string token);
    Task<bool> RevokeAllUserTokensAsync(Guid userId);
    Task<List<RefreshToken>> GetUserActiveTokensAsync(Guid userId);
    string GenerateSecureToken(int length = 32);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly ILogger<JwtTokenService> _logger;
    private readonly JwtConfig _config;
    private readonly List<RefreshToken> _refreshTokens; // In production, use database

    public JwtTokenService(ILogger<JwtTokenService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _config = configuration.GetSection("Jwt").Get<JwtConfig>()
            ?? throw new InvalidOperationException("JWT configuration not found");
        _refreshTokens = new List<RefreshToken>();
    }

    public Task<JwtToken> GenerateAccessTokenAsync(User user)
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.Username),
                new Claim("email_verified", user.IsEmailVerified.ToString()),
                new Claim("mfa_enabled", user.MfaEnabled.ToString())
            };

            // Add roles
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add permissions
            foreach (var permission in user.Permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var issuedAt = DateTime.UtcNow;
            var expiresAt = issuedAt.AddMinutes(_config.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: claims,
                notBefore: issuedAt,
                expires: expiresAt,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("Generated access token for user {UserId}", user.Id);

            return Task.FromResult(new JwtToken
            {
                Token = tokenString,
                IssuedAt = issuedAt,
                ExpiresAt = expiresAt,
                TokenType = "Bearer"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating access token for user {UserId}", user.Id);
            throw;
        }
    }

    public Task<RefreshToken> GenerateRefreshTokenAsync(User user, string? ipAddress, string? userAgent)
    {
        try
        {
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateSecureToken(64),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_config.RefreshTokenExpirationDays),
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            _refreshTokens.Add(refreshToken);

            _logger.LogInformation("Generated refresh token for user {UserId}", user.Id);

            return Task.FromResult(refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating refresh token for user {UserId}", user.Id);
            throw;
        }
    }

    public Task<TokenValidationResult> ValidateTokenAsync(string token)
    {
        var result = new TokenValidationResult();

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = _config.ValidateIssuer,
                ValidateAudience = _config.ValidateAudience,
                ValidateLifetime = _config.ValidateLifetime,
                ValidateIssuerSigningKey = _config.ValidateIssuerSigningKey,
                ValidIssuer = _config.Issuer,
                ValidAudience = _config.Audience,
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtToken)
            {
                result.IsValid = true;
                result.UserId = Guid.Parse(principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty);
                result.Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                
                foreach (var claim in principal.Claims)
                {
                    result.Claims[claim.Type] = claim.Value;
                }

                _logger.LogDebug("Token validated successfully for user {UserId}", result.UserId);
            }
        }
        catch (SecurityTokenExpiredException)
        {
            result.IsValid = false;
            result.ErrorMessage = "Token has expired";
            _logger.LogWarning("Token validation failed: Token expired");
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            result.IsValid = false;
            result.ErrorMessage = "Invalid token signature";
            _logger.LogWarning("Token validation failed: Invalid signature");
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.ErrorMessage = $"Token validation failed: {ex.Message}";
            _logger.LogError(ex, "Error validating token");
        }

        return Task.FromResult(result);
    }

    public Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        try
        {
            var refreshToken = _refreshTokens.FirstOrDefault(rt =>
                rt.Token == token &&
                !rt.IsRevoked &&
                rt.ExpiresAt > DateTime.UtcNow);

            if (refreshToken == null)
            {
                _logger.LogWarning("Invalid or expired refresh token");
                return Task.FromResult<RefreshToken?>(null);
            }

            _logger.LogDebug("Refresh token validated successfully for user {UserId}", refreshToken.UserId);
            return Task.FromResult<RefreshToken?>(refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating refresh token");
            return Task.FromResult<RefreshToken?>(null);
        }
    }

    public Task<bool> RevokeRefreshTokenAsync(string token)
    {
        try
        {
            var refreshToken = _refreshTokens.FirstOrDefault(rt => rt.Token == token);
            
            if (refreshToken == null)
            {
                return Task.FromResult(false);
            }

            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;

            _logger.LogInformation("Revoked refresh token for user {UserId}", refreshToken.UserId);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token");
            return Task.FromResult(false);
        }
    }

    public Task<bool> RevokeAllUserTokensAsync(Guid userId)
    {
        try
        {
            var userTokens = _refreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked).ToList();

            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            _logger.LogInformation("Revoked all refresh tokens for user {UserId}, count: {Count}",
                userId, userTokens.Count);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all user tokens for {UserId}", userId);
            return Task.FromResult(false);
        }
    }

    public Task<List<RefreshToken>> GetUserActiveTokensAsync(Guid userId)
    {
        try
        {
            var activeTokens = _refreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(rt => rt.CreatedAt)
                .ToList();

            return Task.FromResult(activeTokens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active tokens for user {UserId}", userId);
            return Task.FromResult(new List<RefreshToken>());
        }
    }

    public string GenerateSecureToken(int length = 32)
    {
        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
