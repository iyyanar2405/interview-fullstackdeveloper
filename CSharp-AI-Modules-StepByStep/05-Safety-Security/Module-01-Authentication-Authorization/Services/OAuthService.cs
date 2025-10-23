using Module_01_Authentication_Authorization.Models;
using System.Text.Json;

namespace Module_01_Authentication_Authorization.Services;

public interface IOAuthService
{
    Task<AuthenticationResponse> AuthenticateWithGoogleAsync(string code, string redirectUri);
    Task<AuthenticationResponse> AuthenticateWithMicrosoftAsync(string code, string redirectUri);
    Task<AuthenticationResponse> AuthenticateWithGitHubAsync(string code, string redirectUri);
    Task<bool> LinkOAuthProviderAsync(Guid userId, string provider, string providerUserId);
    Task<bool> UnlinkOAuthProviderAsync(Guid userId, string provider);
    Task<List<ExternalLogin>> GetUserOAuthProvidersAsync(Guid userId);
}

public class OAuthService : IOAuthService
{
    private readonly ILogger<OAuthService> _logger;
    private readonly IAuthenticationService _authService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, OAuthProvider> _providers;

    public OAuthService(
        ILogger<OAuthService> logger,
        IAuthenticationService authService,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger;
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _httpClient = httpClient;

        // Load OAuth providers from configuration
        _providers = new Dictionary<string, OAuthProvider>
        {
            ["Google"] = configuration.GetSection("OAuth:Google").Get<OAuthProvider>() ?? new OAuthProvider(),
            ["Microsoft"] = configuration.GetSection("OAuth:Microsoft").Get<OAuthProvider>() ?? new OAuthProvider(),
            ["GitHub"] = configuration.GetSection("OAuth:GitHub").Get<OAuthProvider>() ?? new OAuthProvider()
        };
    }

    public async Task<AuthenticationResponse> AuthenticateWithGoogleAsync(string code, string redirectUri)
    {
        return await AuthenticateWithProviderAsync("Google", code, redirectUri);
    }

    public async Task<AuthenticationResponse> AuthenticateWithMicrosoftAsync(string code, string redirectUri)
    {
        return await AuthenticateWithProviderAsync("Microsoft", code, redirectUri);
    }

    public async Task<AuthenticationResponse> AuthenticateWithGitHubAsync(string code, string redirectUri)
    {
        return await AuthenticateWithProviderAsync("GitHub", code, redirectUri);
    }

    private async Task<AuthenticationResponse> AuthenticateWithProviderAsync(string providerName, string code, string redirectUri)
    {
        try
        {
            if (!_providers.TryGetValue(providerName, out var provider))
            {
                return new AuthenticationResponse
                {
                    Success = false,
                    Message = $"OAuth provider {providerName} not configured"
                };
            }

            // Exchange code for access token
            var tokenResponse = await ExchangeCodeForTokenAsync(provider, code, redirectUri);
            
            if (tokenResponse == null)
            {
                return new AuthenticationResponse
                {
                    Success = false,
                    Message = $"Failed to exchange code for {providerName} access token"
                };
            }

            // Get user info from provider
            var userInfo = await GetUserInfoAsync(provider, tokenResponse.AccessToken);
            
            if (userInfo == null)
            {
                return new AuthenticationResponse
                {
                    Success = false,
                    Message = $"Failed to get user info from {providerName}"
                };
            }

            // Find or create user
            var user = await FindOrCreateUserFromOAuthAsync(providerName, userInfo);

            // Generate JWT tokens
            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(user, null, null);

            _logger.LogInformation("User authenticated with {Provider}: {UserId}", providerName, user.Id);

            return new AuthenticationResponse
            {
                Success = true,
                Message = $"Authenticated with {providerName}",
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token,
                ExpiresAt = accessToken.ExpiresAt,
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = user.Roles,
                    Permissions = user.Permissions,
                    MfaEnabled = user.MfaEnabled
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating with {Provider}", providerName);
            return new AuthenticationResponse
            {
                Success = false,
                Message = $"An error occurred during {providerName} authentication"
            };
        }
    }

    private async Task<OAuthTokenResponse?> ExchangeCodeForTokenAsync(OAuthProvider provider, string code, string redirectUri)
    {
        try
        {
            var tokenRequest = new Dictionary<string, string>
            {
                ["client_id"] = provider.ClientId,
                ["client_secret"] = provider.ClientSecret,
                ["code"] = code,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code"
            };

            var response = await _httpClient.PostAsync(
                provider.TokenEndpoint,
                new FormUrlEncodedContent(tokenRequest));

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to exchange code for token. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<OAuthTokenResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return tokenResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exchanging code for token");
            return null;
        }
    }

    private async Task<OAuthUserInfo?> GetUserInfoAsync(OAuthProvider provider, string accessToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, provider.UserInfoEndpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get user info. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<OAuthUserInfo>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user info");
            return null;
        }
    }

