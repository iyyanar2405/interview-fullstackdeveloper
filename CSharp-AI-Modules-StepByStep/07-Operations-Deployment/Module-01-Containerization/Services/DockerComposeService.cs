using Module_01_Containerization.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Module_01_Containerization.Services;

public sealed class DockerComposeService
{
    private readonly ISerializer _serializer;

    public DockerComposeService()
    {
        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.Preserve)
            .Build();
    }

    public string GenerateCompose(ComposeProfile profile, ContainerBuildRequest request)
    {
        var node = new Dictionary<string, object>
        {
            ["version"] = "3.9",
            ["services"] = profile.Services.ToDictionary(
                s => s.ServiceName,
                s => BuildServiceDefinition(s, request))
        };

        return _serializer.Serialize(node);
    }

    private static Dictionary<string, object> BuildServiceDefinition(ComposeServiceConfig service, ContainerBuildRequest request)
    {
        var dictionary = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(service.Image))
        {
            dictionary["image"] = service.Image;
        }

        if (!string.IsNullOrWhiteSpace(service.BuildContext))
        {
            dictionary["build"] = new Dictionary<string, object>
            {
                ["context"] = service.BuildContext,
                ["args"] = request.BuildArguments.Count > 0
                    ? request.BuildArguments.ToDictionary(arg => arg, _ => (object)string.Empty)
                    : null
            };
        }

        if (service.Environment.Count > 0 || request.Environment.Count > 0)
        {
            dictionary["environment"] = service.Environment
                .Concat(request.Environment)
                .GroupBy(kv => kv.Key)
                .ToDictionary(g => g.Key, g => g.Last().Value);
        }

        if (service.Ports.Count > 0 || request.Ports.Count > 0)
        {
            dictionary["ports"] = service.Ports
                .Concat(request.Ports)
                .Distinct()
                .Select(p => $"{p}:{p}")
                .ToList();
        }

        if (service.DependsOn.Count > 0)
        {
            dictionary["depends_on"] = service.DependsOn;
        }

        if (service.Volumes.Count > 0)
        {
            dictionary["volumes"] = service.Volumes.Select(kv => $"{kv.Key}:{kv.Value}").ToList();
        }

        if (service.Labels.Count > 0)
        {
            dictionary["labels"] = service.Labels;
        }

        if (service.Command.Count > 0)
        {
            dictionary["command"] = service.Command;
        }

        return dictionary;
    }
}
