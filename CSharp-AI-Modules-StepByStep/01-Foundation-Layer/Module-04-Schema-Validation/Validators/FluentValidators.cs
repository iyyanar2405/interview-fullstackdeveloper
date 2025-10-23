using FluentValidation;
using AI.SchemaValidation.Models;

namespace AI.SchemaValidation.Validators;

/// <summary>
/// User validator using FluentValidation
/// </summary>
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(2, 100)
            .WithMessage("Name must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z\s]+$")
            .WithMessage("Name can only contain letters and spaces");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .Length(5, 255)
            .WithMessage("Email must be between 5 and 255 characters");

        RuleFor(x => x.Age)
            .GreaterThan(0)
            .WithMessage("Age must be greater than 0")
            .LessThanOrEqualTo(120)
            .WithMessage("Age must be less than or equal to 120");

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Phone number must be in valid international format");

        RuleFor(x => x.Website)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrEmpty(x.Website))
            .WithMessage("Website must be a valid URL");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today)
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.Today.AddYears(-120))
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage("Date of birth cannot be more than 120 years ago");

        RuleFor(x => x.Address)
            .SetValidator(new AddressValidator())
            .When(x => x.Address != null);

        RuleForEach(x => x.Skills)
            .SetValidator(new SkillValidator());

        RuleFor(x => x.Skills)
            .Must(skills => skills == null || skills.Count <= 20)
            .WithMessage("Maximum 20 skills allowed");

        // Custom validation rule
        RuleFor(x => x)
            .Must(BeUniqueEmailInDatabase)
            .WithMessage("Email address is already in use")
            .WithName("Email");
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    private static bool BeUniqueEmailInDatabase(User user)
    {
        // In a real application, this would check the database
        // For demo purposes, we'll simulate some existing emails
        var existingEmails = new[]
        {
            "existing@example.com",
            "taken@example.com",
            "admin@example.com"
        };

        return !existingEmails.Contains(user.Email, StringComparer.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Address validator
/// </summary>
public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street address is required")
            .Length(5, 200)
            .WithMessage("Street address must be between 5 and 200 characters");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required")
            .Length(2, 100)
            .WithMessage("City must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("City can only contain letters, spaces, hyphens, and apostrophes");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required")
            .Length(2, 100)
            .WithMessage("State must be between 2 and 100 characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage("Zip code is required")
            .Matches(@"^\d{5}(-\d{4})?$")
            .WithMessage("Zip code must be in format 12345 or 12345-6789");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required")
            .Length(2, 100)
            .WithMessage("Country must be between 2 and 100 characters");
    }
}

/// <summary>
/// Skill validator
/// </summary>
public class SkillValidator : AbstractValidator<Skill>
{
    public SkillValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Skill name is required")
            .Length(2, 50)
            .WithMessage("Skill name must be between 2 and 50 characters");

        RuleFor(x => x.Level)
            .IsInEnum()
            .WithMessage("Skill level must be a valid value (Beginner, Intermediate, Advanced, Expert)");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Years of experience cannot be negative")
            .LessThanOrEqualTo(50)
            .WithMessage("Years of experience cannot exceed 50 years");

        RuleFor(x => x.Certified)
            .NotNull()
            .WithMessage("Certified status must be specified");
    }
}

/// <summary>
/// Create user request validator
/// </summary>
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(2, 100)
            .WithMessage("Name must be between 2 and 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address");

        RuleFor(x => x.Age)
            .GreaterThan(0)
            .WithMessage("Age must be greater than 0")
            .LessThanOrEqualTo(120)
            .WithMessage("Age must be less than or equal to 120");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Password and confirmation password must match");

        RuleFor(x => x.TermsAccepted)
            .Equal(true)
            .WithMessage("Terms and conditions must be accepted");
    }
}

/// <summary>
/// Update user request validator
/// </summary>
public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .Length(2, 100)
            .WithMessage("Name must be between 2 and 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Age)
            .GreaterThan(0)
            .WithMessage("Age must be greater than 0")
            .LessThanOrEqualTo(120)
            .WithMessage("Age must be less than or equal to 120")
            .When(x => x.Age.HasValue);

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Phone number must be in valid international format")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}

/// <summary>
/// Product validator demonstrating complex validation scenarios
/// </summary>
public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .Length(3, 100)
            .WithMessage("Product name must be between 3 and 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Product description is required")
            .Length(10, 1000)
            .WithMessage("Product description must be between 10 and 1000 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .LessThanOrEqualTo(1000000)
            .WithMessage("Price cannot exceed $1,000,000");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category is required")
            .Must(BeValidCategory)
            .WithMessage("Category must be one of: Electronics, Clothing, Books, Home, Sports");

        RuleFor(x => x.SKU)
            .NotEmpty()
            .WithMessage("SKU is required")
            .Matches(@"^[A-Z]{2}-\d{4}-[A-Z]{2}$")
            .WithMessage("SKU must be in format XX-1234-XX (e.g., EL-1234-SM)");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be negative");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0")
            .When(x => x.RequiresShipping);

        RuleFor(x => x.Dimensions)
            .SetValidator(new DimensionsValidator())
            .When(x => x.Dimensions != null);

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .WithMessage("Tag cannot be empty")
            .Length(2, 30)
            .WithMessage("Tag must be between 2 and 30 characters");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("Maximum 10 tags allowed");

        // Conditional validation
        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.Today)
            .WithMessage("Expiry date must be in the future")
            .When(x => x.IsPerishable);

        // Custom complex validation
        RuleFor(x => x)
            .Must(HaveValidPriceForCategory)
            .WithMessage("Price is too high for the selected category")
            .WithName("Price");
    }

    private static bool BeValidCategory(string category)
    {
        var validCategories = new[] { "Electronics", "Clothing", "Books", "Home", "Sports" };
        return validCategories.Contains(category, StringComparer.OrdinalIgnoreCase);
    }

    private static bool HaveValidPriceForCategory(Product product)
    {
        // Custom business rule: certain categories have price limits
        var categoryLimits = new Dictionary<string, decimal>
        {
            { "Books", 500 },
            { "Clothing", 1000 },
            { "Electronics", 50000 },
            { "Home", 10000 },
            { "Sports", 5000 }
        };

        if (!categoryLimits.TryGetValue(product.Category, out var limit))
            return true; // Unknown category, allow any price

        return product.Price <= limit;
    }
}

/// <summary>
/// Dimensions validator
/// </summary>
public class DimensionsValidator : AbstractValidator<Dimensions>
{
    public DimensionsValidator()
    {
        RuleFor(x => x.Length)
            .GreaterThan(0)
            .WithMessage("Length must be greater than 0");

        RuleFor(x => x.Width)
            .GreaterThan(0)
            .WithMessage("Width must be greater than 0");

        RuleFor(x => x.Height)
            .GreaterThan(0)
            .WithMessage("Height must be greater than 0");

        RuleFor(x => x.Unit)
            .IsInEnum()
            .WithMessage("Unit must be a valid measurement unit");

        // Complex validation: total volume shouldn't exceed limits
        RuleFor(x => x)
            .Must(NotExceedVolumeLimit)
            .WithMessage("Total volume cannot exceed 1000 cubic units")
            .WithName("Volume");
    }

    private static bool NotExceedVolumeLimit(Dimensions dimensions)
    {
        var volume = dimensions.Length * dimensions.Width * dimensions.Height;
        return volume <= 1000;
    }
}