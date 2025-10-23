using Microsoft.Extensions.Options;
using AI.Configuration.Models;
using System.ComponentModel.DataAnnotations;

namespace AI.Configuration.Services;

/// <summary>
/// Service for managing application configuration
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Get OpenAI settings
    /// </summary>
    OpenAISettings GetOpenAISettings();

    /// <summary>
    /// Get database settings
    /// </summary>
    DatabaseSettings GetDatabaseSettings();

    /// <summary>
    /// Get security settings
    /// </summary>
    SecuritySettings GetSecuritySettings();

    /// <summary>
    /// Get application settings
    /// </summary>
    ApplicationSettings GetApplicationSettings();

    /// <summary>
    /// Validate all configuration settings
    /// </summary>
    Task<ConfigurationValidationResult> ValidateAllSettingsAsync();

    /// <summary>
    /// Check if a feature is enabled
    /// </summary>
    bool IsFeatureEnabled(string featureName);

    /// <summary>
    /// Get configuration value by key
    /// </summary>
    T? GetValue<T>(string key);

    /// <summary>
    /// Update configuration at runtime (if supported)
    /// </summary>
    Task<bool> UpdateConfigurationAsync(string section, object newSettings);
}

/// <summary>
/// Implementation of configuration service
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly IOptionsMonitor<OpenAISettings> _openAIOptions;
    private readonly IOptionsMonitor<DatabaseSettings> _databaseOptions;
    private readonly IOptionsMonitor<SecuritySettings> _securityOptions;
    private readonly IOptionsMonitor<ApplicationSettings> _applicationOptions;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(
        IOptionsMonitor<OpenAISettings> openAIOptions,
        IOptionsMonitor<DatabaseSettings> databaseOptions,
        IOptionsMonitor<SecuritySettings> securityOptions,
        IOptionsMonitor<ApplicationSettings> applicationOptions,
        IConfiguration configuration,
        ILogger<ConfigurationService> logger)
    {
        _openAIOptions = openAIOptions;
        _databaseOptions = databaseOptions;
        _securityOptions = securityOptions;
        _applicationOptions = applicationOptions;
        _configuration = configuration;
        _logger = logger;
    }

    public OpenAISettings GetOpenAISettings()
    {
        var settings = _openAIOptions.CurrentValue;
        _logger.LogDebug("Retrieved OpenAI settings with model: {Model}", settings.Model);
        return settings;
    }

    public DatabaseSettings GetDatabaseSettings()
    {
        var settings = _databaseOptions.CurrentValue;
        _logger.LogDebug("Retrieved database settings for provider: {Provider}", settings.Provider);
        return settings;
    }

    public SecuritySettings GetSecuritySettings()
    {
        var settings = _securityOptions.CurrentValue;
        _logger.LogDebug("Retrieved security settings");
        return settings;
    }

    public ApplicationSettings GetApplicationSettings()
    {
        var settings = _applicationOptions.CurrentValue;
        _logger.LogDebug("Retrieved application settings for environment: {Environment}", settings.Environment);
        return settings;
    }

    public async Task<ConfigurationValidationResult> ValidateAllSettingsAsync()
    {
        var result = new ConfigurationValidationResult();

        try
        {
            // Validate OpenAI settings
            var openAISettings = GetOpenAISettings();
            if (!openAISettings.IsValid(out var openAIErrors))
            {
                result.AddErrors("OpenAI", openAIErrors);
            }

            // Validate database settings
            var databaseSettings = GetDatabaseSettings();
            await ValidateDatabaseConnectionAsync(databaseSettings, result);

            // Validate security settings
            var securitySettings = GetSecuritySettings();
            ValidateSecuritySettings(securitySettings, result);

            // Validate application settings
            var applicationSettings = GetApplicationSettings();
            ValidateApplicationSettings(applicationSettings, result);

            _logger.LogInformation("Configuration validation completed. Valid: {IsValid}, Errors: {ErrorCount}",
                result.IsValid, result.ValidationErrors.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during configuration validation");
            result.AddError("General", "Configuration validation failed with exception: " + ex.Message);
        }

        return result;
    }

    public bool IsFeatureEnabled(string featureName)
    {
        var appSettings = GetApplicationSettings();
        var features = appSettings.Features;

        return featureName.ToLowerInvariant() switch
        {
            "aichat" => features.EnableAIChat,
            "documentprocessing" => features.EnableDocumentProcessing,
            "functioncalling" => features.EnableFunctionCalling,
            "streamingresponses" => features.EnableStreamingResponses,
            "advancedanalytics" => features.EnableAdvancedAnalytics,
            "abtesting" => features.EnableABTesting,
            "betafeatures" => features.EnableBetaFeatures,
            _ => false
        };
    }

    public T? GetValue<T>(string key)
    {
        return _configuration.GetValue<T>(key);
    }

    public async Task<bool> UpdateConfigurationAsync(string section, object newSettings)
    {
        // Note: This is a placeholder for runtime configuration updates
        // Implementation would depend on the configuration provider used
        _logger.LogWarning("Runtime configuration updates not implemented for section: {Section}", section);
        await Task.CompletedTask;
        return false;
    }

    private async Task ValidateDatabaseConnectionAsync(DatabaseSettings settings, ConfigurationValidationResult result)
    {
        try
        {
            // Simulate database connection validation
            // In real implementation, you would test the actual connection
            if (string.IsNullOrWhiteSpace(settings.PrimaryConnectionString))
            {
                result.AddError("Database", "Primary connection string is required");
            }

            if (settings.CommandTimeoutSeconds <= 0)
            {
                result.AddError("Database", "Command timeout must be greater than 0");
            }

            // Simulate connection test
            await Task.Delay(100);
            _logger.LogDebug("Database connection validation completed");
        }
        catch (Exception ex)
        {
            result.AddError("Database", $"Database connection validation failed: {ex.Message}");
        }
    }

    private void ValidateSecuritySettings(SecuritySettings settings, ConfigurationValidationResult result)
    {
        // Validate JWT settings
        if (string.IsNullOrWhiteSpace(settings.Jwt.SecretKey))
        {
            result.AddError("Security", "JWT SecretKey is required");
        }
        else if (settings.Jwt.SecretKey.Length < 32)
        {
            result.AddError("Security", "JWT SecretKey must be at least 32 characters");
        }

        if (string.IsNullOrWhiteSpace(settings.Jwt.Issuer))
        {
            result.AddError("Security", "JWT Issuer is required");
        }

        if (string.IsNullOrWhiteSpace(settings.Jwt.Audience))
        {
            result.AddError("Security", "JWT Audience is required");
        }

        // Validate API key settings
        if (settings.ApiKeys.Enabled && !settings.ApiKeys.ValidKeys.Any())
        {
            result.AddError("Security", "At least one valid API key is required when API key authentication is enabled");
        }

        // Validate rate limiting
        if (settings.RateLimit.Enabled)
        {
            if (settings.RateLimit.General.RequestsPerMinute <= 0)
            {
                result.AddError("Security", "RequestsPerMinute must be greater than 0");
            }
        }
    }

    private void ValidateApplicationSettings(ApplicationSettings settings, ConfigurationValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(settings.Name))
        {
            result.AddError("Application", "Application name is required");
        }

        if (string.IsNullOrWhiteSpace(settings.Version))
        {
            result.AddError("Application", "Application version is required");
        }

        if (!Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out _))
        {
            result.AddError("Application", "BaseUrl must be a valid URL");
        }

        // Validate monitoring settings
        if (settings.EnablePerformanceMonitoring && settings.Monitoring.TraceSampleRate < 0 || settings.Monitoring.TraceSampleRate > 1)
        {
            result.AddError("Application", "TraceSampleRate must be between 0.0 and 1.0");
        }
    }
}

/// <summary>
/// Configuration validation result
/// </summary>
public class ConfigurationValidationResult
{
    public bool IsValid => ValidationErrors.Count == 0;
    public Dictionary<string, List<string>> ValidationErrors { get; } = new();
    public DateTime ValidationTimestamp { get; } = DateTime.UtcNow;

    public void AddError(string section, string error)
    {
        if (!ValidationErrors.ContainsKey(section))
        {
            ValidationErrors[section] = new List<string>();
        }
        ValidationErrors[section].Add(error);
    }

    public void AddErrors(string section, List<string> errors)
    {
        if (!ValidationErrors.ContainsKey(section))
        {
            ValidationErrors[section] = new List<string>();
        }
        ValidationErrors[section].AddRange(errors);
    }

    public string GetErrorSummary()
    {
        if (IsValid)
            return "All configuration settings are valid.";

        var errors = ValidationErrors
            .SelectMany(kvp => kvp.Value.Select(error => $"{kvp.Key}: {error}"))
            .ToList();

        return $"Configuration validation failed with {errors.Count} error(s):\n" + 
               string.Join("\n", errors);
    }
}