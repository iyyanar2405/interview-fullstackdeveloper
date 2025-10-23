using Module_01_Authentication_Authorization.Models;
using OtpNet;
using QRCoder;
using System.Security.Cryptography;

namespace Module_01_Authentication_Authorization.Services;

public interface IMfaService
{
    Task<MfaSetupResponse> SetupTotpAsync(Guid userId);
    Task<bool> VerifyTotpCodeAsync(Guid userId, string code);
    Task<bool> VerifyBackupCodeAsync(Guid userId, string code);
    Task<bool> EnableMfaAsync(Guid userId, string verificationCode);
    Task<bool> DisableMfaAsync(Guid userId, string password);
    Task<List<string>> RegenerateBackupCodesAsync(Guid userId);
    string GenerateQrCodeImage(string totpUri);
}

public class MfaService : IMfaService
{
    private readonly ILogger<MfaService> _logger;
    private readonly IAuthenticationService _authService;
    private readonly TotpConfig _config;

    public MfaService(
        ILogger<MfaService> logger,
        IAuthenticationService authService,
        IConfiguration configuration)
    {
        _logger = logger;
        _authService = authService;
        _config = configuration.GetSection("Totp").Get<TotpConfig>() ?? new TotpConfig();
    }

    public async Task<MfaSetupResponse> SetupTotpAsync(Guid userId)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return new MfaSetupResponse
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Generate secret key
            var key = KeyGeneration.GenerateRandomKey(20);
            var base32Secret = Base32Encoding.ToString(key);

            // Store secret temporarily (in production, encrypt this)
            user.MfaSecret = base32Secret;

            // Generate backup codes
            var backupCodes = GenerateBackupCodes();
            user.MfaBackupCodes = backupCodes.Select(HashBackupCode).ToList();

            // Generate TOTP URI for QR code
            var totpUri = $"otpauth://totp/{_config.Issuer}:{user.Email}?secret={base32Secret}&issuer={_config.Issuer}&digits={_config.Digits}&period={_config.Period}";

            var qrCodeUrl = GenerateQrCodeImage(totpUri);

            _logger.LogInformation("MFA setup initiated for user: {UserId}", userId);

            return new MfaSetupResponse
            {
                Success = true,
                Message = "Scan the QR code with your authenticator app",
                Secret = base32Secret,
                QrCodeUrl = qrCodeUrl,
                BackupCodes = backupCodes
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up MFA for user: {UserId}", userId);
            return new MfaSetupResponse
            {
                Success = false,
                Message = "An error occurred during MFA setup"
            };
        }
    }

    public async Task<bool> VerifyTotpCodeAsync(Guid userId, string code)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null || string.IsNullOrEmpty(user.MfaSecret))
            {
                return false;
            }

            var secretBytes = Base32Encoding.ToBytes(user.MfaSecret);
            var totp = new Totp(secretBytes, step: _config.Period, totpSize: _config.Digits);

            var isValid = totp.VerifyTotp(code, out long timeStepMatched, new VerificationWindow(2, 2));

            if (isValid)
            {
                _logger.LogInformation("TOTP code verified successfully for user: {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("Invalid TOTP code for user: {UserId}", userId);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying TOTP code for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> VerifyBackupCodeAsync(Guid userId, string code)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null || user.MfaBackupCodes.Count == 0)
            {
                return false;
            }

            var hashedCode = HashBackupCode(code);
            
            if (user.MfaBackupCodes.Contains(hashedCode))
            {
                // Remove used backup code
                user.MfaBackupCodes.Remove(hashedCode);
                
                _logger.LogInformation("Backup code used successfully for user: {UserId}", userId);
                return true;
            }

            _logger.LogWarning("Invalid backup code for user: {UserId}", userId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying backup code for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> EnableMfaAsync(Guid userId, string verificationCode)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return false;
            }

            // Verify the code before enabling
            var isValid = await VerifyTotpCodeAsync(userId, verificationCode);
            
            if (!isValid)
            {
                return false;
            }

            user.MfaEnabled = true;
            
            _logger.LogInformation("MFA enabled for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling MFA for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> DisableMfaAsync(Guid userId, string password)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return false;
            }

            // Verify password before disabling MFA
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Failed MFA disable attempt for user: {UserId}", userId);
                return false;
            }

            user.MfaEnabled = false;
            user.MfaSecret = null;
            user.MfaBackupCodes.Clear();

            _logger.LogInformation("MFA disabled for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling MFA for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<List<string>> RegenerateBackupCodesAsync(Guid userId)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return new List<string>();
            }

            var backupCodes = GenerateBackupCodes();
            user.MfaBackupCodes = backupCodes.Select(HashBackupCode).ToList();

            _logger.LogInformation("Backup codes regenerated for user: {UserId}", userId);
            return backupCodes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating backup codes for user: {UserId}", userId);
            return new List<string>();
        }
    }

    public string GenerateQrCodeImage(string totpUri)
    {
        try
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(totpUri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            return Convert.ToBase64String(qrCodeImage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating QR code image");
            return string.Empty;
        }
    }

    private List<string> GenerateBackupCodes()
    {
        var codes = new List<string>();
        
        for (int i = 0; i < _config.BackupCodeCount; i++)
        {
            var code = GenerateRandomCode(_config.BackupCodeLength);
            codes.Add(code);
        }

        return codes;
    }

    private string GenerateRandomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        var code = new char[length];
        for (int i = 0; i < length; i++)
        {
            code[i] = chars[bytes[i] % chars.Length];
        }

        return new string(code);
    }

    private string HashBackupCode(string code)
    {
        return BCrypt.Net.BCrypt.HashPassword(code);
    }
}
