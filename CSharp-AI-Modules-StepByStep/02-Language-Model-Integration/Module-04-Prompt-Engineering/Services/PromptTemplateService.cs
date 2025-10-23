using AI.PromptEngineering.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scriban;
using Scriban.Runtime;
using HandlebarsDotNet;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace AI.PromptEngineering.Services;

/// <summary>
/// Prompt template service interface
/// </summary>
public interface IPromptTemplateService
{
    /// <summary>
    /// Render a prompt template with variables
    /// </summary>
    Task<PromptTemplateResponse> RenderTemplateAsync(PromptTemplateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new prompt template
    /// </summary>
    Task<PromptTemplate> CreateTemplateAsync(PromptTemplate template, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing template
    /// </summary>
    Task<PromptTemplate> UpdateTemplateAsync(string templateId, PromptTemplate template, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a template by ID
    /// </summary>
    Task<PromptTemplate?> GetTemplateAsync(string templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a template
    /// </summary>
    Task<bool> DeleteTemplateAsync(string templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search templates
    /// </summary>
    Task<PromptSearchResponse> SearchTemplatesAsync(PromptSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate a template
    /// </summary>
    Task<PromptValidationResult> ValidateTemplateAsync(PromptTemplate template, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get template versions
    /// </summary>
    Task<List<PromptVersion>> GetTemplateVersionsAsync(string templateId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Prompt template service implementation
/// </summary>
public class PromptTemplateService : IPromptTemplateService
{
    private readonly PromptEngineeringSettings _settings;
    private readonly ILogger<PromptTemplateService> _logger;
    private readonly ConcurrentDictionary<string, PromptTemplate> _templateCache;
    private readonly ConcurrentDictionary<string, List<PromptVersion>> _versionCache;
    private readonly IHandlebars _handlebarsEngine;

    public PromptTemplateService(
        IOptions<PromptEngineeringSettings> settings,
        ILogger<PromptTemplateService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _templateCache = new ConcurrentDictionary<string, PromptTemplate>();
        _versionCache = new ConcurrentDictionary<string, List<PromptVersion>>();
        _handlebarsEngine = Handlebars.Create();
        
        InitializeDefaultTemplates();
    }

    public async Task<PromptTemplateResponse> RenderTemplateAsync(PromptTemplateRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Rendering template {TemplateId}", request.TemplateId);

            var template = await GetTemplateAsync(request.TemplateId, cancellationToken);
            if (template == null)
            {
                throw new ArgumentException($"Template {request.TemplateId} not found");
            }

            // Validate required variables
            ValidateRequiredVariables(template, request.Variables);

            // Merge with default values
            var variables = MergeVariablesWithDefaults(template, request.Variables);

            // Render template based on engine
            var renderedPrompt = template.Engine switch
            {
                TemplateEngine.Scriban => await RenderScribanTemplateAsync(template.Content, variables),
                TemplateEngine.Handlebars => await RenderHandlebarsTemplateAsync(template.Content, variables),
                TemplateEngine.Simple => await RenderSimpleTemplateAsync(template.Content, variables),
                _ => throw new NotSupportedException($"Template engine {template.Engine} not supported")
            };

            // Apply optimization if requested
            if (request.OptimizationOptions != null)
            {
                var optimizationService = new PromptOptimizationService(_logger);
                var optimizationResult = await optimizationService.OptimizePromptAsync(
                    renderedPrompt, request.OptimizationOptions);
                renderedPrompt = optimizationResult.OptimizedPrompt;
            }

            // Calculate metrics
            var metrics = await CalculatePromptMetricsAsync(renderedPrompt);

            var response = new PromptTemplateResponse
            {
                TemplateId = template.Id,
                TemplateName = template.Name,
                RenderedPrompt = renderedPrompt,
                UsedVariables = variables.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                TokenCount = metrics.TokenCount,
                EstimatedCost = metrics.EstimatedCost,
                GeneratedAt = DateTime.UtcNow,
                Metrics = metrics
            };

            _logger.LogInformation("Template {TemplateId} rendered successfully. Tokens: {TokenCount}",
                template.Id, metrics.TokenCount);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering template {TemplateId}", request.TemplateId);
            throw;
        }
    }

    public async Task<PromptTemplate> CreateTemplateAsync(PromptTemplate template, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Creating new template: {TemplateName}", template.Name);

            // Validate template
            var validation = await ValidateTemplateAsync(template, cancellationToken);
            if (!validation.IsValid)
            {
                throw new ArgumentException($"Template validation failed: {string.Join(", ", validation.Issues.Select(i => i.Message))}");
            }

            // Ensure unique ID
            template.Id = Guid.NewGuid().ToString();
            template.CreatedAt = DateTime.UtcNow;
            template.UpdatedAt = DateTime.UtcNow;

            // Store template
            _templateCache[template.Id] = template;

            // Create initial version
            if (_settings.EnableVersioning)
            {
                await CreateVersionAsync(template, "Initial version");
            }

            _logger.LogInformation("Template {TemplateId} created successfully", template.Id);
            return template;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating template");
            throw;
        }
    }

    public async Task<PromptTemplate> UpdateTemplateAsync(string templateId, PromptTemplate template, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating template {TemplateId}", templateId);

            var existingTemplate = await GetTemplateAsync(templateId, cancellationToken);
            if (existingTemplate == null)
            {
                throw new ArgumentException($"Template {templateId} not found");
            }

            // Validate updated template
            var validation = await ValidateTemplateAsync(template, cancellationToken);
            if (!validation.IsValid)
            {
                throw new ArgumentException($"Template validation failed: {string.Join(", ", validation.Issues.Select(i => i.Message))}");
            }

            // Preserve ID and created date
            template.Id = templateId;
            template.CreatedAt = existingTemplate.CreatedAt;
            template.UpdatedAt = DateTime.UtcNow;

            // Increment version
            var versionParts = existingTemplate.Version.Split('.');
            if (versionParts.Length == 3 && int.TryParse(versionParts[2], out var patch))
            {
                template.Version = $"{versionParts[0]}.{versionParts[1]}.{patch + 1}";
            }

            // Update template
            _templateCache[templateId] = template;

            // Create version
            if (_settings.EnableVersioning)
            {
                await CreateVersionAsync(template, "Template updated");
            }

            _logger.LogInformation("Template {TemplateId} updated to version {Version}", templateId, template.Version);
            return template;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {TemplateId}", templateId);
            throw;
        }
    }

    public async Task<PromptTemplate?> GetTemplateAsync(string templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;
            
            if (_templateCache.TryGetValue(templateId, out var template))
            {
                return template;
            }

            _logger.LogWarning("Template {TemplateId} not found", templateId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving template {TemplateId}", templateId);
            throw;
        }
    }

    public async Task<bool> DeleteTemplateAsync(string templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            var removed = _templateCache.TryRemove(templateId, out _);
            if (removed)
            {
                _versionCache.TryRemove(templateId, out _);
                _logger.LogInformation("Template {TemplateId} deleted", templateId);
            }

            return removed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {TemplateId}", templateId);
            throw;
        }
    }

    public async Task<PromptSearchResponse> SearchTemplatesAsync(PromptSearchRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            _logger.LogDebug("Searching templates with query: {Query}", request.Query);

            var templates = _templateCache.Values.AsEnumerable();

            // Apply filters
            if (!request.IncludeInactive)
            {
                templates = templates.Where(t => t.IsActive);
            }

            if (!string.IsNullOrEmpty(request.Category))
            {
                templates = templates.Where(t => t.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase));
            }

            if (request.Tags?.Any() == true)
            {
                templates = templates.Where(t => t.Tags.Any(tag => request.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(request.Query))
            {
                templates = templates.Where(t =>
                    t.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase) ||
                    t.Description.Contains(request.Query, StringComparison.OrdinalIgnoreCase) ||
                    t.Content.Contains(request.Query, StringComparison.OrdinalIgnoreCase));
            }

            // Apply sorting
            templates = request.SortBy switch
            {
                PromptSortBy.Name => templates.OrderBy(t => t.Name),
                PromptSortBy.CreatedDate => templates.OrderByDescending(t => t.CreatedAt),
                PromptSortBy.UpdatedDate => templates.OrderByDescending(t => t.UpdatedAt),
                _ => templates
            };

            // Apply pagination
            var totalCount = templates.Count();
            var paginatedTemplates = templates
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PromptSearchResponse
            {
                Templates = paginatedTemplates,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching templates");
            throw;
        }
    }

    public async Task<PromptValidationResult> ValidateTemplateAsync(PromptTemplate template, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            var result = new PromptValidationResult { IsValid = true };

            // Validate basic properties
            if (string.IsNullOrWhiteSpace(template.Name))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_NAME",
                    Message = "Template name is required",
                    Severity = ValidationSeverity.Error
                });
                result.IsValid = false;
            }

            if (string.IsNullOrWhiteSpace(template.Content))
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "MISSING_CONTENT",
                    Message = "Template content is required",
                    Severity = ValidationSeverity.Error
                });
                result.IsValid = false;
            }

