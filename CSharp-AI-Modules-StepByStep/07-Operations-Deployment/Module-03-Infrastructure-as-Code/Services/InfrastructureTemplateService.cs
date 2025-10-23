using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Options;
using Module_03_Infrastructure_as_Code.Models;

namespace Module_03_Infrastructure_as_Code.Services;

public sealed class InfrastructureTemplateService
{
    private readonly ConcurrentDictionary<string, IaCTemplate> _templates = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<InfrastructureTemplateService> _logger;

    public InfrastructureTemplateService(IOptions<InfrastructureOptions> options, ILogger<InfrastructureTemplateService> logger)
    {
        _logger = logger;
        foreach (var template in options.Value.Templates)
        {
            var clone = Clone(template);
            _templates[clone.Name] = clone;
        }

        if (_templates.IsEmpty)
        {
            _logger.LogWarning("No infrastructure templates were configured. Seeding fallback template.");
            var fallback = new IaCTemplate
            {
                Name = "default",
                Provider = options.Value.Simulation.DefaultProvider,
                Description = "Fallback infrastructure template",
                Resources =
                {
                    new IaCResourceTemplate
                    {
                        Name = "app-service",
                        Type = "azurerm_app_service",
                        Properties =
                        {
                            ["sku"] = "P1v3",
                            ["https_only"] = "true"
                        },
                        SecurityControls =
                        {
                            ["https_only"] = "required"
                        }
                    }
                },
                Variables =
                {
                    ["location"] = options.Value.Simulation.DefaultRegion
                }
            };

            _templates[fallback.Name] = fallback;
        }
    }

    public IReadOnlyCollection<IaCTemplate> GetAll()
    {
        return _templates.Values.Select(Clone).ToArray();
    }

    public IaCTemplate? Get(string name)
    {
        return _templates.TryGetValue(name, out var template) ? Clone(template) : null;
    }

    public IaCTemplate Upsert(IaCTemplate template)
    {
        var clone = Clone(template);
        _templates[clone.Name] = clone;
        _logger.LogInformation("Template {Template} saved with {ResourceCount} resources", clone.Name, clone.Resources.Count);
        return Clone(clone);
    }

    public void Delete(string name)
    {
        _templates.TryRemove(name, out _);
        _logger.LogInformation("Template {Template} deleted", name);
    }

    private static IaCTemplate Clone(IaCTemplate template)
    {
        return new IaCTemplate
        {
            Name = template.Name,
            Provider = template.Provider,
            Description = template.Description,
            Tags = new List<string>(template.Tags),
            Variables = new Dictionary<string, string>(template.Variables, StringComparer.OrdinalIgnoreCase),
            Resources = template.Resources
                .Select(r => new IaCResourceTemplate
                {
                    Name = r.Name,
                    Type = r.Type,
                    Properties = new Dictionary<string, string>(r.Properties, StringComparer.OrdinalIgnoreCase),
                    SecurityControls = new Dictionary<string, string>(r.SecurityControls, StringComparer.OrdinalIgnoreCase)
                })
                .ToList()
        };
    }
}
