using FluentValidation;
using Module_02_Input_Validation.Models;
using System.Text.RegularExpressions;

namespace Module_02_Input_Validation.Validators;

public class UserRegistrationValidator : AbstractValidator<UserRegistrationRequest>
{
    public UserRegistrationValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .Length(3, 20).WithMessage("Username must be between 3 and 20 characters")
            .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Username can only contain letters, numbers, underscore, and hyphen")
            .Must(NotContainProfanity).WithMessage("Username contains inappropriate content");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
            .Must(BeValidEmailDomain).WithMessage("Email domain is not allowed");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
            .Matches(@"[@$!%*?&#]").WithMessage("Password must contain at least one special character");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(2, 50).WithMessage("First name must be between 2 and 50 characters")
            .Matches(@"^[a-zA-Z\s'-]+$").WithMessage("First name contains invalid characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters")
            .Matches(@"^[a-zA-Z\s'-]+$").WithMessage("Last name contains invalid characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.Now.AddYears(-120)).WithMessage("Date of birth is not realistic")
            .Must(BeAtLeast13YearsOld).WithMessage("User must be at least 13 years old")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.Website)
            .Matches(CommonPatterns.Url).WithMessage("Invalid website URL")
            .When(x => !string.IsNullOrEmpty(x.Website));

        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters")
            .Must(NotContainHtml).WithMessage("Bio cannot contain HTML")
            .When(x => !string.IsNullOrEmpty(x.Bio));
    }

    private bool NotContainProfanity(string username)
    {
        var profanityList = new[] { "badword1", "badword2" }; // In production, load from config
        return !profanityList.Any(p => username.ToLower().Contains(p));
    }

    private bool BeValidEmailDomain(string email)
    {
        var blockedDomains = new[] { "tempmail.com", "throwaway.email" };
        var domain = email.Split('@').Last().ToLower();
        return !blockedDomains.Contains(domain);
    }

    private bool BeAtLeast13YearsOld(DateTime? dateOfBirth)
    {
        if (!dateOfBirth.HasValue) return true;
        var age = DateTime.Now.Year - dateOfBirth.Value.Year;
        if (dateOfBirth.Value.Date > DateTime.Now.AddYears(-age)) age--;
        return age >= 13;
    }

    private bool NotContainHtml(string text)
    {
        return !Regex.IsMatch(text, @"<[^>]+>");
    }
}

public class CommentValidator : AbstractValidator<CommentRequest>
{
    public CommentValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required")
            .Length(1, 1000).WithMessage("Comment must be between 1 and 1000 characters")
            .Must(NotContainHtml).WithMessage("Comments cannot contain HTML")
            .Must(NotContainUrls).WithMessage("Comments cannot contain URLs")
            .Must(NotContainProfanity).WithMessage("Comment contains inappropriate content");

        RuleFor(x => x.AuthorName)
            .NotEmpty().WithMessage("Author name is required")
            .Length(2, 50).WithMessage("Author name must be between 2 and 50 characters")
            .Matches(@"^[a-zA-Z\s'-]+$").WithMessage("Author name contains invalid characters");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Website)
            .Matches(CommonPatterns.Url).WithMessage("Invalid website URL")
            .When(x => !string.IsNullOrEmpty(x.Website));
    }

    private bool NotContainHtml(string content)
    {
        return !Regex.IsMatch(content, @"<[^>]+>");
    }

    private bool NotContainUrls(string content)
    {
        return !Regex.IsMatch(content, @"https?://", RegexOptions.IgnoreCase);
    }

    private bool NotContainProfanity(string content)
    {
        var profanityList = new[] { "badword1", "badword2" };
        return !profanityList.Any(p => content.ToLower().Contains(p));
    }
}

