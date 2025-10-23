using AI.Anthropic.Models;
using FluentValidation;

namespace AI.Anthropic.Extensions;

/// <summary>
/// Validator for Claude request
/// </summary>
public class ClaudeRequestValidator : AbstractValidator<ClaudeRequest>
{
    public ClaudeRequestValidator()
    {
        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Model is required")
            .Must(BeValidModel)
            .WithMessage("Invalid model. Supported models: claude-3-opus-20240229, claude-3-sonnet-20240229, claude-3-haiku-20240307, claude-2.1, claude-2.0, claude-instant-1.2");

        RuleFor(x => x.Messages)
            .NotEmpty()
            .WithMessage("At least one message is required")
            .Must(HaveValidMessageStructure)
            .WithMessage("Messages must alternate between user and assistant, starting with user");

        RuleFor(x => x.MaxTokens)
            .GreaterThan(0)
            .WithMessage("MaxTokens must be greater than 0")
            .LessThanOrEqualTo(4096)
            .WithMessage("MaxTokens cannot exceed 4096");

        RuleFor(x => x.Temperature)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Temperature must be between 0 and 1")
            .LessThanOrEqualTo(1)
            .WithMessage("Temperature must be between 0 and 1")
            .When(x => x.Temperature.HasValue);

        RuleFor(x => x.TopP)
            .GreaterThanOrEqualTo(0)
            .WithMessage("TopP must be between 0 and 1")
            .LessThanOrEqualTo(1)
            .WithMessage("TopP must be between 0 and 1")
            .When(x => x.TopP.HasValue);

        RuleFor(x => x.TopK)
            .GreaterThan(0)
            .WithMessage("TopK must be greater than 0")
            .When(x => x.TopK.HasValue);

        RuleForEach(x => x.Messages)
            .SetValidator(new MessageValidator());

        RuleForEach(x => x.StopSequences)
            .NotEmpty()
            .WithMessage("Stop sequences cannot be empty")
            .When(x => x.StopSequences != null);
    }

    private static bool BeValidModel(string model)
    {
        var validModels = new[]
        {
            "claude-3-opus-20240229",
            "claude-3-sonnet-20240229",
            "claude-3-haiku-20240307",
            "claude-2.1",
            "claude-2.0",
            "claude-instant-1.2"
        };

        return validModels.Contains(model);
    }

    private static bool HaveValidMessageStructure(IList<ClaudeMessage> messages)
    {
        if (!messages.Any())
            return false;

        // First message must be from user
        if (messages.First().Role != "user")
            return false;

        // Messages should alternate between user and assistant
        for (int i = 1; i < messages.Count; i++)
        {
            var currentRole = messages[i].Role;
            var previousRole = messages[i - 1].Role;

            if (currentRole == previousRole)
                return false;

            if (currentRole != "user" && currentRole != "assistant")
                return false;
        }

        return true;
    }
}

/// <summary>
/// Validator for Claude stream request
/// </summary>
public class ClaudeStreamRequestValidator : AbstractValidator<ClaudeStreamRequest>
{
    public ClaudeStreamRequestValidator()
    {
        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Model is required")
            .Must(BeValidModel)
            .WithMessage("Invalid model. Supported models: claude-3-opus-20240229, claude-3-sonnet-20240229, claude-3-haiku-20240307, claude-2.1, claude-2.0, claude-instant-1.2");

        RuleFor(x => x.Messages)
            .NotEmpty()
            .WithMessage("At least one message is required")
            .Must(HaveValidMessageStructure)
            .WithMessage("Messages must alternate between user and assistant, starting with user");

        RuleFor(x => x.MaxTokens)
            .GreaterThan(0)
            .WithMessage("MaxTokens must be greater than 0")
            .LessThanOrEqualTo(4096)
            .WithMessage("MaxTokens cannot exceed 4096");

        RuleFor(x => x.Temperature)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Temperature must be between 0 and 1")
            .LessThanOrEqualTo(1)
            .WithMessage("Temperature must be between 0 and 1")
            .When(x => x.Temperature.HasValue);

        RuleFor(x => x.TopP)
            .GreaterThanOrEqualTo(0)
            .WithMessage("TopP must be between 0 and 1")
            .LessThanOrEqualTo(1)
            .WithMessage("TopP must be between 0 and 1")
            .When(x => x.TopP.HasValue);

        RuleFor(x => x.TopK)
            .GreaterThan(0)
            .WithMessage("TopK must be greater than 0")
            .When(x => x.TopK.HasValue);

        RuleFor(x => x.Stream)
            .Equal(true)
            .WithMessage("Stream must be true for streaming requests");

        RuleForEach(x => x.Messages)
            .SetValidator(new MessageValidator());
    }

