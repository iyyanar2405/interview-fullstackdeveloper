using System.Security.Cryptography;
using System.Text;
using Module_04_Data_Protection.Models;

namespace Module_04_Data_Protection.Services;

public interface IEncryptionService
{
    Task<EncryptionResult> EncryptAsync(string data, EncryptionAlgorithm algorithm = EncryptionAlgorithm.AES256);
    Task<DecryptionResult> DecryptAsync(string encryptedData, string? iv, EncryptionAlgorithm algorithm = EncryptionAlgorithm.AES256);
    Task<EncryptionResult> EncryptWithKeyAsync(string data, string keyId);
    Task<DecryptionResult> DecryptWithKeyAsync(string encryptedData, string keyId, string? iv);
    Task<HashResult> HashAsync(string data, HashAlgorithm algorithm = HashAlgorithm.SHA256);
    Task<bool> VerifyHashAsync(string data, string hash, HashAlgorithm algorithm = HashAlgorithm.SHA256);
}

public class EncryptionService : IEncryptionService
{
    private readonly ILogger<EncryptionService> _logger;
    private readonly IKeyManagementService _keyManagement;
    private readonly Dictionary<string, byte[]> _keyCache;

    public EncryptionService(
        ILogger<EncryptionService> logger,
        IKeyManagementService keyManagement)
    {
        _logger = logger;
        _keyManagement = keyManagement;
        _keyCache = new Dictionary<string, byte[]>();
    }

