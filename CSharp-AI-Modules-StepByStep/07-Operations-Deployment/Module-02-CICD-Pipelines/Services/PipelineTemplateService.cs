using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Module_02_CICD_Pipelines.Models;

namespace Module_02_CICD_Pipelines.Services;

public sealed class PipelineTemplateService
{
    private readonly ConcurrentDictionary<string, PipelineTemplate> _templates = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<PipelineTemplateService> _logger;

    public PipelineTemplateService(IOptions<PipelineOptions> options, ILogger<PipelineTemplateService> logger)
    {
        _logger = logger;
        foreach (var template in options.Value.Templates)
        {
            var clone = Clone(template);
            _templates[clone.Name] = clone;
        }
    }

    public IReadOnlyCollection<PipelineTemplate> GetAll()
    {
        return _templates.Values
            .Select(Clone)
            .OrderBy(t => t.Name)
            .ToArray();
    }

    public PipelineTemplate? Get(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return _templates.TryGetValue(name, out var template) ? Clone(template) : null;
    }

    public PipelineTemplate Add(PipelineTemplate template)
    {
        if (string.IsNullOrWhiteSpace(template.Name))
        {
            template.Name = $"pipeline-{Guid.NewGuid():N}";
        }

        var clone = Clone(template);
        _templates[clone.Name] = clone;
        _logger.LogInformation("Pipeline template {Template} registered", clone.Name);
        return Clone(clone);
    }

    public bool Delete(string name)
    {
        return _templates.TryRemove(name, out _);
    }

    private static PipelineTemplate Clone(PipelineTemplate template)
    {
        return new PipelineTemplate
        {
            Name = template.Name,
            Provider = template.Provider,
            Description = template.Description,
            Trigger = template.Trigger,
            Variables = new Dictionary<string, string>(template.Variables),
            Jobs = template.Jobs.Select(job => new PipelineJobTemplate
            {
                Name = job.Name,
                RunsOn = job.RunsOn,
                Steps = new List<string>(job.Steps),
                Environment = new Dictionary<string, string>(job.Environment)
            }).ToList(),
            QualityChecks = new List<string>(template.QualityChecks),
            Notifications = new List<string>(template.Notifications)
        };
    }
}
