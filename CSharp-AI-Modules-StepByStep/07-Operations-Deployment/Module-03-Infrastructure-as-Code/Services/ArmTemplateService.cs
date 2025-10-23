using System.Linq;
using System.Text.Json;
using Module_03_Infrastructure_as_Code.Models;

namespace Module_03_Infrastructure_as_Code.Services;

public sealed class ArmTemplateService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public string Build(InfrastructurePlan plan)
    {
        var template = new Dictionary<string, object>
        {
            ["$schema"] = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
            ["contentVersion"] = "1.0.0.0",
            ["parameters"] = plan.Variables.ToDictionary(
                kvp => kvp.Key,
                kvp => (object)new Dictionary<string, object>
                {
                    ["type"] = InferParameterType(kvp.Value),
                    ["defaultValue"] = kvp.Value
                }),
            ["resources"] = plan.Resources.Select(resource =>
            {
                var dictionary = new Dictionary<string, object>
                {
                    ["type"] = MapType(resource.Type),
                    ["name"] = resource.Name,
                    ["apiVersion"] = resource.Properties.TryGetValue("api_version", out var version)
                        ? version
                        : "2022-09-01",
                    ["location"] = plan.Region,
                    ["properties"] = resource.Properties
                        .Where(kvp => !string.Equals(kvp.Key, "api_version", StringComparison.OrdinalIgnoreCase))
                        .ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value)
                };

                return dictionary;
            }).ToList()
        };

        return JsonSerializer.Serialize(template, SerializerOptions);
    }

    private static string InferParameterType(string value)
    {
        if (bool.TryParse(value, out _))
        {
            return "bool";
        }

        if (int.TryParse(value, out _))
        {
            return "int";
        }

        return "string";
    }

    private static string MapType(string resourceType)
    {
        if (resourceType.Contains("azurerm", StringComparison.OrdinalIgnoreCase))
        {
            return resourceType
                .Replace("azurerm_", "Microsoft.")
                .Replace("_", "/", StringComparison.OrdinalIgnoreCase);
        }

        return resourceType;
    }
}
