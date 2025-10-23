using System.Text;
using Module_01_Containerization.Models;

namespace Module_01_Containerization.Services;

public sealed class DockerfileGeneratorService
{
    private const string BuildStageName = "build";
    private const string RuntimeStageName = "runtime";

    public string GenerateDockerfile(DockerfileTemplate template, ContainerBuildRequest request)
    {
        var builder = new StringBuilder();

        if (template.MultiStage)
        {
            AppendStage(builder, template, request, BuildStageName, isRuntime: false);
            builder.AppendLine();
            AppendStage(builder, template, request, RuntimeStageName, isRuntime: true);
        }
        else
        {
            AppendStage(builder, template, request, RuntimeStageName, isRuntime: true, singleStage: true);
        }

        if (request.EnableHealthCheck)
        {
            builder.AppendLine("HEALTHCHECK --interval=30s --timeout=3s --start-period=20s --retries=3 \\")
                   .AppendLine($"  CMD curl --fail http://localhost:{request.Ports.FirstOrDefault(8080)}/health || exit 1");
        }

        return builder.ToString();
    }

    private static void AppendStage(StringBuilder builder, DockerfileTemplate template, ContainerBuildRequest request, string stageName, bool isRuntime, bool singleStage = false)
    {
        var baseImage = ResolveBaseImage(template, request, isRuntime);
        var fromLine = singleStage ? $"FROM {baseImage}" : $"FROM {baseImage} AS {stageName}";
        builder.AppendLine(fromLine);

        AppendArgs(builder, template);
        builder.AppendLine("WORKDIR /app");

        var steps = isRuntime ? template.RuntimeStageSteps : template.BuildStageSteps;
        if (steps.Count == 0)
        {
            steps = isRuntime
                ? new List<string> { "COPY --from=build /app/publish ./", "ENTRYPOINT [\"dotnet\", \"app.dll\"]" }
                : new List<string> { "COPY . ./", "RUN dotnet restore", "RUN dotnet publish -c Release -o /app/publish" };
        }

        foreach (var step in steps)
        {
            builder.AppendLine(step);
        }

        if (isRuntime)
        {
            foreach (var port in request.Ports.Distinct())
            {
                builder.AppendLine($"EXPOSE {port}");
            }

            if (request.EnableHttps)
            {
                builder.AppendLine("ENV ASPNETCORE_URLS=https://+:8443;http://+:8080");
            }

            foreach (var kvp in request.Environment)
            {
                builder.AppendLine($"ENV {kvp.Key}={kvp.Value}");
            }

            if (template.Labels.Count > 0)
            {
                builder.AppendLine("LABEL ");
                for (var i = 0; i < template.Labels.Count; i++)
                {
                    var label = template.Labels[i];
                    var suffix = i == template.Labels.Count - 1 ? string.Empty : " \\\n";
                    builder.Append($"    {label}{suffix}");
                }
                builder.AppendLine();
            }
        }
    }

    private static string ResolveBaseImage(DockerfileTemplate template, ContainerBuildRequest request, bool isRuntime)
    {
        if (template.MultiStage && !isRuntime)
        {
            return template.BaseImage;
        }

        if (!template.MultiStage || isRuntime)
        {
            if (request.Language.Equals("dotnet", StringComparison.OrdinalIgnoreCase))
            {
                var variant = request.UseSlimImage ? "-alpine" : string.Empty;
                var tag = request.RuntimeVersion.Replace(".", string.Empty);
                return $"mcr.microsoft.com/dotnet/aspnet:{request.RuntimeVersion}{variant}";
            }

            if (request.Language.Equals("python", StringComparison.OrdinalIgnoreCase))
            {
                var variant = request.UseSlimImage ? "-slim" : string.Empty;
                return $"python:{request.RuntimeVersion}{variant}";
            }
        }

        return template.BaseImage;
    }

    private static void AppendArgs(StringBuilder builder, DockerfileTemplate template)
    {
        foreach (var arg in template.Arguments)
        {
            builder.AppendLine($"ARG {arg.Key}={arg.Value}");
        }
    }
}
