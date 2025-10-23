using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using Module_03_Content_Safety.Models;

namespace Module_03_Content_Safety.Services;

public interface IPiiDetectionService
{
    Task<PiiDetectionResult> DetectPiiAsync(string content);
    Task<PiiDetectionResult> DetectPiiAsync(string content, List<PiiType> piiTypes);
    Task<string> RedactPiiAsync(string content, RedactionStrategy strategy = RedactionStrategy.Replace);
    Task<string> MaskPiiAsync(string content);
    Task<bool> ContainsPiiAsync(string content);
}

public class PiiDetectionService : IPiiDetectionService
{
    private readonly ILogger<PiiDetectionService> _logger;
    private readonly Dictionary<PiiType, Regex> _piiPatterns;

    public PiiDetectionService(ILogger<PiiDetectionService> logger)
    {
        _logger = logger;
        _piiPatterns = InitializePiiPatterns();
    }

    public async Task<PiiDetectionResult> DetectPiiAsync(string content)
    {
        return await DetectPiiAsync(content, _piiPatterns.Keys.ToList());
    }

    public async Task<PiiDetectionResult> DetectPiiAsync(string content, List<PiiType> piiTypes)
    {
        _logger.LogInformation("Detecting PII in content. Length: {Length}", content.Length);

        var result = new PiiDetectionResult
        {
            DetectedEntities = new List<PiiEntity>(),
            PiiTypeCount = new Dictionary<PiiType, int>()
        };

        foreach (var piiType in piiTypes)
        {
            if (_piiPatterns.TryGetValue(piiType, out var pattern))
            {
                var matches = pattern.Matches(content);
                
                foreach (Match match in matches)
                {
                    var entity = new PiiEntity
                    {
                        Type = piiType,
                        Value = match.Value,
                        Confidence = CalculateConfidence(piiType, match.Value),
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        RedactedValue = GenerateRedaction(piiType, match.Value),
                        Context = ExtractContext(content, match.Index, match.Length)
                    };

                    result.DetectedEntities.Add(entity);
                    
                    if (!result.PiiTypeCount.ContainsKey(piiType))
                    {
                        result.PiiTypeCount[piiType] = 0;
                    }
                    result.PiiTypeCount[piiType]++;
                }
            }
        }

        result.ContainsPii = result.DetectedEntities.Any();
        result.PiiCount = result.DetectedEntities.Count;
        result.RedactedContent = await RedactPiiAsync(content);
        result.MaskedContent = await MaskPiiAsync(content);

        _logger.LogInformation("PII detection complete. Found {Count} PII entities", result.PiiCount);

        return result;
    }

    public async Task<string> RedactPiiAsync(string content, RedactionStrategy strategy = RedactionStrategy.Replace)
    {
        var detectionResult = await DetectPiiAsync(content);
        var redacted = content;

        // Process entities in reverse order to maintain correct positions
        foreach (var entity in detectionResult.DetectedEntities.OrderByDescending(e => e.StartPosition))
        {
            var replacement = strategy switch
            {
                RedactionStrategy.Replace => entity.RedactedValue,
                RedactionStrategy.Mask => MaskValue(entity.Value),
                RedactionStrategy.Hash => HashValue(entity.Value),
                RedactionStrategy.Remove => string.Empty,
                RedactionStrategy.Pseudonymize => PseudonymizeValue(entity.Type),
                _ => entity.RedactedValue
            };

            redacted = redacted.Remove(entity.StartPosition, entity.EndPosition - entity.StartPosition)
                              .Insert(entity.StartPosition, replacement);
        }

        return redacted;
    }

    public async Task<string> MaskPiiAsync(string content)
    {
        var detectionResult = await DetectPiiAsync(content);
        var masked = content;

        foreach (var entity in detectionResult.DetectedEntities.OrderByDescending(e => e.StartPosition))
        {
            var maskedValue = MaskValue(entity.Value);
            masked = masked.Remove(entity.StartPosition, entity.EndPosition - entity.StartPosition)
                          .Insert(entity.StartPosition, maskedValue);
        }

        return masked;
    }

