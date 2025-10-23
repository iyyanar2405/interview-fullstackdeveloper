using System.Text;
using System.Text.Json;
using Module_04_Data_Protection.Models;

namespace Module_04_Data_Protection.Services;

public interface IAnonymizationService
{
    Task<AnonymizationResult> AnonymizeAsync(AnonymizationRequest request);
    Task<string> AnonymizeFieldAsync(string value, AnonymizationMethod method);
    Task<Dictionary<string, string>> BulkAnonymizeAsync(Dictionary<string, string> data, AnonymizationMethod method);
}

public class AnonymizationService : IAnonymizationService
{
    private readonly ILogger<AnonymizationService> _logger;
    private readonly Dictionary<string, string> _tokenMappings;
    private readonly Random _random;

    public AnonymizationService(ILogger<AnonymizationService> logger)
    {
        _logger = logger;
        _tokenMappings = new Dictionary<string, string>();
        _random = new Random();
    }

    public async Task<AnonymizationResult> AnonymizeAsync(AnonymizationRequest request)
    {
        _logger.LogInformation("Anonymizing data using method: {Method}", request.Method);

        var anonymizedData = new Dictionary<string, object>();
        var fieldMappings = new Dictionary<string, string>();

        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(request.Data) 
            ?? new Dictionary<string, object>();

        foreach (var kvp in data)
        {
            if (request.FieldsToAnonymize.Contains(kvp.Key))
            {
                var originalValue = kvp.Value?.ToString() ?? "";
                var anonymizedValue = await AnonymizeFieldAsync(originalValue, request.Method);
                anonymizedData[kvp.Key] = anonymizedValue;
                fieldMappings[kvp.Key] = $"{originalValue} -> {anonymizedValue}";
            }
            else
            {
                anonymizedData[kvp.Key] = kvp.Value;
            }
        }

        return new AnonymizationResult
        {
            AnonymizedData = JsonSerializer.Serialize(anonymizedData),
            Method = request.Method,
            FieldMappings = fieldMappings,
            AnonymizedFieldsCount = request.FieldsToAnonymize.Count,
            Reversible = request.Reversible && (request.Method == AnonymizationMethod.Tokenization 
                || request.Method == AnonymizationMethod.Pseudonymization),
            ProcessedAt = DateTime.UtcNow
        };
    }

    public async Task<string> AnonymizeFieldAsync(string value, AnonymizationMethod method)
    {
        return method switch
        {
            AnonymizationMethod.Masking => Mask(value),
            AnonymizationMethod.Pseudonymization => Pseudonymize(value),
            AnonymizationMethod.Generalization => Generalize(value),
            AnonymizationMethod.Perturbation => Perturb(value),
            AnonymizationMethod.Suppression => "[SUPPRESSED]",
            AnonymizationMethod.KAnonymity => await ApplyKAnonymityAsync(value),
            AnonymizationMethod.Tokenization => Tokenize(value),
            _ => value
        };
    }

    public async Task<Dictionary<string, string>> BulkAnonymizeAsync(
        Dictionary<string, string> data, 
        AnonymizationMethod method)
    {
        var result = new Dictionary<string, string>();

        foreach (var kvp in data)
        {
            result[kvp.Key] = await AnonymizeFieldAsync(kvp.Value, method);
        }

        return result;
    }

    private string Mask(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        // Email masking: john.doe@example.com -> j***@example.com
        if (value.Contains("@"))
        {
            var parts = value.Split('@');
            if (parts.Length == 2)
            {
                var username = parts[0];
                var masked = username.Length > 1 
                    ? username[0] + new string('*', Math.Min(username.Length - 1, 3)) 
                    : "*";
                return $"{masked}@{parts[1]}";
            }
        }

        // Phone masking: +1234567890 -> +123***7890
        if (value.StartsWith("+") && value.Length > 7)
        {
            return value.Substring(0, 4) + new string('*', 3) + value.Substring(value.Length - 4);
        }

        // General masking: show first and last char only
        if (value.Length > 2)
        {
            return value[0] + new string('*', Math.Min(value.Length - 2, 8)) + value[^1];
        }

        return new string('*', value.Length);
    }

    private string Pseudonymize(string value)
    {
        // Replace with fake but realistic data
        if (value.Contains("@"))
        {
            return $"user{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
        }

        if (value.StartsWith("+") || (value.Length >= 10 && value.All(char.IsDigit)))
        {
            return $"+1{_random.Next(100, 999)}{_random.Next(100, 999)}{_random.Next(1000, 9999)}";
        }

        // Name pseudonymization
        var fakeNames = new[] { "John Smith", "Jane Doe", "Alex Johnson", "Sam Wilson", "Pat Taylor" };
        return fakeNames[_random.Next(fakeNames.Length)];
    }

    private string Generalize(string value)
    {
        // Age generalization: 25 -> "20-30"
        if (int.TryParse(value, out int age))
        {
            int lowerBound = (age / 10) * 10;
            return $"{lowerBound}-{lowerBound + 9}";
        }

        // Date generalization: 2024-05-15 -> 2024-05
        if (DateTime.TryParse(value, out DateTime date))
        {
            return date.ToString("yyyy-MM");
        }

        // Zip code generalization: 12345 -> 123**
        if (value.Length == 5 && value.All(char.IsDigit))
        {
            return value.Substring(0, 3) + "**";
        }

        return value;
    }

    private string Perturb(string value)
    {
        // Add noise to numerical data
        if (double.TryParse(value, out double number))
        {
            double noise = _random.NextDouble() * 10 - 5; // ±5 noise
            return (number + noise).ToString("F2");
        }

        return value;
    }

    private async Task<string> ApplyKAnonymityAsync(string value)
    {
        // Simplified k-anonymity: generalize to ensure k=5 similar records
        // In production, this would involve analyzing the entire dataset
        return await Task.FromResult(Generalize(value));
    }

    private string Tokenize(string value)
    {
        // Create reversible token mapping
        if (_tokenMappings.ContainsKey(value))
        {
            return _tokenMappings[value];
        }

        var token = $"TOKEN_{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}";
        _tokenMappings[value] = token;
        _tokenMappings[token] = value; // Reverse mapping
        
        return token;
    }
}
