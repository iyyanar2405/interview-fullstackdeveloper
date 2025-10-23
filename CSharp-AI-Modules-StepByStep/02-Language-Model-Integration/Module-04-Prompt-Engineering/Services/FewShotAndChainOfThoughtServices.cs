using AI.PromptEngineering.Models;
using Microsoft.Extensions.Logging;
using FuzzySharp;
using System.Collections.Concurrent;

namespace AI.PromptEngineering.Services;

/// <summary>
/// Few-shot learning service interface
/// </summary>
public interface IFewShotService
{
    /// <summary>
    /// Generate few-shot prompt
    /// </summary>
    Task<FewShotPromptResponse> GenerateFewShotPromptAsync(FewShotPromptRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Select best examples for input
    /// </summary>
    Task<List<FewShotExample>> SelectExamplesAsync(string input, List<FewShotExample> examples, FewShotConfig config, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add example to repository
    /// </summary>
    Task<FewShotExample> AddExampleAsync(FewShotExample example, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get examples by tags
    /// </summary>
    Task<List<FewShotExample>> GetExamplesByTagsAsync(List<string> tags, CancellationToken cancellationToken = default);
}

/// <summary>
/// Few-shot learning service implementation
/// </summary>
public class FewShotService : IFewShotService
{
    private readonly ILogger<FewShotService> _logger;
    private readonly ConcurrentDictionary<string, FewShotExample> _exampleRepository;

    public FewShotService(ILogger<FewShotService> logger)
    {
        _logger = logger;
        _exampleRepository = new ConcurrentDictionary<string, FewShotExample>();
        InitializeDefaultExamples();
    }

    public async Task<FewShotPromptResponse> GenerateFewShotPromptAsync(FewShotPromptRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Generating few-shot prompt for task: {Task}", request.TaskDescription);

            var config = request.Config ?? new FewShotConfig
            {
                TaskDescription = request.TaskDescription,
                Examples = request.Examples ?? new List<FewShotExample>(),
                MaxExamples = 5
            };

            // Select best examples
            var selectedExamples = await SelectExamplesAsync(request.UserInput, config.Examples, config, cancellationToken);

            // Build few-shot prompt
            var promptBuilder = new System.Text.StringBuilder();

            // Add task description
            promptBuilder.AppendLine(config.TaskDescription);
            promptBuilder.AppendLine();

            // Add examples
            if (selectedExamples.Any())
            {
                promptBuilder.AppendLine("Here are some examples:");
                promptBuilder.AppendLine();

                for (int i = 0; i < selectedExamples.Count; i++)
                {
                    var example = selectedExamples[i];
                    
                    if (!string.IsNullOrEmpty(config.ExampleFormat))
                    {
                        var formattedExample = FormatExample(example, config.ExampleFormat, i + 1);
                        promptBuilder.AppendLine(formattedExample);
                    }
                    else
                    {
                        promptBuilder.AppendLine($"Example {i + 1}:");
                        promptBuilder.AppendLine($"Input: {example.Input}");
                        promptBuilder.AppendLine($"Output: {example.Output}");
                        
                        if (!string.IsNullOrEmpty(example.Explanation))
                        {
                            promptBuilder.AppendLine($"Explanation: {example.Explanation}");
                        }
                        
                        promptBuilder.AppendLine();
                    }

                    // Update usage count
                    example.UsageCount++;
                }
            }

            // Add user input
            promptBuilder.AppendLine("Now, please process the following:");
            promptBuilder.AppendLine($"Input: {request.UserInput}");
            promptBuilder.AppendLine("Output:");

            var prompt = promptBuilder.ToString();
            var tokenCount = EstimateTokenCount(prompt);

            var response = new FewShotPromptResponse
            {
                Prompt = prompt,
                SelectedExamples = selectedExamples,
                ExampleCount = selectedExamples.Count,
                TokenCount = tokenCount,
                GeneratedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Few-shot prompt generated with {ExampleCount} examples, {TokenCount} tokens",
                selectedExamples.Count, tokenCount);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating few-shot prompt");
            throw;
        }
    }

    public async Task<List<FewShotExample>> SelectExamplesAsync(string input, List<FewShotExample> examples, FewShotConfig config, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            if (!examples.Any())
            {
                return new List<FewShotExample>();
            }

            var selectedExamples = config.SelectionStrategy switch
            {
                ExampleSelectionStrategy.MostRelevant => SelectMostRelevant(input, examples, config.MaxExamples),
                ExampleSelectionStrategy.MostRecent => SelectMostRecent(examples, config.MaxExamples),
                ExampleSelectionStrategy.HighestQuality => SelectHighestQuality(examples, config.MaxExamples),
                ExampleSelectionStrategy.Random => SelectRandom(examples, config.MaxExamples),
                ExampleSelectionStrategy.Diverse => SelectDiverse(input, examples, config.MaxExamples),
                _ => examples.Take(config.MaxExamples).ToList()
            };

            if (config.ShuffleExamples)
            {
                selectedExamples = ShuffleExamples(selectedExamples);
            }

            return selectedExamples;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting examples");
            throw;
        }
    }

    public async Task<FewShotExample> AddExampleAsync(FewShotExample example, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            example.Id = Guid.NewGuid().ToString();
            example.CreatedAt = DateTime.UtcNow;

            _exampleRepository[example.Id] = example;

            _logger.LogInformation("Example {ExampleId} added to repository", example.Id);
            return example;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding example");
            throw;
        }
    }

