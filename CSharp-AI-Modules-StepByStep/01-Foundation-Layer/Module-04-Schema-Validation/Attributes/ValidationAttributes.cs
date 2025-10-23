using System.ComponentModel.DataAnnotations;

namespace AI.SchemaValidation.Attributes;

/// <summary>
/// Custom validation attribute for email domains
/// </summary>
public class AllowedEmailDomainsAttribute : ValidationAttribute
{
    private readonly string[] _allowedDomains;

    public AllowedEmailDomainsAttribute(params string[] allowedDomains)
    {
        _allowedDomains = allowedDomains ?? throw new ArgumentNullException(nameof(allowedDomains));
        ErrorMessage = ErrorMessage ?? "Email domain is not allowed.";
    }

    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return true; // Let other validators handle required validation

        var email = value.ToString()!;
        var emailParts = email.Split('@');
        
        if (emailParts.Length != 2)
            return false;

        var domain = emailParts[1].ToLowerInvariant();
        return _allowedDomains.Contains(domain, StringComparer.OrdinalIgnoreCase);
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field must use one of the following domains: {string.Join(", ", _allowedDomains)}.";
    }
}

/// <summary>
/// Custom validation attribute for age validation
/// </summary>
public class AgeRangeAttribute : ValidationAttribute
{
    public int MinimumAge { get; set; } = 0;
    public int MaximumAge { get; set; } = 120;

    public override bool IsValid(object? value)
    {
        if (value == null)
            return true; // Let other validators handle required validation

        if (value is int age)
        {
            return age >= MinimumAge && age <= MaximumAge;
        }

        if (value is DateTime dateOfBirth)
        {
            var calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-calculatedAge))
                calculatedAge--;

            return calculatedAge >= MinimumAge && calculatedAge <= MaximumAge;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} must be between {MinimumAge} and {MaximumAge} years.";
    }
}

/// <summary>
/// Custom validation attribute for strong passwords
/// </summary>
public class StrongPasswordAttribute : ValidationAttribute
{
    public bool RequireUppercase { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireNumbers { get; set; } = true;
    public bool RequireSpecialChars { get; set; } = true;
    public int MinimumLength { get; set; } = 8;

    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return true; // Let other validators handle required validation

        var password = value.ToString()!;

        if (password.Length < MinimumLength)
            return false;

        if (RequireUppercase && !password.Any(char.IsUpper))
            return false;

        if (RequireLowercase && !password.Any(char.IsLower))
            return false;

        if (RequireNumbers && !password.Any(char.IsDigit))
            return false;

        if (RequireSpecialChars && !password.Any(c => !char.IsLetterOrDigit(c)))
            return false;

        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        var requirements = new List<string>();
        
        if (RequireUppercase) requirements.Add("uppercase letter");
        if (RequireLowercase) requirements.Add("lowercase letter");
        if (RequireNumbers) requirements.Add("number");
        if (RequireSpecialChars) requirements.Add("special character");

        var requirementsText = string.Join(", ", requirements);
        return $"The {name} must be at least {MinimumLength} characters long and contain at least one {requirementsText}.";
    }
}

/// <summary>
/// Custom validation attribute for phone numbers
/// </summary>
public class PhoneNumberAttribute : ValidationAttribute
{
    public string CountryCode { get; set; } = "";
    public bool AllowInternational { get; set; } = true;

    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return true; // Let other validators handle required validation

        var phoneNumber = value.ToString()!.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        // Remove common formatting characters
        phoneNumber = phoneNumber.Replace("+", "");

        // Basic validation: only digits
        if (!phoneNumber.All(char.IsDigit))
            return false;

        // Length validation
        if (phoneNumber.Length < 7 || phoneNumber.Length > 15)
            return false;

        // Country-specific validation
        if (!string.IsNullOrEmpty(CountryCode))
        {
            return phoneNumber.StartsWith(CountryCode);
        }

        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field must be a valid phone number.";
    }
}

/// <summary>
/// Custom validation attribute for file sizes
/// </summary>
public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly long _maxFileSize;

    public MaxFileSizeAttribute(long maxFileSize)
    {
        _maxFileSize = maxFileSize;
        ErrorMessage = ErrorMessage ?? $"File size cannot exceed {FormatFileSize(maxFileSize)}.";
    }

    public override bool IsValid(object? value)
    {
        if (value is IFormFile file)
        {
            return file.Length <= _maxFileSize;
        }

        if (value is long fileSize)
        {
            return fileSize <= _maxFileSize;
        }

        return true; // Not a file, let other validators handle
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

/// <summary>
/// Custom validation attribute for allowed file types
/// </summary>
public class AllowedFileTypesAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions;

    public AllowedFileTypesAttribute(params string[] allowedExtensions)
    {
        _allowedExtensions = allowedExtensions?.Select(ext => ext.ToLowerInvariant()).ToArray() 
                           ?? throw new ArgumentNullException(nameof(allowedExtensions));
        ErrorMessage = ErrorMessage ?? $"Only the following file types are allowed: {string.Join(", ", allowedExtensions)}.";
    }

    public override bool IsValid(object? value)
    {
        if (value is IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            return !string.IsNullOrEmpty(extension) && _allowedExtensions.Contains(extension);
        }

        if (value is string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            return !string.IsNullOrEmpty(extension) && _allowedExtensions.Contains(extension);
        }

        return true; // Not a file, let other validators handle
    }
}

