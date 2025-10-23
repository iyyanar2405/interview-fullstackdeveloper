using System.Text;
using Module_06_Specialized_Features.Models;

namespace Module_06_Specialized_Features.Services;

public sealed class CreativeContentService
{
    private static readonly string[] TransitionalPhrases = new[]
    {
        "Additionally",
        "Moreover",
        "From another perspective",
        "In practice",
        "To keep momentum"
    };

    private readonly SpecializedFeatureCatalogService _catalog;

    public CreativeContentService(SpecializedFeatureCatalogService catalog)
    {
        _catalog = catalog;
    }

    public async Task<(SpecializedExecutionResult Result, List<SpecializedArtifact> Artifacts)> CreateAsync(SpecializedTaskRequest request, CancellationToken cancellationToken)
    {
        if (!request.Parameters.TryGetValue("styleId", out var styleId))
        {
            throw new InvalidOperationException("styleId parameter is required for content creation.");
        }

        var style = _catalog.GetContentStyle(styleId) ?? throw new InvalidOperationException($"Content style '{styleId}' not found.");
        var topic = request.Parameters.TryGetValue("topic", out var providedTopic) ? providedTopic : "emerging technology";
        var audience = request.Parameters.TryGetValue("audience", out var providedAudience) ? providedAudience : "technology leaders";
        var length = request.Parameters.TryGetValue("length", out var providedLength) ? providedLength : "medium";

        var startedAt = DateTimeOffset.UtcNow;
        await Task.Delay(Random.Shared.Next(180, 320), cancellationToken);

        var opener = style.SampleOpeners.Count > 0
            ? style.SampleOpeners[Random.Shared.Next(style.SampleOpeners.Count)]
            : "Let us examine {{Topic}} from a fresh angle.";

        var builder = new StringBuilder();
        builder.AppendLine(opener.Replace("{{Topic}}", topic));
        builder.AppendLine();
        builder.AppendLine($"This {length}-form narrative distills what {audience} should know about {topic}.");
        builder.AppendLine();

        if (style.Guidelines.Count == 0)
        {
            builder.AppendLine($"We highlight why {topic} matters now, how teams can respond, and what signals to monitor next.");
            builder.AppendLine();
        }
        else
        {
            for (var i = 0; i < style.Guidelines.Count; i++)
            {
                var connector = i == 0 ? "First," : $"{TransitionalPhrases[i % TransitionalPhrases.Length]},";
                var sentence = style.Guidelines[i]
                    .Replace("{{Topic}}", topic)
                    .Replace("{{Audience}}", audience);

                builder.AppendLine($"{connector} {sentence}");
                builder.AppendLine();
            }
        }

        builder.AppendLine("Key actions to consider:");
        builder.AppendLine($"- Align current initiatives with the emerging themes inside {topic}.");
        builder.AppendLine("- Empower cross-functional teams to experiment with low-risk pilots.");
        builder.AppendLine("- Instrument feedback loops to validate impact early.");
        builder.AppendLine();
        builder.AppendLine($"In closing, keep a pragmatic yet optimistic view of how {topic} will evolve over the next few quarters.");
        builder.AppendLine();

        var content = builder.ToString();
        var completedAt = DateTimeOffset.UtcNow;
        var wordCount = CountWords(content);

        var outputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["style"] = style.Name,
            ["tone"] = style.Tone,
            ["topic"] = topic,
            ["audience"] = audience,
            ["length"] = length,
            ["wordCount"] = wordCount.ToString()
        };

        var recommendations = new List<string>
        {
            "Review tone alignment with brand guidelines.",
            "Adjust call-to-action copy to match the intended campaign.",
            "Run the narrative through editorial review for voice consistency."
        };

        var result = new SpecializedExecutionResult
        {
            Success = true,
            Status = "completed",
            Summary = $"Generated a {length}-form narrative on {topic}.",
            Outputs = outputs,
            Recommendations = recommendations,
            Duration = completedAt - startedAt,
            CompletedAt = completedAt
        };

        var artifacts = new List<SpecializedArtifact>
        {
            new SpecializedArtifact
            {
                RunId = string.Empty,
                FeatureType = SpecializedFeatureKinds.ContentCreation,
                ArtifactType = "narrative",
                Name = $"{style.StyleId}-{SanitizeFileName(topic)}.md",
                Content = content,
                GeneratedAt = completedAt
            }
        };

        return (result, artifacts);
    }

    private static int CountWords(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return 0;
        }

        return content.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static string SanitizeFileName(string value)
    {
        var sb = new StringBuilder(value.Length);

        foreach (var ch in value)
        {
            sb.Append(char.IsLetterOrDigit(ch) ? char.ToLowerInvariant(ch) : '-');
        }

        var sanitized = sb.ToString().Trim('-');
        return string.IsNullOrEmpty(sanitized) ? "content" : sanitized;
    }
}
