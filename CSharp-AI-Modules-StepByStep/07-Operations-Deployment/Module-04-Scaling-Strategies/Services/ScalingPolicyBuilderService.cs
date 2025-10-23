using System.Text;
using Module_04_Scaling_Strategies.Models;

namespace Module_04_Scaling_Strategies.Services;

public sealed class ScalingPolicyBuilderService
{
    public ScalingPolicyDocument BuildPolicy(ScalingProfile profile, CapacityForecast forecast)
    {
        var provider = profile.ResourceType switch
        {
            "AzureAppService" => "AzureMonitorAutoscale",
            "AKS" => "KubernetesHorizontalPodAutoscaler",
            "AzureContainerApps" => "ContainerAppsAutoscale",
            _ => "GenericAutoscale"
        };

        var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["provider"] = provider,
            ["minInstances"] = profile.MinInstances.ToString(),
            ["maxInstances"] = profile.MaxInstances.ToString(),
            ["scaleOutThresholdCpu"] = profile.Cpu.ScaleOut.ToString("F2"),
            ["scaleInThresholdCpu"] = profile.Cpu.ScaleIn.ToString("F2"),
            ["scaleOutThresholdRequests"] = profile.Requests.ScaleOut.ToString("F0"),
            ["scaleInThresholdRequests"] = profile.Requests.ScaleIn.ToString("F0"),
            ["forecastRequiredInstances"] = forecast.RequiredInstances.ToString()
        };

        var yaml = BuildYaml(profile, provider);
        var json = BuildJson(profile, provider);

        return new ScalingPolicyDocument
        {
            Provider = provider,
            Settings = settings,
            SampleYaml = yaml,
            SampleJson = json
        };
    }

    private static string BuildYaml(ScalingProfile profile, string provider)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"apiVersion: autoscale/v1");
        builder.AppendLine($"kind: {provider}");
        builder.AppendLine("metadata:");
        builder.AppendLine($"  name: {profile.Name}-autoscale");
        builder.AppendLine("spec:");
        builder.AppendLine($"  minReplicas: {profile.MinInstances}");
        builder.AppendLine($"  maxReplicas: {profile.MaxInstances}");
        builder.AppendLine("  metrics:");
        builder.AppendLine("    - type: Resource");
        builder.AppendLine("      resource:");
        builder.AppendLine("        name: cpu");
        builder.AppendLine($"        targetAverageUtilization: {profile.Cpu.ScaleOut}");
        builder.AppendLine("    - type: Pods");
        builder.AppendLine("      pods:");
        builder.AppendLine("        metricName: requests_per_second");
        builder.AppendLine($"        targetAverageValue: {profile.Requests.ScaleOut}");
        return builder.ToString();
    }

    private static string BuildJson(ScalingProfile profile, string provider)
    {
        return $$"""
{
  "provider": "{{provider}}",
  "name": "{{profile.Name}}-autoscale",
  "capacity": {
    "minimum": {{profile.MinInstances}},
    "maximum": {{profile.MaxInstances}},
    "default": {{Math.Min(profile.MaxInstances, Math.Max(profile.MinInstances, 3))}}
  },
  "rules": [
    {
      "metric": "cpuPercentage",
      "operator": "GreaterThan",
      "threshold": {{profile.Cpu.ScaleOut}},
      "direction": "Increase",
      "changeCount": {{profile.ScaleOutStep}}
    },
    {
      "metric": "requestsPerSecond",
      "operator": "GreaterThan",
      "threshold": {{profile.Requests.ScaleOut}},
      "direction": "Increase",
      "changeCount": {{profile.ScaleOutStep}}
    },
    {
      "metric": "cpuPercentage",
      "operator": "LessThan",
      "threshold": {{profile.Cpu.ScaleIn}},
      "direction": "Decrease",
      "changeCount": {{profile.ScaleInStep}}
    }
  ]
}
""";
    }
}