    public async Task<EncryptionResult> EncryptAsync(string data, EncryptionAlgorithm algorithm = EncryptionAlgorithm.AES256)
    {
        _logger.LogInformation("Encrypting data with algorithm: {Algorithm}", algorithm);

        try
        {
            return algorithm switch
            {
                EncryptionAlgorithm.AES128 => await EncryptAesAsync(data, 128),
                EncryptionAlgorithm.AES192 => await EncryptAesAsync(data, 192),
                EncryptionAlgorithm.AES256 => await EncryptAesAsync(data, 256),
                EncryptionAlgorithm.RSA2048 => await EncryptRsaAsync(data, 2048),
                EncryptionAlgorithm.RSA4096 => await EncryptRsaAsync(data, 4096),
                _ => await EncryptAesAsync(data, 256)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encrypting data");
            throw;
        }
    }

    public async Task<DecryptionResult> DecryptAsync(string encryptedData, string? iv, EncryptionAlgorithm algorithm = EncryptionAlgorithm.AES256)
    {
        _logger.LogInformation("Decrypting data with algorithm: {Algorithm}", algorithm);

        try
        {
            return algorithm switch
            {
                EncryptionAlgorithm.AES128 or EncryptionAlgorithm.AES192 or EncryptionAlgorithm.AES256 
                    => await DecryptAesAsync(encryptedData, iv),
                EncryptionAlgorithm.RSA2048 or EncryptionAlgorithm.RSA4096 
                    => await DecryptRsaAsync(encryptedData),
                _ => await DecryptAesAsync(encryptedData, iv)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrypting data");
            return new DecryptionResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public async Task<EncryptionResult> EncryptWithKeyAsync(string data, string keyId)
    {
        var key = await _keyManagement.GetKeyAsync(keyId);
        
        // Use the key from key management
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(key);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            await sw.WriteAsync(data);
        }

        return new EncryptionResult
        {
            EncryptedData = Convert.ToBase64String(ms.ToArray()),
            Algorithm = "AES256",
            InitializationVector = Convert.ToBase64String(aes.IV),
            KeyId = keyId
        };
    }

    public async Task<DecryptionResult> DecryptWithKeyAsync(string encryptedData, string keyId, string? iv)
    {
        try
        {
            var key = await _keyManagement.GetKeyAsync(keyId);
            
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv ?? string.Empty);

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(encryptedData));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            var decrypted = await sr.ReadToEndAsync();

            return new DecryptionResult
            {
                DecryptedData = decrypted,
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new DecryptionResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public async Task<HashResult> HashAsync(string data, HashAlgorithm algorithm = HashAlgorithm.SHA256)
    {
        await Task.CompletedTask;

        return algorithm switch
        {
            HashAlgorithm.SHA256 => HashSha256(data),
            HashAlgorithm.SHA512 => HashSha512(data),
            HashAlgorithm.BCrypt => HashBCrypt(data),
            HashAlgorithm.Argon2 => HashArgon2(data),
            HashAlgorithm.PBKDF2 => HashPBKDF2(data),
            _ => HashSha256(data)
        };
    }

    public async Task<bool> VerifyHashAsync(string data, string hash, HashAlgorithm algorithm = HashAlgorithm.SHA256)
    {
        await Task.CompletedTask;

        return algorithm switch
        {
            HashAlgorithm.SHA256 or HashAlgorithm.SHA512 or HashAlgorithm.PBKDF2 => 
                (await HashAsync(data, algorithm)).Hash == hash,
            HashAlgorithm.BCrypt => BCrypt.Net.BCrypt.Verify(data, hash),
            HashAlgorithm.Argon2 => VerifyArgon2(data, hash),
            _ => false
        };
    }

    private async Task<EncryptionResult> EncryptAesAsync(string data, int keySize)
    {
        using var aes = Aes.Create();
        aes.KeySize = keySize;
        aes.GenerateKey();
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            await sw.WriteAsync(data);
        }

        // Store the key for later decryption (in production, use Key Management Service)
        var keyId = Guid.NewGuid().ToString();
        _keyCache[keyId] = aes.Key;

        return new EncryptionResult
        {
            EncryptedData = Convert.ToBase64String(ms.ToArray()),
            Algorithm = $"AES{keySize}",
            InitializationVector = Convert.ToBase64String(aes.IV),
            KeyId = keyId,
            Metadata = new Dictionary<string, string>
            {
                ["KeySize"] = keySize.ToString(),
                ["Mode"] = aes.Mode.ToString(),
                ["Padding"] = aes.Padding.ToString()
            }
        };
    }

    private async Task<DecryptionResult> DecryptAesAsync(string encryptedData, string? iv)
    {
        try
        {
            // In production, retrieve key from Key Management Service
            var keyId = _keyCache.Keys.LastOrDefault();
            if (keyId == null || !_keyCache.ContainsKey(keyId))
            {
                return new DecryptionResult
                {
                    Success = false,
                    Error = "Encryption key not found"
                };
            }

            using var aes = Aes.Create();
            aes.Key = _keyCache[keyId];
            aes.IV = Convert.FromBase64String(iv ?? string.Empty);

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(encryptedData));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            var decrypted = await sr.ReadToEndAsync();

            return new DecryptionResult
            {
                DecryptedData = decrypted,
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new DecryptionResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    private async Task<EncryptionResult> EncryptRsaAsync(string data, int keySize)
    {
        using var rsa = RSA.Create(keySize);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        var encryptedBytes = rsa.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA256);

        // Store the key for later decryption
        var keyId = Guid.NewGuid().ToString();
        var privateKey = rsa.ExportRSAPrivateKey();
        _keyCache[keyId] = privateKey;

        return await Task.FromResult(new EncryptionResult
        {
            EncryptedData = Convert.ToBase64String(encryptedBytes),
            Algorithm = $"RSA{keySize}",
            KeyId = keyId,
            Metadata = new Dictionary<string, string>
            {
                ["KeySize"] = keySize.ToString(),
                ["Padding"] = "OaepSHA256"
            }
        });
    }

    private async Task<DecryptionResult> DecryptRsaAsync(string encryptedData)
    {
        try
        {
            var keyId = _keyCache.Keys.LastOrDefault();
            if (keyId == null || !_keyCache.ContainsKey(keyId))
            {
                return new DecryptionResult
                {
                    Success = false,
                    Error = "Encryption key not found"
                };
            }

            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(_keyCache[keyId], out _);
            
            var encryptedBytes = Convert.FromBase64String(encryptedData);
            var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            var decrypted = Encoding.UTF8.GetString(decryptedBytes);

            return await Task.FromResult(new DecryptionResult
            {
                DecryptedData = decrypted,
                Success = true
            });
        }
        catch (Exception ex)
        {
            return new DecryptionResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    private HashResult HashSha256(string data)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        
        return new HashResult
        {
            Hash = Convert.ToBase64String(hashBytes),
            Algorithm = HashAlgorithm.SHA256
        };
    }

    private HashResult HashSha512(string data)
    {
        using var sha512 = SHA512.Create();
        var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(data));
        
        return new HashResult
        {
            Hash = Convert.ToBase64String(hashBytes),
            Algorithm = HashAlgorithm.SHA512
        };
    }

    private HashResult HashBCrypt(string data)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(data, workFactor: 12);
        
        return new HashResult
        {
            Hash = hash,
            Algorithm = HashAlgorithm.BCrypt
        };
    }

    private HashResult HashArgon2(string data)
    {
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using var argon2 = new Konscious.Security.Cryptography.Argon2id(Encoding.UTF8.GetBytes(data))
        {
            Salt = salt,
            DegreeOfParallelism = 8,
            Iterations = 4,
            MemorySize = 65536
        };

        var hashBytes = argon2.GetBytes(32);
        
        return new HashResult
        {
            Hash = Convert.ToBase64String(hashBytes),
            Salt = Convert.ToBase64String(salt),
            Algorithm = HashAlgorithm.Argon2
        };
    }

    private HashResult HashPBKDF2(string data)
    {
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using var pbkdf2 = new Rfc2898DeriveBytes(data, salt, 100000, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(32);
        
        return new HashResult
        {
            Hash = Convert.ToBase64String(hashBytes),
            Salt = Convert.ToBase64String(salt),
            Algorithm = HashAlgorithm.PBKDF2
        };
    }

    private bool VerifyArgon2(string data, string hash)
    {
        // Simplified verification - in production, extract salt from hash
        var hashResult = HashArgon2(data);
        return hashResult.Hash == hash;
    }
}