/// <summary>
/// Custom validation attribute for credit card numbers
/// </summary>
public class CreditCardNumberAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return true; // Let other validators handle required validation

        var cardNumber = value.ToString()!.Replace(" ", "").Replace("-", "");

        // Must be all digits
        if (!cardNumber.All(char.IsDigit))
            return false;

        // Length validation (most cards are 13-19 digits)
        if (cardNumber.Length < 13 || cardNumber.Length > 19)
            return false;

        // Luhn algorithm validation
        return IsValidLuhn(cardNumber);
    }

    private static bool IsValidLuhn(string cardNumber)
    {
        int sum = 0;
        bool alternate = false;

        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(cardNumber[i].ToString());

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit = digit / 10 + digit % 10;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field must be a valid credit card number.";
    }
}

/// <summary>
/// Custom validation attribute for URL validation with specific schemes
/// </summary>
public class UrlWithSchemeAttribute : ValidationAttribute
{
    private readonly string[] _allowedSchemes;

    public UrlWithSchemeAttribute(params string[] allowedSchemes)
    {
        _allowedSchemes = allowedSchemes ?? new[] { "http", "https" };
        ErrorMessage = ErrorMessage ?? $"URL must use one of the following schemes: {string.Join(", ", _allowedSchemes)}.";
    }

    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return true; // Let other validators handle required validation

        var url = value.ToString()!;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
            return false;

        return _allowedSchemes.Contains(uriResult.Scheme, StringComparer.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Custom validation attribute for date ranges
/// </summary>
public class DateRangeAttribute : ValidationAttribute
{
    public string? MinimumDate { get; set; }
    public string? MaximumDate { get; set; }
    public bool AllowFutureDates { get; set; } = true;
    public bool AllowPastDates { get; set; } = true;

    public override bool IsValid(object? value)
    {
        if (value == null)
            return true; // Let other validators handle required validation

        if (value is not DateTime date)
        {
            if (DateTime.TryParse(value.ToString(), out date))
            {
                // Successfully parsed
            }
            else
            {
                return false;
            }
        }

        var today = DateTime.Today;

        if (!AllowFutureDates && date > today)
            return false;

        if (!AllowPastDates && date < today)
            return false;

        if (!string.IsNullOrEmpty(MinimumDate) && DateTime.TryParse(MinimumDate, out var minDate))
        {
            if (date < minDate)
                return false;
        }

        if (!string.IsNullOrEmpty(MaximumDate) && DateTime.TryParse(MaximumDate, out var maxDate))
        {
            if (date > maxDate)
                return false;
        }

        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        var constraints = new List<string>();

        if (!AllowPastDates) constraints.Add("cannot be in the past");
        if (!AllowFutureDates) constraints.Add("cannot be in the future");
        if (!string.IsNullOrEmpty(MinimumDate)) constraints.Add($"must be after {MinimumDate}");
        if (!string.IsNullOrEmpty(MaximumDate)) constraints.Add($"must be before {MaximumDate}");

        var constraintsText = string.Join(" and ", constraints);
        return $"The {name} field {constraintsText}.";
    }
}

/// <summary>
/// Custom validation attribute for comparing two properties
/// </summary>
public class ComparePropertyAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;
    private readonly ComparisonType _comparisonType;

    public ComparePropertyAttribute(string comparisonProperty, ComparisonType comparisonType = ComparisonType.Equal)
    {
        _comparisonProperty = comparisonProperty ?? throw new ArgumentNullException(nameof(comparisonProperty));
        _comparisonType = comparisonType;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var comparisonValue = validationContext.ObjectType.GetProperty(_comparisonProperty)
                             ?.GetValue(validationContext.ObjectInstance);

        if (value == null && comparisonValue == null)
            return ValidationResult.Success;

        if (value == null || comparisonValue == null)
        {
            return _comparisonType == ComparisonType.NotEqual 
                ? ValidationResult.Success 
                : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        var comparison = Comparer.Default.Compare(value, comparisonValue);

        var isValid = _comparisonType switch
        {
            ComparisonType.Equal => comparison == 0,
            ComparisonType.NotEqual => comparison != 0,
            ComparisonType.GreaterThan => comparison > 0,
            ComparisonType.GreaterThanOrEqual => comparison >= 0,
            ComparisonType.LessThan => comparison < 0,
            ComparisonType.LessThanOrEqual => comparison <= 0,
            _ => false
        };

        return isValid ? ValidationResult.Success : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    }

    public override string FormatErrorMessage(string name)
    {
        var comparisonText = _comparisonType switch
        {
            ComparisonType.Equal => "equal to",
            ComparisonType.NotEqual => "not equal to",
            ComparisonType.GreaterThan => "greater than",
            ComparisonType.GreaterThanOrEqual => "greater than or equal to",
            ComparisonType.LessThan => "less than",
            ComparisonType.LessThanOrEqual => "less than or equal to",
            _ => "compared to"
        };

        return $"The {name} field must be {comparisonText} {_comparisonProperty}.";
    }
}

/// <summary>
/// Comparison types for property comparison
/// </summary>
public enum ComparisonType
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual
}