    private async Task<User> FindOrCreateUserFromOAuthAsync(string provider, OAuthUserInfo oauthInfo)
    {
        // Try to find existing user by provider user ID
        var users = await GetAllUsersAsync(); // In production, use database query
        var existingUser = users.FirstOrDefault(u =>
            u.ExternalLogins.Any(el =>
                el.Provider == provider &&
                el.ProviderUserId == oauthInfo.Id));

        if (existingUser != null)
        {
            // Update last login
            existingUser.LastLoginAt = DateTime.UtcNow;
            return existingUser;
        }

        // Try to find by email
        var userByEmail = await _authService.GetUserByEmailAsync(oauthInfo.Email);
        
        if (userByEmail != null)
        {
            // Link OAuth provider to existing account
            userByEmail.ExternalLogins.Add(new ExternalLogin
            {
                Provider = provider,
                ProviderUserId = oauthInfo.Id,
                Email = oauthInfo.Email,
                DisplayName = oauthInfo.Name,
                LinkedAt = DateTime.UtcNow
            });
            
            return userByEmail;
        }

        // Create new user
        var newUser = new User
        {
            Username = GenerateUsernameFromEmail(oauthInfo.Email),
            Email = oauthInfo.Email,
            FirstName = oauthInfo.GivenName ?? "",
            LastName = oauthInfo.FamilyName ?? "",
            IsEmailVerified = oauthInfo.EmailVerified,
            Roles = new List<string> { "User" },
            ExternalLogins = new List<ExternalLogin>
            {
                new ExternalLogin
                {
                    Provider = provider,
                    ProviderUserId = oauthInfo.Id,
                    Email = oauthInfo.Email,
                    DisplayName = oauthInfo.Name,
                    LinkedAt = DateTime.UtcNow
                }
            },
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };

        // Add to in-memory list (in production, save to database)
        users.Add(newUser);

        _logger.LogInformation("New user created from {Provider} OAuth: {UserId}", provider, newUser.Id);
        return newUser;
    }

    public async Task<bool> LinkOAuthProviderAsync(Guid userId, string provider, string providerUserId)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return false;
            }

            // Check if already linked
            if (user.ExternalLogins.Any(el => el.Provider == provider))
            {
                _logger.LogWarning("OAuth provider {Provider} already linked to user: {UserId}", provider, userId);
                return false;
            }

            user.ExternalLogins.Add(new ExternalLogin
            {
                Provider = provider,
                ProviderUserId = providerUserId,
                LinkedAt = DateTime.UtcNow
            });

            _logger.LogInformation("OAuth provider {Provider} linked to user: {UserId}", provider, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking OAuth provider {Provider} to user: {UserId}", provider, userId);
            return false;
        }
    }

    public async Task<bool> UnlinkOAuthProviderAsync(Guid userId, string provider)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return false;
            }

            var externalLogin = user.ExternalLogins.FirstOrDefault(el => el.Provider == provider);
            
            if (externalLogin == null)
            {
                _logger.LogWarning("OAuth provider {Provider} not linked to user: {UserId}", provider, userId);
                return false;
            }

            user.ExternalLogins.Remove(externalLogin);

            _logger.LogInformation("OAuth provider {Provider} unlinked from user: {UserId}", provider, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlinking OAuth provider {Provider} from user: {UserId}", provider, userId);
            return false;
        }
    }

    public async Task<List<ExternalLogin>> GetUserOAuthProvidersAsync(Guid userId)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            return user?.ExternalLogins ?? new List<ExternalLogin>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting OAuth providers for user: {UserId}", userId);
            return new List<ExternalLogin>();
        }
    }

    private async Task<List<User>> GetAllUsersAsync()
    {
        // In production, this would be a database query
        // For now, we'll get users through reflection from AuthenticationService
        // This is a workaround for the in-memory demo
        return new List<User>();
    }

    private string GenerateUsernameFromEmail(string email)
    {
        var username = email.Split('@')[0];
        var random = new Random();
        return $"{username}{random.Next(1000, 9999)}";
    }
}