    public async Task<List<FewShotExample>> GetExamplesByTagsAsync(List<string> tags, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            var examples = _exampleRepository.Values
                .Where(e => e.Tags.Any(t => tags.Contains(t, StringComparer.OrdinalIgnoreCase)))
                .ToList();

            return examples;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving examples by tags");
            throw;
        }
    }

    private static List<FewShotExample> SelectMostRelevant(string input, List<FewShotExample> examples, int maxExamples)
    {
        // Calculate similarity scores using fuzzy matching
        var scoredExamples = examples.Select(example => new
        {
            Example = example,
            Score = Fuzz.PartialRatio(input.ToLower(), example.Input.ToLower())
        })
        .OrderByDescending(x => x.Score)
        .ThenByDescending(x => x.Example.Quality)
        .Take(maxExamples)
        .Select(x => x.Example)
        .ToList();

        return scoredExamples;
    }

    private static List<FewShotExample> SelectMostRecent(List<FewShotExample> examples, int maxExamples)
    {
        return examples
            .OrderByDescending(e => e.CreatedAt)
            .Take(maxExamples)
            .ToList();
    }

    private static List<FewShotExample> SelectHighestQuality(List<FewShotExample> examples, int maxExamples)
    {
        return examples
            .OrderByDescending(e => e.Quality)
            .ThenByDescending(e => e.UsageCount)
            .Take(maxExamples)
            .ToList();
    }

    private static List<FewShotExample> SelectRandom(List<FewShotExample> examples, int maxExamples)
    {
        var random = new Random();
        return examples
            .OrderBy(_ => random.Next())
            .Take(maxExamples)
            .ToList();
    }

    private static List<FewShotExample> SelectDiverse(string input, List<FewShotExample> examples, int maxExamples)
    {
        var selected = new List<FewShotExample>();
        var remaining = examples.ToList();

        // Start with most relevant example
        var mostRelevant = SelectMostRelevant(input, remaining, 1).FirstOrDefault();
        if (mostRelevant != null)
        {
            selected.Add(mostRelevant);
            remaining.Remove(mostRelevant);
        }

        // Add diverse examples
        while (selected.Count < maxExamples && remaining.Any())
        {
            FewShotExample? mostDifferent = null;
            var maxMinSimilarity = -1.0;

            foreach (var candidate in remaining)
            {
                var minSimilarity = selected.Min(s => 
                    Fuzz.Ratio(candidate.Input.ToLower(), s.Input.ToLower()));

                if (minSimilarity > maxMinSimilarity)
                {
                    maxMinSimilarity = minSimilarity;
                    mostDifferent = candidate;
                }
            }

            if (mostDifferent != null)
            {
                selected.Add(mostDifferent);
                remaining.Remove(mostDifferent);
            }
            else
            {
                break;
            }
        }

        return selected;
    }

    private static List<FewShotExample> ShuffleExamples(List<FewShotExample> examples)
    {
        var random = new Random();
        return examples.OrderBy(_ => random.Next()).ToList();
    }

    private static string FormatExample(FewShotExample example, string format, int index)
    {
        return format
            .Replace("{index}", index.ToString())
            .Replace("{input}", example.Input)
            .Replace("{output}", example.Output)
            .Replace("{explanation}", example.Explanation ?? string.Empty);
    }

    private static int EstimateTokenCount(string text)
    {
        return (int)Math.Ceiling(text.Length / 4.0);
    }

    private void InitializeDefaultExamples()
    {
        // Add some default examples for common tasks
        var sentimentExample1 = new FewShotExample
        {
            Id = "sentiment-1",
            Input = "This product is amazing! I love it.",
            Output = "Positive",
            Explanation = "Expresses strong positive emotion with words like 'amazing' and 'love'",
            Tags = new List<string> { "sentiment", "classification" },
            Quality = 1.0
        };

        var sentimentExample2 = new FewShotExample
        {
            Id = "sentiment-2",
            Input = "Terrible experience, would not recommend.",
            Output = "Negative",
            Explanation = "Uses negative words 'terrible' and explicitly states dissatisfaction",
            Tags = new List<string> { "sentiment", "classification" },
            Quality = 1.0
        };

        _exampleRepository[sentimentExample1.Id] = sentimentExample1;
        _exampleRepository[sentimentExample2.Id] = sentimentExample2;

        _logger.LogInformation("Initialized {Count} default examples", _exampleRepository.Count);
    }
}