    private static bool BeValidModel(string model)
    {
        var validModels = new[]
        {
            "claude-3-opus-20240229",
            "claude-3-sonnet-20240229",
            "claude-3-haiku-20240307",
            "claude-2.1",
            "claude-2.0",
            "claude-instant-1.2"
        };

        return validModels.Contains(model);
    }

    private static bool HaveValidMessageStructure(IList<ClaudeMessage> messages)
    {
        if (!messages.Any())
            return false;

        // First message must be from user
        if (messages.First().Role != "user")
            return false;

        // Messages should alternate between user and assistant
        for (int i = 1; i < messages.Count; i++)
        {
            var currentRole = messages[i].Role;
            var previousRole = messages[i - 1].Role;

            if (currentRole == previousRole)
                return false;

            if (currentRole != "user" && currentRole != "assistant")
                return false;
        }

        return true;
    }
}

/// <summary>
/// Validator for long context request
/// </summary>
public class LongContextRequestValidator : AbstractValidator<LongContextRequest>
{
    public LongContextRequestValidator()
    {
        RuleFor(x => x.Document)
            .NotEmpty()
            .WithMessage("Document content is required")
            .MinimumLength(100)
            .WithMessage("Document must be at least 100 characters long")
            .MaximumLength(10_000_000)
            .WithMessage("Document cannot exceed 10MB in text length");

        RuleFor(x => x.Query)
            .NotEmpty()
            .WithMessage("Query is required")
            .MinimumLength(5)
            .WithMessage("Query must be at least 5 characters long")
            .MaximumLength(1000)
            .WithMessage("Query cannot exceed 1000 characters");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Model is required")
            .Must(BeValidModel)
            .WithMessage("Invalid model. Supported models: claude-3-opus-20240229, claude-3-sonnet-20240229, claude-3-haiku-20240307");

        RuleFor(x => x.ChunkOverlap)
            .GreaterThanOrEqualTo(0)
            .WithMessage("ChunkOverlap must be non-negative")
            .LessThan(1000)
            .WithMessage("ChunkOverlap should be less than 1000 tokens")
            .When(x => x.ChunkOverlap.HasValue);

        RuleFor(x => x.MaxChunks)
            .GreaterThan(0)
            .WithMessage("MaxChunks must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("MaxChunks cannot exceed 100 for performance reasons")
            .When(x => x.MaxChunks.HasValue);

        RuleFor(x => x.SynthesisInstructions)
            .MaximumLength(500)
            .WithMessage("SynthesisInstructions cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.SynthesisInstructions));
    }

    private static bool BeValidModel(string model)
    {
        var validModels = new[]
        {
            "claude-3-opus-20240229",
            "claude-3-sonnet-20240229",
            "claude-3-haiku-20240307"
        };

        return validModels.Contains(model);
    }
}

/// <summary>
/// Validator for Claude message
/// </summary>
public class MessageValidator : AbstractValidator<ClaudeMessage>
{
    public MessageValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required")
            .Must(BeValidRole)
            .WithMessage("Role must be either 'user' or 'assistant'");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required")
            .MaximumLength(200_000)
            .WithMessage("Content cannot exceed 200,000 characters");
    }

    private static bool BeValidRole(string role)
    {
        return role == "user" || role == "assistant";
    }
}

/// <summary>
/// Validator for Constitutional AI validation request
/// </summary>
public class ConstitutionalValidationRequestValidator : AbstractValidator<ClaudeController.ConstitutionalValidationRequest>
{
    public ConstitutionalValidationRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required for validation")
            .MaximumLength(100_000)
            .WithMessage("Content cannot exceed 100,000 characters for validation");

        RuleFor(x => x.Principles)
            .NotEmpty()
            .WithMessage("At least one Constitutional AI principle is required")
            .Must(HaveValidPrinciples)
            .WithMessage("All principles must be non-empty strings");

        RuleForEach(x => x.Principles)
            .NotEmpty()
            .WithMessage("Principle cannot be empty")
            .MaximumLength(200)
            .WithMessage("Each principle cannot exceed 200 characters");
    }

    private static bool HaveValidPrinciples(IList<string> principles)
    {
        return principles.All(p => !string.IsNullOrWhiteSpace(p));
    }
}

/// <summary>
/// Validator for Constitutional AI improvement request
/// </summary>
public class ConstitutionalImprovementRequestValidator : AbstractValidator<ClaudeController.ConstitutionalImprovementRequest>
{
    public ConstitutionalImprovementRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required for improvement")
            .MaximumLength(50_000)
            .WithMessage("Content cannot exceed 50,000 characters for improvement");

        RuleFor(x => x.Principles)
            .NotEmpty()
            .WithMessage("At least one Constitutional AI principle is required")
            .Must(HaveValidPrinciples)
            .WithMessage("All principles must be non-empty strings");

