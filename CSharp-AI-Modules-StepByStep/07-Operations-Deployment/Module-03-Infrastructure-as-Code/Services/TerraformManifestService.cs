using System.Globalization;
using System.Text;
using Module_03_Infrastructure_as_Code.Models;

namespace Module_03_Infrastructure_as_Code.Services;

public sealed class TerraformManifestService
{
    public string Build(InfrastructurePlan plan)
    {
        var builder = new StringBuilder();
        builder.AppendLine("terraform {");
        builder.AppendLine("  required_version = \"~= 1.5.0\"");
        builder.AppendLine("}");
        builder.AppendLine();

        foreach (var variable in plan.Variables)
        {
            builder.AppendLine($"variable \"{variable.Key}\" {{");
            builder.AppendLine($"  default = {FormatValue(variable.Value)}");
            builder.AppendLine("}");
            builder.AppendLine();
        }

        foreach (var resource in plan.Resources)
        {
            builder.AppendLine($"resource \"{resource.Type}\" \"{Sanitize(resource.Name)}\" {{");
            foreach (var property in resource.Properties)
            {
                builder.AppendLine($"  {property.Key} = {FormatValue(property.Value)}");
            }

            if (resource.SecurityControls.Count > 0)
            {
                builder.AppendLine("  lifecycle {{");
                builder.AppendLine("    prevent_destroy = true");
                builder.AppendLine("  }}");
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

    private static string FormatValue(string value)
    {
        if (bool.TryParse(value, out var boolValue))
        {
            return boolValue ? "true" : "false";
        }

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var numeric))
        {
            return numeric.ToString(CultureInfo.InvariantCulture);
        }

        return $"\"{value}\"";
    }
}