/// <summary>
/// Chain-of-thought prompting service interface
/// </summary>
public interface IChainOfThoughtService
{
    /// <summary>
    /// Generate chain-of-thought prompt
    /// </summary>
    Task<ChainOfThoughtPromptResponse> GenerateChainOfThoughtPromptAsync(ChainOfThoughtPromptRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parse chain-of-thought response
    /// </summary>
    Task<List<ChainOfThoughtStep>> ParseChainOfThoughtResponseAsync(string response, CancellationToken cancellationToken = default);
}

/// <summary>
/// Chain-of-thought prompting service implementation
/// </summary>
public class ChainOfThoughtService : IChainOfThoughtService
{
    private readonly ILogger<ChainOfThoughtService> _logger;

    public ChainOfThoughtService(ILogger<ChainOfThoughtService> logger)
    {
        _logger = logger;
    }

    public async Task<ChainOfThoughtPromptResponse> GenerateChainOfThoughtPromptAsync(ChainOfThoughtPromptRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            _logger.LogDebug("Generating chain-of-thought prompt for problem: {Problem}", request.Problem);

            var config = request.Config ?? new ChainOfThoughtConfig
            {
                Problem = request.Problem,
                IncludeExplanations = true,
                ShowIntermediateSteps = true,
                MaxSteps = 10,
                Style = ChainOfThoughtStyle.Detailed
            };

            var promptBuilder = new System.Text.StringBuilder();

            // Add instruction based on style
            promptBuilder.AppendLine(GetStyleInstruction(config.Style));
            promptBuilder.AppendLine();

            // Add few-shot examples if provided
            if (request.Examples?.Any() == true)
            {
                promptBuilder.AppendLine("Here are some examples of step-by-step reasoning:");
                promptBuilder.AppendLine();

                foreach (var example in request.Examples.Take(2))
                {
                    promptBuilder.AppendLine($"Problem: {example.Input}");
                    promptBuilder.AppendLine($"Solution: {example.Output}");
                    if (!string.IsNullOrEmpty(example.Explanation))
                    {
                        promptBuilder.AppendLine($"Reasoning: {example.Explanation}");
                    }
                    promptBuilder.AppendLine();
                }
            }

            // Add the problem
            promptBuilder.AppendLine("Now, solve this problem step by step:");
            promptBuilder.AppendLine($"Problem: {config.Problem}");
            promptBuilder.AppendLine();

            // Add guiding questions if provided
            if (config.GuidingQuestions?.Any() == true)
            {
                promptBuilder.AppendLine("Consider these questions:");
                foreach (var question in config.GuidingQuestions)
                {
                    promptBuilder.AppendLine($"- {question}");
                }
                promptBuilder.AppendLine();
            }

            // Add reasoning structure
            promptBuilder.AppendLine(config.Style switch
            {
                ChainOfThoughtStyle.Structured => "Please structure your response as follows:\n" +
                    "Step 1: [Description]\nReasoning: [Explanation]\nResult: [Outcome]\n" +
                    "Continue with subsequent steps...\n",
                
                ChainOfThoughtStyle.Detailed => "Think through this carefully:\n" +
                    "1. First, let's understand what we're trying to solve\n" +
                    "2. Then, break down the problem into smaller parts\n" +
                    "3. Work through each part systematically\n" +
                    "4. Combine the results\n" +
                    "5. Verify the solution\n",
                
                ChainOfThoughtStyle.Concise => "Provide a step-by-step solution with brief explanations for each step.\n",
                
                ChainOfThoughtStyle.Conversational => "Let's think about this together, step by step. " +
                    "I'll explain my reasoning as I go.\n",
                
                _ => "Solve step by step:\n"
            });

            if (config.IncludeExplanations)
            {
                promptBuilder.AppendLine("Make sure to explain your reasoning for each step.");
            }

            promptBuilder.AppendLine("\nYour solution:");

            var prompt = promptBuilder.ToString();
            var tokenCount = EstimateTokenCount(prompt);

            var response = new ChainOfThoughtPromptResponse
            {
                Prompt = prompt,
                Steps = new List<ChainOfThoughtStep>(),
                TokenCount = tokenCount,
                GeneratedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Chain-of-thought prompt generated with {TokenCount} tokens", tokenCount);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating chain-of-thought prompt");
            throw;
        }
    }

