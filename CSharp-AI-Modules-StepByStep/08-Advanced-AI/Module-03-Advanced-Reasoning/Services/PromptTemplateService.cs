using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Options;
using Module_03_Advanced_Reasoning.Models;

namespace Module_03_Advanced_Reasoning.Services;

public sealed class PromptTemplateService
{
    private readonly ConcurrentDictionary<string, PromptTemplate> _prompts = new(StringComparer.OrdinalIgnoreCase);

    public PromptTemplateService(IOptions<AdvancedReasoningOptions> options)
    {
        foreach (var prompt in options.Value.Prompts)
        {
            _prompts[prompt.PromptId] = Clone(prompt);
        }
    }

    public IReadOnlyCollection<PromptTemplate> GetPrompts()
    {
        return _prompts.Values.Select(Clone).ToArray();
    }

    public PromptTemplate? GetPrompt(string promptId)
    {
        return _prompts.TryGetValue(promptId, out var prompt) ? Clone(prompt) : null;
    }

    private static PromptTemplate Clone(PromptTemplate prompt)
    {
        return new PromptTemplate
        {
            PromptId = prompt.PromptId,
            Title = prompt.Title,
            Template = prompt.Template,
            Tags = prompt.Tags.ToList()
        };
    }
}
