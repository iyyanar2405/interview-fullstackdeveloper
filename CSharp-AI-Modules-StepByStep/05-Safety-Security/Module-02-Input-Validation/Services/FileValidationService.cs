using Module_02_Input_Validation.Models;
using MimeDetective;
using System.Security.Cryptography;

namespace Module_02_Input_Validation.Services;

public interface IFileValidationService
{
    FileValidationResult ValidateFile(byte[] fileContent, string fileName, string declaredMimeType, FileValidationRules? rules = null);
    bool IsAllowedExtension(string fileName, List<string> allowedExtensions);
    bool IsAllowedMimeType(string mimeType, List<string> allowedMimeTypes);
    string DetectMimeType(byte[] fileContent, string fileName);
    bool CheckFileSignature(byte[] fileContent, string fileName);
    string CalculateFileHash(byte[] fileContent);
    bool IsExecutable(string fileName);
}

public class FileValidationService : IFileValidationService
{
    private readonly ILogger<FileValidationService> _logger;
    private readonly ContentInspector _contentInspector;

    // File signatures (magic numbers) for common file types
    private static readonly Dictionary<string, byte[][]> FileSignatures = new()
    {
        { ".jpg", new[] { new byte[] { 0xFF, 0xD8, 0xFF } } },
        { ".jpeg", new[] { new byte[] { 0xFF, 0xD8, 0xFF } } },
        { ".png", new[] { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
        { ".gif", new[] { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
        { ".pdf", new[] { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
        { ".zip", new[] { new byte[] { 0x50, 0x4B, 0x03, 0x04 }, new byte[] { 0x50, 0x4B, 0x05, 0x06 } } },
        { ".doc", new[] { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
        { ".docx", new[] { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
        { ".xlsx", new[] { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
        { ".exe", new[] { new byte[] { 0x4D, 0x5A } } }
    };

    private static readonly List<string> ExecutableExtensions = new()
    {
        ".exe", ".bat", ".cmd", ".com", ".pif", ".scr", ".vbs", ".js", ".jar",
        ".msi", ".app", ".deb", ".rpm", ".sh", ".ps1", ".psm1", ".dll", ".so"
    };

    public FileValidationService(ILogger<FileValidationService> logger)
    {
        _logger = logger;
        _contentInspector = new ContentInspectorBuilder()
        {
            Definitions = MimeDetective.Definitions.Default.All()
        }.Build();
    }

    public FileValidationResult ValidateFile(byte[] fileContent, string fileName, string declaredMimeType, FileValidationRules? rules = null)
    {
        rules ??= new FileValidationRules();
        var result = new FileValidationResult
        {
            IsValid = true,
            FileName = fileName,
            DeclaredMimeType = declaredMimeType,
            FileSize = fileContent.Length
        };

        try
        {
            // Validate file name
            if (string.IsNullOrWhiteSpace(fileName))
            {
                result.IsValid = false;
                result.ValidationErrors.Add("File name is required");
                return result;
            }

            // Check for path traversal in file name
            if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
            {
                result.IsValid = false;
                result.ValidationErrors.Add("File name contains invalid path characters");
            }

            // Check file extension
            var extension = Path.GetExtension(fileName).ToLower();
            if (rules.AllowedExtensions.Count > 0 && !rules.AllowedExtensions.Contains(extension))
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"File extension '{extension}' is not allowed");
            }

            // Check if executable
            if (!rules.AllowExecutables && IsExecutable(fileName))
            {
                result.IsValid = false;
                result.ValidationErrors.Add("Executable files are not allowed");
            }

            // Check file size
            if (fileContent.Length == 0)
            {
                result.IsValid = false;
                result.ValidationErrors.Add("File is empty");
            }
            else if (fileContent.Length > rules.MaxFileSizeBytes)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"File size exceeds maximum allowed size of {rules.MaxFileSizeBytes / 1024 / 1024} MB");
            }

            // Detect actual MIME type
            if (rules.CheckMimeType)
            {
                result.DetectedMimeType = DetectMimeType(fileContent, fileName);

                if (rules.AllowedMimeTypes.Count > 0 && !rules.AllowedMimeTypes.Contains(result.DetectedMimeType))
                {
                    result.IsValid = false;
                    result.ValidationErrors.Add($"Detected MIME type '{result.DetectedMimeType}' is not allowed");
                }

                // Check for MIME type mismatch
                if (!string.IsNullOrEmpty(declaredMimeType) && 
                    !declaredMimeType.Equals(result.DetectedMimeType, StringComparison.OrdinalIgnoreCase))
                {
                    result.MimeTypeMismatch = true;
                    result.ValidationErrors.Add($"MIME type mismatch: declared '{declaredMimeType}' but detected '{result.DetectedMimeType}'");
                    result.IsValid = false;
                }
            }

            // Check file signature
            if (rules.CheckFileSignature)
            {
                if (!CheckFileSignature(fileContent, fileName))
                {
                    result.IsValid = false;
                    result.ValidationErrors.Add("File signature validation failed");
                }
            }

            // Calculate file hash
            var fileHash = CalculateFileHash(fileContent);
            result.FileProperties["SHA256"] = fileHash;
            result.FileProperties["Extension"] = extension;

            _logger.LogInformation("File validation {Result} for {FileName}. Errors: {ErrorCount}",
                result.IsValid ? "passed" : "failed", fileName, result.ValidationErrors.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating file {FileName}", fileName);
            result.IsValid = false;
            result.ValidationErrors.Add($"Validation error: {ex.Message}");
            return result;
        }
    }

    public bool IsAllowedExtension(string fileName, List<string> allowedExtensions)
    {
        if (allowedExtensions.Count == 0)
            return true;

        var extension = Path.GetExtension(fileName).ToLower();
        return allowedExtensions.Contains(extension);
    }

    public bool IsAllowedMimeType(string mimeType, List<string> allowedMimeTypes)
    {
        if (allowedMimeTypes.Count == 0)
            return true;

        return allowedMimeTypes.Any(allowed => 
            mimeType.Equals(allowed, StringComparison.OrdinalIgnoreCase));
    }

    public string DetectMimeType(byte[] fileContent, string fileName)
    {
        try
        {
            var results = _contentInspector.Inspect(fileContent);
            if (results.Any())
            {
                return results.First().Definition.File.MimeType;
            }

            // Fallback to extension-based detection
            return MimeMapping.MimeUtility.GetMimeMapping(fileName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error detecting MIME type for {FileName}, using extension-based detection", fileName);
            return MimeMapping.MimeUtility.GetMimeMapping(fileName);
        }
    }

    public bool CheckFileSignature(byte[] fileContent, string fileName)
    {
        if (fileContent.Length == 0)
            return false;

        var extension = Path.GetExtension(fileName).ToLower();

        if (!FileSignatures.ContainsKey(extension))
        {
            // No signature defined for this extension, consider it valid
            return true;
        }

        var signatures = FileSignatures[extension];

        foreach (var signature in signatures)
        {
            if (fileContent.Length < signature.Length)
                continue;

            var matches = true;
            for (int i = 0; i < signature.Length; i++)
            {
                if (fileContent[i] != signature[i])
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
                return true;
        }

        _logger.LogWarning("File signature mismatch for {FileName} with extension {Extension}", fileName, extension);
        return false;
    }

    public string CalculateFileHash(byte[] fileContent)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(fileContent);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    public bool IsExecutable(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return ExecutableExtensions.Contains(extension);
    }
}