    public async Task<List<ChainOfThoughtStep>> ParseChainOfThoughtResponseAsync(string response, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            _logger.LogDebug("Parsing chain-of-thought response");

            var steps = new List<ChainOfThoughtStep>();
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var stepNumber = 1;
            ChainOfThoughtStep? currentStep = null;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Detect step markers
                if (IsStepMarker(trimmedLine))
                {
                    if (currentStep != null)
                    {
                        steps.Add(currentStep);
                    }

                    currentStep = new ChainOfThoughtStep
                    {
                        StepNumber = stepNumber++,
                        Description = ExtractStepDescription(trimmedLine)
                    };
                }
                else if (currentStep != null)
                {
                    // Add to reasoning or result
                    if (trimmedLine.StartsWith("Reasoning:", StringComparison.OrdinalIgnoreCase) ||
                        trimmedLine.StartsWith("Because:", StringComparison.OrdinalIgnoreCase))
                    {
                        currentStep.Reasoning = trimmedLine.Substring(trimmedLine.IndexOf(':') + 1).Trim();
                    }
                    else if (trimmedLine.StartsWith("Result:", StringComparison.OrdinalIgnoreCase) ||
                             trimmedLine.StartsWith("Answer:", StringComparison.OrdinalIgnoreCase))
                    {
                        currentStep.Result = trimmedLine.Substring(trimmedLine.IndexOf(':') + 1).Trim();
                    }
                    else if (!string.IsNullOrWhiteSpace(currentStep.Reasoning))
                    {
                        currentStep.Reasoning += " " + trimmedLine;
                    }
                    else
                    {
                        currentStep.Description += " " + trimmedLine;
                    }
                }
            }

            if (currentStep != null)
            {
                steps.Add(currentStep);
            }

            _logger.LogInformation("Parsed {StepCount} steps from chain-of-thought response", steps.Count);

            return steps;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing chain-of-thought response");
            throw;
        }
    }

    private static string GetStyleInstruction(ChainOfThoughtStyle style)
    {
        return style switch
        {
            ChainOfThoughtStyle.Concise => "Solve the following problem step by step with concise explanations:",
            ChainOfThoughtStyle.Detailed => "Let's solve this problem step by step with detailed reasoning:",
            ChainOfThoughtStyle.Structured => "Please solve this problem following a structured step-by-step approach:",
            ChainOfThoughtStyle.Conversational => "Let's think through this problem together in a conversational way:",
            _ => "Solve the following problem step by step:"
        };
    }

    private static bool IsStepMarker(string line)
    {
        var stepPatterns = new[]
        {
            @"^Step \d+:",
            @"^\d+\.",
            @"^First,",
            @"^Second,",
            @"^Third,",
            @"^Next,",
            @"^Then,",
            @"^Finally,"
        };

        return stepPatterns.Any(pattern => 
            System.Text.RegularExpressions.Regex.IsMatch(line, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
    }

    private static string ExtractStepDescription(string line)
    {
        // Remove step markers
        var cleaned = System.Text.RegularExpressions.Regex.Replace(
            line, 
            @"^(Step \d+:|First,|Second,|Third,|Next,|Then,|Finally,|\d+\.)\s*", 
            string.Empty, 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return cleaned.Trim();
    }

    private static int EstimateTokenCount(string text)
    {
        return (int)Math.Ceiling(text.Length / 4.0);
    }
}