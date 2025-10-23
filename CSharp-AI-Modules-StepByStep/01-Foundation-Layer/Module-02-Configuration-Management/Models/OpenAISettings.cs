using System.ComponentModel.DataAnnotations;

namespace AI.Configuration.Models;

/// <summary>
/// OpenAI API configuration settings
/// </summary>
public class OpenAISettings
{
    public const string SectionName = "OpenAI";

    /// <summary>
    /// OpenAI API key
    /// </summary>
    [Required(ErrorMessage = "OpenAI API key is required")]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Base URL for OpenAI API
    /// </summary>
    [Url(ErrorMessage = "OpenAI BaseUrl must be a valid URL")]
    public string BaseUrl { get; set; } = "https://api.openai.com/v1";

    /// <summary>
    /// Default model to use
    /// </summary>
    [Required(ErrorMessage = "OpenAI Model is required")]
    public string Model { get; set; } = "gpt-3.5-turbo";

    /// <summary>
    /// Maximum tokens for responses
    /// </summary>
    [Range(1, 4000, ErrorMessage = "MaxTokens must be between 1 and 4000")]
    public int MaxTokens { get; set; } = 1000;

    /// <summary>
    /// Temperature for response generation (0.0 to 2.0)
    /// </summary>
    [Range(0.0, 2.0, ErrorMessage = "Temperature must be between 0.0 and 2.0")]
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Top-p for nucleus sampling
    /// </summary>
    [Range(0.0, 1.0, ErrorMessage = "TopP must be between 0.0 and 1.0")]
    public double TopP { get; set; } = 1.0;

    /// <summary>
    /// Frequency penalty
    /// </summary>
    [Range(-2.0, 2.0, ErrorMessage = "FrequencyPenalty must be between -2.0 and 2.0")]
    public double FrequencyPenalty { get; set; } = 0.0;

    /// <summary>
    /// Presence penalty
    /// </summary>
    [Range(-2.0, 2.0, ErrorMessage = "PresencePenalty must be between -2.0 and 2.0")]
    public double PresencePenalty { get; set; } = 0.0;

    /// <summary>
    /// Timeout for API requests in seconds
    /// </summary>
    [Range(1, 300, ErrorMessage = "TimeoutSeconds must be between 1 and 300")]
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum retry attempts
    /// </summary>
    [Range(0, 10, ErrorMessage = "MaxRetryAttempts must be between 0 and 10")]
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Organization ID (optional)
    /// </summary>
    public string? OrganizationId { get; set; }

    /// <summary>
    /// Available models for this configuration
    /// </summary>
    public List<string> AvailableModels { get; set; } = new()
    {
        "gpt-3.5-turbo",
        "gpt-3.5-turbo-16k",
        "gpt-4",
        "gpt-4-32k"
    };

    /// <summary>
    /// Validate the configuration
    /// </summary>
    public bool IsValid(out List<string> errors)
    {
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(ApiKey))
            errors.Add("API Key is required");

        if (string.IsNullOrWhiteSpace(Model))
            errors.Add("Model is required");

        if (MaxTokens <= 0 || MaxTokens > 4000)
            errors.Add("MaxTokens must be between 1 and 4000");

        if (Temperature < 0.0 || Temperature > 2.0)
            errors.Add("Temperature must be between 0.0 and 2.0");

        if (TimeoutSeconds <= 0 || TimeoutSeconds > 300)
            errors.Add("TimeoutSeconds must be between 1 and 300");

        return errors.Count == 0;
    }
}

/// <summary>
/// Azure OpenAI specific settings
/// </summary>
public class AzureOpenAISettings
{
    public const string SectionName = "Azure:OpenAI";

    /// <summary>
    /// Azure OpenAI endpoint
    /// </summary>
    [Required(ErrorMessage = "Azure OpenAI Endpoint is required")]
    [Url(ErrorMessage = "Endpoint must be a valid URL")]
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Azure OpenAI API key
    /// </summary>
    [Required(ErrorMessage = "Azure OpenAI API key is required")]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Deployment name
    /// </summary>
    [Required(ErrorMessage = "Deployment name is required")]
    public string DeploymentName { get; set; } = string.Empty;

    /// <summary>
    /// API version
    /// </summary>
    public string ApiVersion { get; set; } = "2024-02-01";

    /// <summary>
    /// Use managed identity instead of API key
    /// </summary>
    public bool UseManagedIdentity { get; set; } = false;

    /// <summary>
    /// Tenant ID for managed identity
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Client ID for managed identity
    /// </summary>
    public string? ClientId { get; set; }
}