        RuleForEach(x => x.Principles)
            .NotEmpty()
            .WithMessage("Principle cannot be empty")
            .MaximumLength(200)
            .WithMessage("Each principle cannot exceed 200 characters");
    }

    private static bool HaveValidPrinciples(IList<string> principles)
    {
        return principles.All(p => !string.IsNullOrWhiteSpace(p));
    }
}

/// <summary>
/// Validator for Constitutional AI critique request
/// </summary>
public class ConstitutionalCritiqueRequestValidator : AbstractValidator<ClaudeController.ConstitutionalCritiqueRequest>
{
    public ConstitutionalCritiqueRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required for critique")
            .MaximumLength(50_000)
            .WithMessage("Content cannot exceed 50,000 characters for critique");

        RuleFor(x => x.Principles)
            .NotEmpty()
            .WithMessage("At least one Constitutional AI principle is required")
            .Must(HaveValidPrinciples)
            .WithMessage("All principles must be non-empty strings");

        RuleForEach(x => x.Principles)
            .NotEmpty()
            .WithMessage("Principle cannot be empty")
            .MaximumLength(200)
            .WithMessage("Each principle cannot exceed 200 characters");
    }

    private static bool HaveValidPrinciples(IList<string> principles)
    {
        return principles.All(p => !string.IsNullOrWhiteSpace(p));
    }
}

/// <summary>
/// Validator for token estimation request
/// </summary>
public class TokenEstimationRequestValidator : AbstractValidator<ClaudeController.TokenEstimationRequest>
{
    public TokenEstimationRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Text is required for token estimation")
            .MaximumLength(1_000_000)
            .WithMessage("Text cannot exceed 1,000,000 characters for estimation");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Model is required")
            .Must(BeValidModel)
            .WithMessage("Invalid model for token estimation");

        RuleFor(x => x.EstimatedOutputTokens)
            .GreaterThan(0)
            .WithMessage("EstimatedOutputTokens must be greater than 0")
            .LessThanOrEqualTo(4096)
            .WithMessage("EstimatedOutputTokens cannot exceed 4096")
            .When(x => x.EstimatedOutputTokens.HasValue);
    }

    private static bool BeValidModel(string model)
    {
        var validModels = new[]
        {
            "claude-3-opus-20240229",
            "claude-3-sonnet-20240229",
            "claude-3-haiku-20240307",
            "claude-2.1",
            "claude-2.0",
            "claude-instant-1.2"
        };

        return validModels.Contains(model);
    }
}

/// <summary>
/// Custom validation attributes
/// </summary>
public class ValidClaudeModelAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string model)
        {
            return new ValidationResult("Model must be a string");
        }

        var validModels = new[]
        {
            "claude-3-opus-20240229",
            "claude-3-sonnet-20240229",
            "claude-3-haiku-20240307",
            "claude-2.1",
            "claude-2.0",
            "claude-instant-1.2"
        };

        if (!validModels.Contains(model))
        {
            return new ValidationResult($"Invalid model. Supported models: {string.Join(", ", validModels)}");
        }

        return ValidationResult.Success;
    }
}

/// <summary>
/// Custom validation for Constitutional AI principles
/// </summary>
public class ValidConstitutionalPrinciplesAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not List<string> principles)
        {
            return new ValidationResult("Principles must be a list of strings");
        }

        if (principles.Count == 0)
        {
            return new ValidationResult("At least one Constitutional AI principle is required");
        }

        if (principles.Any(string.IsNullOrWhiteSpace))
        {
            return new ValidationResult("All principles must be non-empty strings");
        }

        if (principles.Any(p => p.Length > 200))
        {
            return new ValidationResult("Each principle cannot exceed 200 characters");
        }

        return ValidationResult.Success;
    }
}

/// <summary>
/// Custom validation for message role sequence
/// </summary>
public class ValidMessageSequenceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not List<ClaudeMessage> messages)
        {
            return new ValidationResult("Messages must be a list of ClaudeMessage objects");
        }

        if (messages.Count == 0)
        {
            return new ValidationResult("At least one message is required");
        }

        // First message must be from user
        if (messages.First().Role != "user")
        {
            return new ValidationResult("First message must be from user");
        }

        // Messages should alternate between user and assistant
        for (int i = 1; i < messages.Count; i++)
        {
            var currentRole = messages[i].Role;
            var previousRole = messages[i - 1].Role;

            if (currentRole == previousRole)
            {
                return new ValidationResult($"Messages should alternate between user and assistant. Found consecutive {currentRole} messages at position {i}");
            }

            if (currentRole != "user" && currentRole != "assistant")
            {
                return new ValidationResult($"Invalid role '{currentRole}' at position {i}. Role must be either 'user' or 'assistant'");
            }
        }

        return ValidationResult.Success;
    }
}