public class SearchRequestValidator : AbstractValidator<SearchRequest>
{
    public SearchRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty().WithMessage("Search query is required")
            .MaximumLength(200).WithMessage("Search query must not exceed 200 characters")
            .Must(NotContainSqlInjection).WithMessage("Search query contains invalid characters")
            .Must(NotContainScriptTags).WithMessage("Search query contains invalid content");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Page number is too large");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100");

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField).WithMessage("Invalid sort field")
            .When(x => !string.IsNullOrEmpty(x.SortBy));

        RuleFor(x => x.SortOrder)
            .Must(BeValidSortOrder).WithMessage("Sort order must be 'asc' or 'desc'")
            .When(x => !string.IsNullOrEmpty(x.SortOrder));

        RuleFor(x => x.Filters)
            .Must(HaveValidCount).WithMessage("Too many filters")
            .When(x => x.Filters != null);
    }

    private bool NotContainSqlInjection(string query)
    {
        var sqlPatterns = new[] { "drop", "delete", "insert", "update", "exec", "execute", "--", "/*", "*/" };
        return !sqlPatterns.Any(p => query.ToLower().Contains(p));
    }

    private bool NotContainScriptTags(string query)
    {
        return !Regex.IsMatch(query, @"<script", RegexOptions.IgnoreCase);
    }

    private bool BeValidSortField(string sortBy)
    {
        var allowedFields = new[] { "name", "date", "price", "rating", "views" };
        return allowedFields.Contains(sortBy.ToLower());
    }

    private bool BeValidSortOrder(string sortOrder)
    {
        return sortOrder.ToLower() is "asc" or "desc";
    }

    private bool HaveValidCount(List<string> filters)
    {
        return filters.Count <= 10;
    }
}

public class FileUploadValidator : AbstractValidator<FileUploadRequest>
{
    private static readonly List<string> AllowedExtensions = new()
    {
        ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt"
    };

    private static readonly List<string> AllowedMimeTypes = new()
    {
        "image/jpeg", "image/png", "image/gif",
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "text/plain"
    };

    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public FileUploadValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required")
            .MaximumLength(255).WithMessage("File name is too long")
            .Must(HaveValidExtension).WithMessage("File extension is not allowed")
            .Must(NotContainPathTraversal).WithMessage("File name contains invalid characters");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required")
            .Must(BeAllowedMimeType).WithMessage("File type is not allowed");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("File size must be greater than 0")
            .LessThanOrEqualTo(MaxFileSize).WithMessage($"File size must not exceed {MaxFileSize / 1024 / 1024} MB");

        RuleFor(x => x.FileContent)
            .NotEmpty().WithMessage("File content is required")
            .Must((request, content) => content.Length == request.FileSize)
            .WithMessage("File size mismatch");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }

    private bool HaveValidExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return AllowedExtensions.Contains(extension);
    }

    private bool NotContainPathTraversal(string fileName)
    {
        return !fileName.Contains("..") && !fileName.Contains("/") && !fileName.Contains("\\");
    }

    private bool BeAllowedMimeType(string contentType)
    {
        return AllowedMimeTypes.Contains(contentType.ToLower());
    }
}

public class HtmlContentValidator : AbstractValidator<HtmlContentRequest>
{
    public HtmlContentValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(50000).WithMessage("Content is too large")
            .Must((request, content) => ValidateHtmlContent(request, content))
            .WithMessage("Content contains disallowed elements");

        RuleFor(x => x.AllowedTags)
            .Must(HaveValidTags).WithMessage("Allowed tags list contains invalid tags")
            .When(x => x.AllowedTags != null && x.AllowedTags.Count > 0);
    }

    private bool ValidateHtmlContent(HtmlContentRequest request, string content)
    {
        if (!request.AllowScripts && Regex.IsMatch(content, @"<script", RegexOptions.IgnoreCase))
            return false;

        if (!request.AllowStyles && Regex.IsMatch(content, @"<style", RegexOptions.IgnoreCase))
            return false;

        return true;
    }

    private bool HaveValidTags(List<string> tags)
    {
        var validTags = new[] { "p", "br", "strong", "em", "u", "a", "img", "ul", "ol", "li", "h1", "h2", "h3", "h4", "h5", "h6" };
        return tags.All(t => validTags.Contains(t.ToLower()));
    }
}
