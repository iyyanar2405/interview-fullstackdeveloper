using Module_01_Containerization.Models;

namespace Module_01_Containerization.Services;

public sealed class ImageOptimizationService
{
    public ImageOptimizationReport Analyze(DockerfileTemplate template, ContainerBuildRequest request, string dockerfile)
    {
        var report = new ImageOptimizationReport
        {
            BaseImage = template.BaseImage,
            Layers = ExtractLayers(dockerfile)
        };

        report.EstimatedImageSizeMb = report.Layers.Sum(l => l.EstimatedSizeMb);
        report.Recommendations.AddRange(template.OptimizationTips);

        if (request.PublishTrimmed)
        {
            report.Recommendations.Add("Project enables PublishTrimmed; ensure trimming annotations cover reflection-based code.");
        }

        if (!request.UseSlimImage)
        {
            report.Recommendations.Add("Consider using slim or alpine runtime images to reduce footprint.");
        }

        if (request.AdditionalDependencies.Count > 0)
        {
            report.Recommendations.Add("Move dependency installation into dedicated layers to leverage caching.");
        }

        return report;
    }

    private static List<LayerBreakdown> ExtractLayers(string dockerfile)
    {
        var layers = new List<LayerBreakdown>();
        using var reader = new StringReader(dockerfile);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            line = line.Trim();
            if (line.StartsWith("RUN", StringComparison.OrdinalIgnoreCase))
            {
                layers.Add(new LayerBreakdown
                {
                    Instruction = line,
                    Category = "runtime",
                    EstimatedSizeMb = EstimateSize(line)
                });
            }
            else if (line.StartsWith("COPY", StringComparison.OrdinalIgnoreCase) || line.StartsWith("ADD", StringComparison.OrdinalIgnoreCase))
            {
                layers.Add(new LayerBreakdown
                {
                    Instruction = line,
                    Category = "asset",
                    EstimatedSizeMb = 50 // heuristic
                });
            }
        }

        if (layers.Count == 0)
        {
            layers.Add(new LayerBreakdown
            {
                Instruction = "FROM base",
                Category = "base",
                EstimatedSizeMb = 120
            });
        }

        return layers;
    }

    private static double EstimateSize(string line)
    {
        if (line.Contains("apt-get", StringComparison.OrdinalIgnoreCase) || line.Contains("apk", StringComparison.OrdinalIgnoreCase))
        {
            return 80;
        }

        if (line.Contains("dotnet publish", StringComparison.OrdinalIgnoreCase))
        {
            return 65;
        }

        if (line.Contains("npm install", StringComparison.OrdinalIgnoreCase))
        {
            return 120;
        }

        return 20;
    }
}
