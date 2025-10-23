using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Module_01_Authentication_Authorization.Models;
using Module_01_Authentication_Authorization.Services;

namespace Module_01_Authentication_Authorization.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IAuthenticationService _authService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IOAuthService _oauthService;
    private readonly IMfaService _mfaService;
    private readonly IRbacService _rbacService;
    private readonly IApiKeyService _apiKeyService;

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        IAuthenticationService authService,
        IJwtTokenService jwtTokenService,
        IOAuthService oauthService,
        IMfaService mfaService,
        IRbacService rbacService,
        IApiKeyService apiKeyService)
    {
        _logger = logger;
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _oauthService = oauthService;
        _mfaService = mfaService;
        _rbacService = rbacService;
        _apiKeyService = apiKeyService;
    }

    #region Authentication

    [HttpPost("register")]
    public async Task<ActionResult<AuthenticationResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] LoginRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers["User-Agent"].ToString();

        var result = await _authService.LoginAsync(request, ipAddress, userAgent);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthenticationResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers["User-Agent"].ToString();

        var result = await _authService.RefreshTokenAsync(request.RefreshToken, ipAddress, userAgent);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromBody] LogoutRequest? request)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? string.Empty);
        var refreshToken = request?.RefreshToken;

        var success = await _authService.LogoutAsync(userId, refreshToken);

        if (!success)
        {
            return BadRequest(new { Message = "Logout failed" });
        }

        return Ok(new { Message = "Logged out successfully" });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? string.Empty);

        var success = await _authService.ChangePasswordAsync(userId, request);

        if (!success)
        {
            return BadRequest(new { Message = "Password change failed" });
        }

        return Ok(new { Message = "Password changed successfully" });
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authService.RequestPasswordResetAsync(request.Email);

        return Ok(new { Message = "If the email exists, a reset link has been sent" });
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] PasswordResetConfirm request)
    {
        var success = await _authService.ResetPasswordAsync(request);

        if (!success)
        {
            return BadRequest(new { Message = "Password reset failed" });
        }

        return Ok(new { Message = "Password reset successfully" });
    }

    #endregion

    #region OAuth

    [HttpPost("oauth/google")]
    public async Task<ActionResult<AuthenticationResponse>> GoogleAuth([FromBody] OAuthCallbackRequest request)
    {
        var result = await _oauthService.AuthenticateWithGoogleAsync(request.Code, request.RedirectUri);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("oauth/microsoft")]
    public async Task<ActionResult<AuthenticationResponse>> MicrosoftAuth([FromBody] OAuthCallbackRequest request)
    {
        var result = await _oauthService.AuthenticateWithMicrosoftAsync(request.Code, request.RedirectUri);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("oauth/github")]
    public async Task<ActionResult<AuthenticationResponse>> GitHubAuth([FromBody] OAuthCallbackRequest request)
    {
        var result = await _oauthService.AuthenticateWithGitHubAsync(request.Code, request.RedirectUri);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("oauth/providers")]
    public async Task<ActionResult<List<ExternalLogin>>> GetOAuthProviders()
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? string.Empty);
        var providers = await _oauthService.GetUserOAuthProvidersAsync(userId);

        return Ok(providers);
    }

    [Authorize]
    [HttpPost("oauth/link")]
    public async Task<ActionResult> LinkOAuthProvider([FromBody] LinkOAuthRequest request)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? string.Empty);
        var success = await _oauthService.LinkOAuthProviderAsync(userId, request.Provider, request.ProviderUserId);

        if (!success)
        {
            return BadRequest(new { Message = "Failed to link OAuth provider" });
        }

        return Ok(new { Message = "OAuth provider linked successfully" });
    }

    [Authorize]
    [HttpPost("oauth/unlink")]
    public async Task<ActionResult> UnlinkOAuthProvider([FromBody] UnlinkOAuthRequest request)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? string.Empty);
        var success = await _oauthService.UnlinkOAuthProviderAsync(userId, request.Provider);

        if (!success)
        {
            return BadRequest(new { Message = "Failed to unlink OAuth provider" });
        }

        return Ok(new { Message = "OAuth provider unlinked successfully" });
    }

    #endregion

    #region MFA

    [Authorize]
    [HttpPost("mfa/setup")]
    public async Task<ActionResult<MfaSetupResponse>> SetupMfa()
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? string.Empty);
        var result = await _mfaService.SetupTotpAsync(userId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize]
    [HttpPost("mfa/enable")]
    public async Task<ActionResult> EnableMfa([FromBody] MfaVerifyRequest request)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? string.Empty);
        var success = await _mfaService.EnableMfaAsync(userId, request.Code);

        if (!success)
        {
            return BadRequest(new { Message = "Failed to enable MFA. Invalid verification code." });
        }

        return Ok(new { Message = "MFA enabled successfully" });
    }

    [Authorize]
    [HttpPost("mfa/disable")]
    public async Task<ActionResult> DisableMfa([FromBody] DisableMfaRequest request)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? string.Empty);
        var success = await _mfaService.DisableMfaAsync(userId, request.Password);

        if (!success)
        {
            return BadRequest(new { Message = "Failed to disable MFA. Invalid password." });
        }

        return Ok(new { Message = "MFA disabled successfully" });
    }

    [Authorize]
    [HttpPost("mfa/regenerate-backup-codes")]
    public async Task<ActionResult<List<string>>> RegenerateBackupCodes()
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? string.Empty);
        var codes = await _mfaService.RegenerateBackupCodesAsync(userId);

        return Ok(new { BackupCodes = codes });
    }

    #endregion

    #region RBAC

    [Authorize(Roles = "Admin")]
    [HttpPost("roles/assign")]
    public async Task<ActionResult> AssignRole([FromBody] RoleAssignmentRequest request)
    {
        var success = await _rbacService.AssignRoleToUserAsync(request.UserId, request.RoleName);

        if (!success)
        {
            return BadRequest(new { Message = "Failed to assign role" });
        }

        return Ok(new { Message = "Role assigned successfully" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("roles/remove")]
    public async Task<ActionResult> RemoveRole([FromBody] RoleAssignmentRequest request)
    {
        var success = await _rbacService.RemoveRoleFromUserAsync(request.UserId, request.RoleName);

        if (!success)
        {
            return BadRequest(new { Message = "Failed to remove role" });
        }

        return Ok(new { Message = "Role removed successfully" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("roles")]
    public async Task<ActionResult<List<Role>>> GetAllRoles()
    {
        var roles = await _rbacService.GetAllRolesAsync();
        return Ok(roles);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("permissions/assign")]
    public async Task<ActionResult> AssignPermission([FromBody] PermissionAssignmentRequest request)
    {
        var success = await _rbacService.AssignPermissionToUserAsync(request.UserId, request.Permission);

        if (!success)
        {
            return BadRequest(new { Message = "Failed to assign permission" });
        }

        return Ok(new { Message = "Permission assigned successfully" });
    }

    [Authorize]
    [HttpPost("permissions/check")]
    public async Task<ActionResult<PermissionCheck>> CheckPermission([FromBody] CheckPermissionRequest request)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? string.Empty);
        var result = await _rbacService.CheckPermissionAsync(userId, request.Permission);

        return Ok(result);
    }

    #endregion

    #region API Keys

    [Authorize]
    [HttpPost("api-keys")]
    public async Task<ActionResult<ApiKeyResponse>> CreateApiKey([FromBody] ApiKeyCreateRequest request)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? string.Empty);
        var result = await _apiKeyService.CreateApiKeyAsync(userId, request);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("api-keys")]
    public async Task<ActionResult<List<ApiKey>>> GetApiKeys()
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? string.Empty);
        var apiKeys = await _apiKeyService.GetUserApiKeysAsync(userId);

        // Don't return sensitive data
        foreach (var key in apiKeys)
        {
            key.KeyHash = string.Empty;
        }

        return Ok(apiKeys);
    }

    [Authorize]
    [HttpPost("api-keys/{id}/revoke")]
    public async Task<ActionResult> RevokeApiKey(Guid id)
    {
        var success = await _apiKeyService.RevokeApiKeyAsync(id);

        if (!success)
        {
            return BadRequest(new { Message = "Failed to revoke API key" });
        }

        return Ok(new { Message = "API key revoked successfully" });
    }

    [Authorize]
    [HttpPost("api-keys/{id}/rotate")]
    public async Task<ActionResult> RotateApiKey(Guid id)
    {
        var success = await _apiKeyService.RotateApiKeyAsync(id);

        if (!success)
        {
            return BadRequest(new { Message = "Failed to rotate API key" });
        }

        return Ok(new { Message = "API key rotated successfully" });
    }

    #endregion

    #region Helper Models

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class LogoutRequest
    {
        public string? RefreshToken { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class OAuthCallbackRequest
    {
        public string Code { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
    }

    public class LinkOAuthRequest
    {
        public string Provider { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
    }

    public class UnlinkOAuthRequest
    {
        public string Provider { get; set; } = string.Empty;
    }

    public class DisableMfaRequest
    {
        public string Password { get; set; } = string.Empty;
    }

    public class RoleAssignmentRequest
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    public class PermissionAssignmentRequest
    {
        public Guid UserId { get; set; }
        public string Permission { get; set; } = string.Empty;
    }

    public class CheckPermissionRequest
    {
        public string Permission { get; set; } = string.Empty;
    }

    #endregion
}
