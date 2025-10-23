using System.Globalization;
using System.Linq;
using System.Text;
using Module_03_Infrastructure_as_Code.Models;

namespace Module_03_Infrastructure_as_Code.Services;

public sealed class BicepTemplateService
{
    public string Build(InfrastructurePlan plan)
    {
        var builder = new StringBuilder();

        foreach (var variable in plan.Variables)
        {
            builder.Append("param ");
            builder.Append(variable.Key);
            builder.Append(' ');
            builder.Append(InferType(variable.Value));
            builder.Append(" = ");
            builder.AppendLine(FormatValue(variable.Value));
        }

        if (plan.Variables.Count > 0)
        {
            builder.AppendLine();
        }

        foreach (var resource in plan.Resources)
        {
            var symbol = Sanitize(resource.Name);
            var type = MapType(resource.Type);
            var version = resource.Properties.TryGetValue("api_version", out var api) ? api : "2022-09-01";
            builder.AppendLine($"resource {symbol} '{type}@{version}' = {{");
            builder.AppendLine($"  name: '{resource.Name}'");
            builder.AppendLine($"  location: '{plan.Region}'");
            builder.AppendLine("  properties: {");

            foreach (var property in resource.Properties.Where(p => !string.Equals(p.Key, "api_version", StringComparison.OrdinalIgnoreCase)))
            {
                builder.AppendLine($"    {property.Key}: {FormatValue(property.Value)}");
            }

            builder.AppendLine("  }");

            if (resource.SecurityControls.Count > 0)
            {
                builder.AppendLine("  metadata: {");
                builder.AppendLine("    annotations: {");
                foreach (var control in resource.SecurityControls)
                {
                    builder.AppendLine($"      {Sanitize(control.Key)}: '{control.Value}'");
                }

                builder.AppendLine("    }");
                builder.AppendLine("  }");
            }

            builder.AppendLine("}");
            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static string Sanitize(string value)
    {
        return value.Replace("-", "_").Replace(" ", "_");
    }

    private static string MapType(string resourceType)
    {
        if (resourceType.Contains("azurerm", StringComparison.OrdinalIgnoreCase))
        {
            return resourceType
                .Replace("azurerm_", "Microsoft.")
                .Replace("_", ".", StringComparison.OrdinalIgnoreCase);
        }

        return resourceType;
    }

    private static string InferType(string value)
    {
        if (bool.TryParse(value, out _))
        {
            return "bool";
        }

        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
        {
            return "int";
        }

        return "string";
    }

    private static string FormatValue(string value)
    {
        if (bool.TryParse(value, out var boolValue))
        {
            return boolValue ? "true" : "false";
        }

        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var numeric))
        {
            return numeric.ToString(CultureInfo.InvariantCulture);
        }

        return $"'{value}'";
    }
}
