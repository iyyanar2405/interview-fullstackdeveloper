using Module_02_Quality_Assessment.Models;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Module_02_Quality_Assessment.Services;

/// <summary>
/// Performs lexical bias detection using configurable lexicons and heuristic weighting.
/// </summary>
public sealed class BiasDetectionService
{
    private readonly QualityAssessmentOptions _options;

    public BiasDetectionService(IOptions<QualityAssessmentOptions> options)
    {
        _options = options.Value;
    }

    public BiasReport Analyze(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return new BiasReport();
        }

        var matches = new List<BiasMatch>();
        var normalizedResponse = response.ToLowerInvariant();

        foreach (var (category, terms) in _options.BiasLexicon)
        {
            foreach (var term in terms)
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    continue;
                }

                var pattern = $"\\b{Regex.Escape(term.ToLowerInvariant())}\\b";
                var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);
                var regexMatches = regex.Matches(normalizedResponse);
                foreach (Match match in regexMatches)
                {
                    var snippet = ExtractSnippet(response, match.Value);
                    matches.Add(new BiasMatch
                    {
                        Term = match.Value,
                        Category = category,
                        Snippet = snippet
                    });
                }
            }
        }

        if (matches.Count == 0)
        {
            return new BiasReport();
        }

        var probability = Math.Min(1.0, 0.15 * matches.Count);
        return new BiasReport
        {
            ContainsBias = true,
            BiasProbability = Math.Round(probability, 2),
            Matches = matches
        };
    }

    private static string ExtractSnippet(string response, string term)
    {
        var idx = response.IndexOf(term, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
        {
            return string.Empty;
        }

        var start = Math.Max(0, idx - 40);
        var length = Math.Min(80, response.Length - start);
        var snippet = response.Substring(start, length).Trim();
        return snippet.Replace('\n', ' ');
    }
}
