using System.Collections.Concurrent;
using Module_01_Containerization.Models;
using Microsoft.Extensions.Options;

namespace Module_01_Containerization.Services;

public sealed class DockerfileTemplateService
{
    private readonly ConcurrentDictionary<string, DockerfileTemplate> _templates = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<DockerfileTemplateService> _logger;

    public DockerfileTemplateService(IOptions<ContainerizationOptions> options, ILogger<DockerfileTemplateService> logger)
    {
        _logger = logger;

        foreach (var template in options.Value.Templates)
        {
            var clone = Clone(template);
            _templates[clone.Name] = clone;
        }
    }

    public IReadOnlyCollection<DockerfileTemplate> GetAll()
    {
        return _templates.Values
            .Select(Clone)
            .OrderBy(t => t.Name)
            .ToArray();
    }

    public DockerfileTemplate? Get(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return _templates.TryGetValue(name, out var template) ? Clone(template) : null;
    }

    public DockerfileTemplate Add(DockerfileTemplate template)
    {
        if (string.IsNullOrWhiteSpace(template.Name))
        {
            template.Name = $"custom-{Guid.NewGuid():N}";
        }

        var clone = Clone(template);
        _templates[clone.Name] = clone;
        _logger.LogInformation("Dockerfile template {Template} registered", clone.Name);
        return Clone(clone);
    }

    public bool Delete(string name)
    {
        return _templates.TryRemove(name, out _);
    }

    private static DockerfileTemplate Clone(DockerfileTemplate template)
    {
        return new DockerfileTemplate
        {
            Name = template.Name,
            Description = template.Description,
            BaseImage = template.BaseImage,
            MultiStage = template.MultiStage,
            BuildStageSteps = new List<string>(template.BuildStageSteps),
            RuntimeStageSteps = new List<string>(template.RuntimeStageSteps),
            ExposedPorts = new List<string>(template.ExposedPorts),
            Labels = new List<string>(template.Labels),
            Arguments = new Dictionary<string, string>(template.Arguments),
            OptimizationTips = new List<string>(template.OptimizationTips)
        };
    }
}