    public async Task<bool> ContainsPiiAsync(string content)
    {
        var result = await DetectPiiAsync(content);
        return result.ContainsPii;
    }

    private Dictionary<PiiType, Regex> InitializePiiPatterns()
    {
        return new Dictionary<PiiType, Regex>
        {
            // Email addresses
            [PiiType.EmailAddress] = new Regex(
                @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase),

            // Phone numbers (various formats)
            [PiiType.PhoneNumber] = new Regex(
                @"\b(\+?\d{1,3}[-.\s]?)?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}\b",
                RegexOptions.Compiled),

            // Social Security Numbers (US)
            [PiiType.SocialSecurityNumber] = new Regex(
                @"\b\d{3}-\d{2}-\d{4}\b",
                RegexOptions.Compiled),

            // Credit Card Numbers (16 digits with optional spaces/dashes)
            [PiiType.CreditCardNumber] = new Regex(
                @"\b\d{4}[-\s]?\d{4}[-\s]?\d{4}[-\s]?\d{4}\b",
                RegexOptions.Compiled),

            // IP Addresses (IPv4)
            [PiiType.IpAddress] = new Regex(
                @"\b(?:\d{1,3}\.){3}\d{1,3}\b",
                RegexOptions.Compiled),

            // MAC Addresses
            [PiiType.MacAddress] = new Regex(
                @"\b([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})\b",
                RegexOptions.Compiled),

            // URLs
            [PiiType.Url] = new Regex(
                @"https?://[^\s<>""]+|www\.[^\s<>""]+",
                RegexOptions.Compiled | RegexOptions.IgnoreCase),

            // Driver's License (simple pattern)
            [PiiType.DriverLicense] = new Regex(
                @"\b[A-Z]\d{7,8}\b",
                RegexOptions.Compiled),

            // Passport Numbers (simplified)
            [PiiType.Passport] = new Regex(
                @"\b[A-Z]{1,2}\d{6,9}\b",
                RegexOptions.Compiled),

            // Bank Account Numbers (8-17 digits)
            [PiiType.BankAccount] = new Regex(
                @"\b\d{8,17}\b",
                RegexOptions.Compiled),

            // Date of Birth (MM/DD/YYYY or MM-DD-YYYY)
            [PiiType.DateOfBirth] = new Regex(
                @"\b(0?[1-9]|1[0-2])[-/](0?[1-9]|[12]\d|3[01])[-/](19|20)\d{2}\b",
                RegexOptions.Compiled),

            // Street Addresses (simplified)
            [PiiType.Address] = new Regex(
                @"\b\d+\s+[A-Za-z0-9\s,]+\s+(Street|St|Avenue|Ave|Road|Rd|Boulevard|Blvd|Lane|Ln|Drive|Dr|Court|Ct|Way)\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase),

            // Person Names (simple pattern - requires context)
            [PiiType.PersonName] = new Regex(
                @"\b[A-Z][a-z]+\s+[A-Z][a-z]+\b",
                RegexOptions.Compiled),

            // Tax ID (US EIN)
            [PiiType.TaxId] = new Regex(
                @"\b\d{2}-\d{7}\b",
                RegexOptions.Compiled),

            // API Keys (common patterns)
            [PiiType.ApiKey] = new Regex(
                @"\b(api[_-]?key|apikey|access[_-]?token|secret[_-]?key)[:\s=]+[A-Za-z0-9_\-]{20,}\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase),

            // Passwords in text
            [PiiType.Password] = new Regex(
                @"\b(password|passwd|pwd)[:\s=]+[^\s]{6,}\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase),

            // GPS Coordinates
            [PiiType.Coordinate] = new Regex(
                @"\b-?\d{1,3}\.\d+,\s*-?\d{1,3}\.\d+\b",
                RegexOptions.Compiled)
        };
    }

    private double CalculateConfidence(PiiType type, string value)
    {
        // More sophisticated validation for higher confidence
        return type switch
        {
            PiiType.EmailAddress => ValidateEmail(value) ? 0.95 : 0.7,
            PiiType.PhoneNumber => ValidatePhoneNumber(value) ? 0.9 : 0.6,
            PiiType.CreditCardNumber => ValidateCreditCard(value) ? 0.95 : 0.5,
            PiiType.SocialSecurityNumber => ValidateSSN(value) ? 0.95 : 0.7,
            PiiType.IpAddress => ValidateIpAddress(value) ? 0.95 : 0.7,
            PiiType.Url => ValidateUrl(value) ? 0.9 : 0.6,
            _ => 0.75
        };
    }

    private bool ValidateEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool ValidatePhoneNumber(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        return digits.Length >= 10 && digits.Length <= 15;
    }

    private bool ValidateCreditCard(string cardNumber)
    {
        // Luhn algorithm
        var digits = cardNumber.Where(char.IsDigit).Select(c => c - '0').ToArray();
        if (digits.Length != 16) return false;

        int sum = 0;
        bool alternate = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int digit = digits[i];
            if (alternate)
            {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }
            sum += digit;
            alternate = !alternate;
        }
        return sum % 10 == 0;
    }

    private bool ValidateSSN(string ssn)
    {
        var parts = ssn.Split('-');
        if (parts.Length != 3) return false;
        return parts[0] != "000" && parts[0] != "666" && !parts[0].StartsWith("9");
    }

    private bool ValidateIpAddress(string ip)
    {
        var parts = ip.Split('.');
        if (parts.Length != 4) return false;
        return parts.All(p => int.TryParse(p, out int num) && num >= 0 && num <= 255);
    }

    private bool ValidateUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    private string GenerateRedaction(PiiType type, string value)
    {
        return type switch
        {
            PiiType.EmailAddress => "[EMAIL]",
            PiiType.PhoneNumber => "[PHONE]",
            PiiType.SocialSecurityNumber => "[SSN]",
            PiiType.CreditCardNumber => "[CREDIT_CARD]",
            PiiType.IpAddress => "[IP_ADDRESS]",
            PiiType.MacAddress => "[MAC_ADDRESS]",
            PiiType.DriverLicense => "[DRIVER_LICENSE]",
            PiiType.Passport => "[PASSPORT]",
            PiiType.BankAccount => "[BANK_ACCOUNT]",
            PiiType.DateOfBirth => "[DATE_OF_BIRTH]",
            PiiType.Address => "[ADDRESS]",
            PiiType.PersonName => "[NAME]",
            PiiType.TaxId => "[TAX_ID]",
            PiiType.ApiKey => "[API_KEY]",
            PiiType.Password => "[PASSWORD]",
            PiiType.Url => "[URL]",
            PiiType.Coordinate => "[COORDINATES]",
            _ => "[REDACTED]"
        };
    }

    private string MaskValue(string value)
    {
        if (value.Length <= 4)
            return new string('*', value.Length);

        // Show first and last 2 characters, mask the rest
        return value.Substring(0, 2) + new string('*', value.Length - 4) + value.Substring(value.Length - 2);
    }

    private string HashValue(string value)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(hashBytes).Substring(0, 16);
    }

    private string PseudonymizeValue(PiiType type)
    {
        return type switch
        {
            PiiType.EmailAddress => $"user{Random.Shared.Next(1000, 9999)}@example.com",
            PiiType.PhoneNumber => $"+1-555-{Random.Shared.Next(100, 999)}-{Random.Shared.Next(1000, 9999)}",
            PiiType.PersonName => $"User {Random.Shared.Next(1000, 9999)}",
            PiiType.Address => $"{Random.Shared.Next(100, 999)} Main Street",
            _ => $"[PSEUDO_{Random.Shared.Next(1000, 9999)}]"
        };
    }

    private string ExtractContext(string content, int position, int length, int contextWindow = 30)
    {
        var start = Math.Max(0, position - contextWindow);
        var end = Math.Min(content.Length, position + length + contextWindow);
        var context = content.Substring(start, end - start);

        if (start > 0) context = "..." + context;
        if (end < content.Length) context = context + "...";

        return context;
    }
}