            // Validate template syntax
            try
            {
                var testVariables = template.Variables
                    .ToDictionary(v => v.Name, v => v.DefaultValue ?? "test");

                _ = template.Engine switch
                {
                    TemplateEngine.Scriban => Template.Parse(template.Content),
                    TemplateEngine.Handlebars => _handlebarsEngine.Compile(template.Content),
                    _ => template.Content
                };
            }
            catch (Exception ex)
            {
                result.Issues.Add(new ValidationIssue
                {
                    Code = "SYNTAX_ERROR",
                    Message = $"Template syntax error: {ex.Message}",
                    Severity = ValidationSeverity.Error
                });
                result.IsValid = false;
            }

            // Validate variables
            var undeclaredVars = FindUndeclaredVariables(template);
            if (undeclaredVars.Any())
            {
                result.Warnings.Add($"Undeclared variables found: {string.Join(", ", undeclaredVars)}");
            }

            // Check for best practices
            if (template.Content.Length > 10000)
            {
                result.Warnings.Add("Template is very long. Consider breaking it into smaller templates.");
            }

            // Calculate metrics
            result.Metrics = await CalculatePromptMetricsAsync(template.Content);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating template");
            throw;
        }
    }

    public async Task<List<PromptVersion>> GetTemplateVersionsAsync(string templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            if (_versionCache.TryGetValue(templateId, out var versions))
            {
                return versions.OrderByDescending(v => v.CreatedAt).ToList();
            }

            return new List<PromptVersion>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving template versions for {TemplateId}", templateId);
            throw;
        }
    }

    private async Task<string> RenderScribanTemplateAsync(string content, Dictionary<string, object> variables)
    {
        try
        {
            var template = Template.Parse(content);
            var scriptObject = new ScriptObject();
            
            foreach (var variable in variables)
            {
                scriptObject.Add(variable.Key, variable.Value);
            }

            var context = new TemplateContext();
            context.PushGlobal(scriptObject);

            return await template.RenderAsync(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering Scriban template");
            throw;
        }
    }

    private async Task<string> RenderHandlebarsTemplateAsync(string content, Dictionary<string, object> variables)
    {
        try
        {
            await Task.CompletedTask;
            var template = _handlebarsEngine.Compile(content);
            return template(variables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering Handlebars template");
            throw;
        }
    }

    private async Task<string> RenderSimpleTemplateAsync(string content, Dictionary<string, object> variables)
    {
        try
        {
            await Task.CompletedTask;
            
            var result = content;
            foreach (var variable in variables)
            {
                var placeholder = $"{{{{{variable.Key}}}}}";
                result = result.Replace(placeholder, variable.Value?.ToString() ?? string.Empty);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering simple template");
            throw;
        }
    }

    private static void ValidateRequiredVariables(PromptTemplate template, Dictionary<string, object> variables)
    {
        var requiredVariables = template.Variables.Where(v => v.Required).Select(v => v.Name);
        var missingVariables = requiredVariables.Except(variables.Keys);

        if (missingVariables.Any())
        {
            throw new ArgumentException($"Missing required variables: {string.Join(", ", missingVariables)}");
        }
    }

    private static Dictionary<string, object> MergeVariablesWithDefaults(PromptTemplate template, Dictionary<string, object> variables)
    {
        var merged = new Dictionary<string, object>(variables);

        foreach (var templateVar in template.Variables.Where(v => v.DefaultValue != null))
        {
            if (!merged.ContainsKey(templateVar.Name))
            {
                merged[templateVar.Name] = templateVar.DefaultValue!;
            }
        }

        return merged;
    }

    private static List<string> FindUndeclaredVariables(PromptTemplate template)
    {
        var declaredVars = template.Variables.Select(v => v.Name).ToHashSet();
        var pattern = template.Engine switch
        {
            TemplateEngine.Scriban => @"\{\{(\w+)\}\}",
            TemplateEngine.Handlebars => @"\{\{(\w+)\}\}",
            TemplateEngine.Simple => @"\{\{(\w+)\}\}",
            _ => @"\{\{(\w+)\}\}"
        };

        var matches = Regex.Matches(template.Content, pattern);
        var usedVars = matches.Select(m => m.Groups[1].Value).Distinct();

        return usedVars.Except(declaredVars).ToList();
    }

    private async Task<PromptMetrics> CalculatePromptMetricsAsync(string prompt)
    {
        await Task.CompletedTask;

        return new PromptMetrics
        {
            CharacterCount = prompt.Length,
            WordCount = prompt.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length,
            TokenCount = EstimateTokenCount(prompt),
            ReadabilityScore = CalculateReadabilityScore(prompt),
            ClarityScore = CalculateClarityScore(prompt),
            EstimatedCost = EstimateCost(prompt)
        };
    }

    private static int EstimateTokenCount(string text)
    {
        // Simple estimation: ~4 characters per token
        return (int)Math.Ceiling(text.Length / 4.0);
    }

    private static double CalculateReadabilityScore(string text)
    {
        // Simple readability calculation (Flesch Reading Ease approximation)
        var words = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (sentences.Length == 0) return 100.0;

        var avgWordsPerSentence = words.Length / (double)sentences.Length;
        var avgSyllablesPerWord = words.Average(w => EstimateSyllables(w));

        var score = 206.835 - 1.015 * avgWordsPerSentence - 84.6 * avgSyllablesPerWord;
        return Math.Max(0, Math.Min(100, score));
    }

    private static int EstimateSyllables(string word)
    {
        var vowels = "aeiouy";
        var syllables = 0;
        var previousWasVowel = false;

        foreach (var c in word.ToLower())
        {
            var isVowel = vowels.Contains(c);
            if (isVowel && !previousWasVowel)
            {
                syllables++;
            }
            previousWasVowel = isVowel;
        }

        return Math.Max(1, syllables);
    }

    private static double CalculateClarityScore(string text)
    {
        // Simple clarity score based on sentence structure and complexity
        var score = 100.0;

        // Penalize very long sentences
        var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        var avgSentenceLength = sentences.Average(s => s.Length);
        if (avgSentenceLength > 100) score -= 20;
        else if (avgSentenceLength > 150) score -= 40;

        // Penalize excessive punctuation
        var punctuationCount = text.Count(c => ".,;:!?".Contains(c));
        var punctuationRatio = punctuationCount / (double)text.Length;
        if (punctuationRatio > 0.1) score -= 15;

        return Math.Max(0, Math.Min(100, score));
    }

    private static double EstimateCost(string prompt, string model = "gpt-4")
    {
        var tokens = EstimateTokenCount(prompt);
        var costPer1KTokens = 0.03; // Default GPT-4 pricing
        return (tokens / 1000.0) * costPer1KTokens;
    }

    private async Task CreateVersionAsync(PromptTemplate template, string changeDescription)
    {
        try
        {
            await Task.CompletedTask;

            var version = new PromptVersion
            {
                VersionNumber = template.Version,
                TemplateId = template.Id,
                Content = template.Content,
                ChangeDescription = changeDescription,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var versions = _versionCache.GetOrAdd(template.Id, _ => new List<PromptVersion>());
            
            // Deactivate previous versions
            foreach (var v in versions)
            {
                v.IsActive = false;
            }

            versions.Add(version);
            _logger.LogDebug("Version {Version} created for template {TemplateId}", version.VersionNumber, template.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating version for template {TemplateId}", template.Id);
        }
    }

    private void InitializeDefaultTemplates()
    {
        // Create some default templates
        var summaryTemplate = new PromptTemplate
        {
            Id = "summary-template",
            Name = "Text Summarization",
            Description = "Generate a concise summary of the provided text",
            Category = "Summarization",
            Tags = new List<string> { "summary", "condensation", "text-processing" },
            Engine = TemplateEngine.Scriban,
            Content = @"Please provide a concise summary of the following text:

Text: {{text}}

Requirements:
- Length: {{length}} sentences
{{if style}}- Style: {{style}}{{end}}
{{if focus}}- Focus on: {{focus}}{{end}}

Summary:",
            Variables = new List<TemplateVariable>
            {
                new() { Name = "text", Type = "string", Description = "Text to summarize", Required = true },
                new() { Name = "length", Type = "number", Description = "Number of sentences", Required = false, DefaultValue = "3" },
                new() { Name = "style", Type = "string", Description = "Summary style", Required = false },
                new() { Name = "focus", Type = "string", Description = "Focus area", Required = false }
            }
        };

        _templateCache[summaryTemplate.Id] = summaryTemplate;

        _logger.LogInformation("Initialized {Count} default templates", _templateCache.Count);
    }
}