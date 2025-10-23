using System.ComponentModel.DataAnnotations;
using AI.SchemaValidation.Attributes;

namespace AI.SchemaValidation.Models;

/// <summary>
/// User model with comprehensive validation attributes
/// </summary>
public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(255, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 255 characters")]
    [AllowedEmailDomains("example.com", "company.com", "gmail.com", "outlook.com")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Age is required")]
    [AgeRange(MinimumAge = 13, MaximumAge = 120, ErrorMessage = "Age must be between 13 and 120")]
    public int Age { get; set; }

    [PhoneNumber(AllowInternational = true)]
    public string? Phone { get; set; }

    [UrlWithScheme("http", "https")]
    public string? Website { get; set; }

    [DateRange(AllowFutureDates = false, ErrorMessage = "Date of birth cannot be in the future")]
    public DateTime? DateOfBirth { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Address? Address { get; set; }

    public List<Skill>? Skills { get; set; }
}

/// <summary>
/// Address model with validation
/// </summary>
public class Address
{
    [Required(ErrorMessage = "Street address is required")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Street address must be between 5 and 200 characters")]
    public required string Street { get; set; }

    [Required(ErrorMessage = "City is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "City can only contain letters, spaces, hyphens, and apostrophes")]
    public required string City { get; set; }

    [Required(ErrorMessage = "State is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "State must be between 2 and 100 characters")]
    public required string State { get; set; }

    [Required(ErrorMessage = "Zip code is required")]
    [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Zip code must be in format 12345 or 12345-6789")]
    public required string ZipCode { get; set; }

    [Required(ErrorMessage = "Country is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 100 characters")]
    public required string Country { get; set; }
}

/// <summary>
/// Skill model with validation
/// </summary>
public class Skill
{
    [Required(ErrorMessage = "Skill name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Skill name must be between 2 and 50 characters")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Skill level is required")]
    public SkillLevel Level { get; set; }

    [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
    public int YearsOfExperience { get; set; }

    public bool Certified { get; set; }
}

/// <summary>
/// Skill level enumeration
/// </summary>
public enum SkillLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
    Expert = 4
}

/// <summary>
/// Create user request model with validation
/// </summary>
public class CreateUserRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [AllowedEmailDomains("example.com", "company.com", "gmail.com", "outlook.com")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Age is required")]
    [AgeRange(MinimumAge = 13, MaximumAge = 120)]
    public int Age { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StrongPassword(MinimumLength = 8, ErrorMessage = "Password does not meet security requirements")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Password confirmation is required")]
    [CompareProperty(nameof(Password), ErrorMessage = "Password and confirmation password must match")]
    public required string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "You must accept the terms and conditions")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions")]
    public bool TermsAccepted { get; set; }

    [PhoneNumber(AllowInternational = true)]
    public string? Phone { get; set; }

    [UrlWithScheme("http", "https")]
    public string? Website { get; set; }
}

/// <summary>
/// Update user request model with validation
/// </summary>
public class UpdateUserRequest
{
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [AllowedEmailDomains("example.com", "company.com", "gmail.com", "outlook.com")]
    public string? Email { get; set; }

    [AgeRange(MinimumAge = 13, MaximumAge = 120)]
    public int? Age { get; set; }

    [PhoneNumber(AllowInternational = true)]
    public string? Phone { get; set; }

    [UrlWithScheme("http", "https")]
    public string? Website { get; set; }
}

/// <summary>
/// Product model demonstrating complex validation scenarios
/// </summary>
public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 100 characters")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Product description is required")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Product description must be between 10 and 1000 characters")]
    public required string Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 1000000, ErrorMessage = "Price must be between $0.01 and $1,000,000")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public required string Category { get; set; }

    [Required(ErrorMessage = "SKU is required")]
    [RegularExpression(@"^[A-Z]{2}-\d{4}-[A-Z]{2}$", ErrorMessage = "SKU must be in format XX-1234-XX")]
    public required string SKU { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int Stock { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
    public double? Weight { get; set; }

    public bool RequiresShipping { get; set; }

    public bool IsPerishable { get; set; }

    [DateRange(AllowPastDates = false, ErrorMessage = "Expiry date must be in the future")]
    public DateTime? ExpiryDate { get; set; }

    public Dimensions? Dimensions { get; set; }

    public List<string>? Tags { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Dimensions model with validation
/// </summary>
public class Dimensions
{
    [Required(ErrorMessage = "Length is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Length must be greater than 0")]
    public double Length { get; set; }

    [Required(ErrorMessage = "Width is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Width must be greater than 0")]
    public double Width { get; set; }

    [Required(ErrorMessage = "Height is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Height must be greater than 0")]
    public double Height { get; set; }

    [Required(ErrorMessage = "Unit is required")]
    public MeasurementUnit Unit { get; set; }
}

/// <summary>
/// Measurement unit enumeration
/// </summary>
public enum MeasurementUnit
{
    Inches = 1,
    Centimeters = 2,
    Feet = 3,
    Meters = 4
}

/// <summary>
/// File upload model with validation
/// </summary>
public class FileUploadRequest
{
    [Required(ErrorMessage = "File is required")]
    [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size cannot exceed 5MB")]
    [AllowedFileTypes(".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx")]
    public required IFormFile File { get; set; }

    [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public required string Category { get; set; }
}

/// <summary>
/// Payment model with validation
/// </summary>
public class PaymentRequest
{
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Amount must be between $0.01 and $999,999.99")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Currency is required")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter code")]
    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be a valid 3-letter code")]
    public required string Currency { get; set; }

    [Required(ErrorMessage = "Credit card number is required")]
    [CreditCardNumber]
    public required string CreditCardNumber { get; set; }

    [Required(ErrorMessage = "Card holder name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Card holder name must be between 2 and 100 characters")]
    public required string CardHolderName { get; set; }

    [Required(ErrorMessage = "Expiry month is required")]
    [Range(1, 12, ErrorMessage = "Expiry month must be between 1 and 12")]
    public int ExpiryMonth { get; set; }

    [Required(ErrorMessage = "Expiry year is required")]
    [Range(2024, 2034, ErrorMessage = "Expiry year must be between 2024 and 2034")]
    public int ExpiryYear { get; set; }

    [Required(ErrorMessage = "CVV is required")]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits")]
    public required string CVV { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}

/// <summary>
/// Search request model with validation
/// </summary>
public class SearchRequest
{
    [Required(ErrorMessage = "Search query is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Search query must be between 2 and 200 characters")]
    public required string Query { get; set; }

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;

    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; } = 1;

    public string? Category { get; set; }

    public string? SortBy { get; set; }

    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

    public List<string>? Filters { get; set; }
}

/// <summary>
/// Sort order enumeration
/// </summary>
public enum SortOrder
{
    Ascending = 1,
    Descending = 2
}