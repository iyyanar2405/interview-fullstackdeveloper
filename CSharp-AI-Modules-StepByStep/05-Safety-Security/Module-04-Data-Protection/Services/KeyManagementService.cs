using System.Security.Cryptography;
using Module_04_Data_Protection.Models;

namespace Module_04_Data_Protection.Services;

public interface IKeyManagementService
{
    Task<KeyManagementResult> GenerateKeyAsync(string keyType, int keySize = 256);
    Task<string> GetKeyAsync(string keyId);
    Task<KeyRotationResult> RotateKeyAsync(string oldKeyId);
    Task<KeyManagementResult> GetKeyInfoAsync(string keyId);
    Task<bool> RevokeKeyAsync(string keyId);
    Task<KeyBackupResult> BackupKeyAsync(string keyId);
    Task<List<KeyManagementResult>> ListKeysAsync();
}

public class KeyManagementService : IKeyManagementService
{
    private readonly ILogger<KeyManagementService> _logger;
    private readonly Dictionary<string, KeyInfo> _keys;
    private readonly IAuditLoggingService _auditService;

    public KeyManagementService(
        ILogger<KeyManagementService> logger,
        IAuditLoggingService auditService)
    {
        _logger = logger;
        _auditService = auditService;
        _keys = new Dictionary<string, KeyInfo>();
    }

    public async Task<KeyManagementResult> GenerateKeyAsync(string keyType, int keySize = 256)
    {
        _logger.LogInformation("Generating new key. Type: {KeyType}, Size: {KeySize}", keyType, keySize);

        var keyId = Guid.NewGuid().ToString();
        byte[] keyData;

        if (keyType.ToUpper() == "RSA")
        {
            using var rsa = RSA.Create(keySize);
            keyData = rsa.ExportRSAPrivateKey();
        }
        else // AES
        {
            using var aes = Aes.Create();
            aes.KeySize = keySize;
            aes.GenerateKey();
            keyData = aes.Key;
        }

        var keyInfo = new KeyInfo
        {
            KeyId = keyId,
            KeyType = keyType,
            KeyData = keyData,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(90),
            Status = KeyStatus.Active
        };

        _keys[keyId] = keyInfo;

        await _auditService.LogAsync("KeyGeneration", null, keyId, $"Generated {keyType} key", AuditSeverity.Information);

        return new KeyManagementResult
        {
            KeyId = keyId,
            KeyType = keyType,
            CreatedAt = keyInfo.CreatedAt,
            ExpiresAt = keyInfo.ExpiresAt,
            Status = keyInfo.Status,
            Properties = new Dictionary<string, object>
            {
                ["KeySize"] = keySize,
                ["Algorithm"] = keyType
            }
        };
    }

    public async Task<string> GetKeyAsync(string keyId)
    {
        if (!_keys.ContainsKey(keyId))
        {
            throw new KeyNotFoundException($"Key with ID {keyId} not found");
        }

        var keyInfo = _keys[keyId];
        
        if (keyInfo.Status != KeyStatus.Active)
        {
            throw new InvalidOperationException($"Key {keyId} is not active (Status: {keyInfo.Status})");
        }

        if (keyInfo.ExpiresAt.HasValue && keyInfo.ExpiresAt.Value < DateTime.UtcNow)
        {
            throw new InvalidOperationException($"Key {keyId} has expired");
        }

        await _auditService.LogAsync("KeyAccess", null, keyId, "Key accessed", AuditSeverity.Information);

        return Convert.ToBase64String(keyInfo.KeyData);
    }

    public async Task<KeyRotationResult> RotateKeyAsync(string oldKeyId)
    {
        _logger.LogInformation("Rotating key: {OldKeyId}", oldKeyId);

        if (!_keys.ContainsKey(oldKeyId))
        {
            throw new KeyNotFoundException($"Key with ID {oldKeyId} not found");
        }

        var oldKeyInfo = _keys[oldKeyId];
        
        // Generate new key
        var newKeyResult = await GenerateKeyAsync(oldKeyInfo.KeyType, oldKeyInfo.KeyData.Length * 8);

        // Mark old key as expired
        oldKeyInfo.Status = KeyStatus.Expired;
        oldKeyInfo.RotatedAt = DateTime.UtcNow;
        oldKeyInfo.RotatedTo = newKeyResult.KeyId;

        await _auditService.LogAsync("KeyRotation", null, oldKeyId, 
            $"Key rotated to {newKeyResult.KeyId}", AuditSeverity.Warning);

        return new KeyRotationResult
        {
            OldKeyId = oldKeyId,
            NewKeyId = newKeyResult.KeyId,
            RotatedAt = DateTime.UtcNow,
            ReencryptedRecordsCount = 0, // In production, re-encrypt data with new key
            Success = true
        };
    }

    public async Task<KeyManagementResult> GetKeyInfoAsync(string keyId)
    {
        if (!_keys.ContainsKey(keyId))
        {
            throw new KeyNotFoundException($"Key with ID {keyId} not found");
        }

        var keyInfo = _keys[keyId];

        return await Task.FromResult(new KeyManagementResult
        {
            KeyId = keyInfo.KeyId,
            KeyType = keyInfo.KeyType,
            CreatedAt = keyInfo.CreatedAt,
            ExpiresAt = keyInfo.ExpiresAt,
            Status = keyInfo.Status,
            Properties = new Dictionary<string, object>
            {
                ["KeySize"] = keyInfo.KeyData.Length * 8,
                ["RotatedAt"] = keyInfo.RotatedAt?.ToString() ?? "Never",
                ["RotatedTo"] = keyInfo.RotatedTo ?? "N/A"
            }
        });
    }

    public async Task<bool> RevokeKeyAsync(string keyId)
    {
        if (!_keys.ContainsKey(keyId))
        {
            return false;
        }

        _keys[keyId].Status = KeyStatus.Revoked;
        
        await _auditService.LogAsync("KeyRevocation", null, keyId, "Key revoked", AuditSeverity.Critical);

        _logger.LogWarning("Key revoked: {KeyId}", keyId);
        
        return true;
    }

    public async Task<KeyBackupResult> BackupKeyAsync(string keyId)
    {
        if (!_keys.ContainsKey(keyId))
        {
            throw new KeyNotFoundException($"Key with ID {keyId} not found");
        }

        var keyInfo = _keys[keyId];
        var backupId = Guid.NewGuid().ToString();
        
        // In production, encrypt and store backup securely
        var backupLocation = $"backup://{backupId}";

        await _auditService.LogAsync("KeyBackup", null, keyId, $"Key backed up to {backupLocation}", AuditSeverity.Information);

        return new KeyBackupResult
        {
            BackupId = backupId,
            KeyId = keyId,
            BackupLocation = backupLocation,
            BackedUpAt = DateTime.UtcNow,
            Encrypted = true
        };
    }

    public async Task<List<KeyManagementResult>> ListKeysAsync()
    {
        return await Task.FromResult(_keys.Values.Select(k => new KeyManagementResult
        {
            KeyId = k.KeyId,
            KeyType = k.KeyType,
            CreatedAt = k.CreatedAt,
            ExpiresAt = k.ExpiresAt,
            Status = k.Status
        }).ToList());
    }

    private class KeyInfo
    {
        public string KeyId { get; set; } = string.Empty;
        public string KeyType { get; set; } = string.Empty;
        public byte[] KeyData { get; set; } = Array.Empty<byte>();
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public KeyStatus Status { get; set; }
        public DateTime? RotatedAt { get; set; }
        public string? RotatedTo { get; set; }
    }
}
