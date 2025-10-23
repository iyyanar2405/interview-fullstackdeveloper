using AI.RetrievalStrategies.Models;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text;

namespace AI.RetrievalStrategies.Services;

public interface IQueryExpansionService
{
    Task<QueryExpansionResponse> ExpandQueryAsync(QueryExpansionRequest request);
    Task<List<string>> GenerateSynonymsAsync(string query, int maxExpansions);
    Task<List<string>> GenerateParaphrasesAsync(string query, int maxExpansions);
    Task<List<string>> GenerateMultiPerspectiveQueriesAsync(string query, int maxExpansions);
}

public class QueryExpansionService : IQueryExpansionService
{
    private readonly OpenAIClient _openAIClient;
    private readonly RetrievalSettings _settings;
    private readonly ILogger<QueryExpansionService> _logger;

    public QueryExpansionService(
        OpenAIClient openAIClient,
        IOptions<RetrievalSettings> settings,
        ILogger<QueryExpansionService> logger)
    {
        _openAIClient = openAIClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<QueryExpansionResponse> ExpandQueryAsync(QueryExpansionRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var expandedQueries = request.Method switch
            {
                QueryExpansionMethod.Synonyms => await GenerateSynonymsAsync(request.OriginalQuery, request.MaxExpansions),
                QueryExpansionMethod.Paraphrasing => await GenerateParaphrasesAsync(request.OriginalQuery, request.MaxExpansions),
                QueryExpansionMethod.QuestionGeneration => await GenerateQuestionsAsync(request.OriginalQuery, request.MaxExpansions),
                QueryExpansionMethod.MultiPerspective => await GenerateMultiPerspectiveQueriesAsync(request.OriginalQuery, request.MaxExpansions),
                QueryExpansionMethod.ContextualExpansion => await GenerateContextualExpansionsAsync(request.OriginalQuery, request.MaxExpansions),
                _ => new List<string> { request.OriginalQuery }
            };

            stopwatch.Stop();

            _logger.LogInformation(
                "Query expansion completed: Method={Method}, Original='{Query}', Expanded={Count}",
                request.Method, request.OriginalQuery, expandedQueries.Count);

            return new QueryExpansionResponse
            {
                OriginalQuery = request.OriginalQuery,
                ExpandedQueries = expandedQueries,
                Method = request.Method,
                Success = true,
                Duration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Query expansion failed for: {Query}", request.OriginalQuery);

            return new QueryExpansionResponse
            {
                OriginalQuery = request.OriginalQuery,
                Success = false,
                Duration = stopwatch.Elapsed
            };
        }
    }

    public async Task<List<string>> GenerateSynonymsAsync(string query, int maxExpansions)
    {
        var prompt = $@"Generate {maxExpansions} alternative queries using synonyms for: '{query}'

Provide only the alternative queries, one per line, without numbering or explanation.";

        return await GenerateExpansionsWithLLM(prompt, maxExpansions);
    }

    public async Task<List<string>> GenerateParaphrasesAsync(string query, int maxExpansions)
    {
        var prompt = $@"Generate {maxExpansions} paraphrased versions of this query: '{query}'

Each paraphrase should have the same meaning but use different words and sentence structure.
Provide only the paraphrases, one per line, without numbering or explanation.";

        return await GenerateExpansionsWithLLM(prompt, maxExpansions);
    }

    public async Task<List<string>> GenerateMultiPerspectiveQueriesAsync(string query, int maxExpansions)
    {
        var prompt = $@"Generate {maxExpansions} different perspectives or angles for this query: '{query}'

Think about different ways someone might ask the same question from various viewpoints.
Provide only the alternative queries, one per line, without numbering or explanation.";

        return await GenerateExpansionsWithLLM(prompt, maxExpansions);
    }

    private async Task<List<string>> GenerateQuestionsAsync(string query, int maxExpansions)
    {
        var prompt = $@"Generate {maxExpansions} related questions for the topic: '{query}'

Provide questions that explore different aspects of the topic.
Provide only the questions, one per line, without numbering or explanation.";

        return await GenerateExpansionsWithLLM(prompt, maxExpansions);
    }

    private async Task<List<string>> GenerateContextualExpansionsAsync(string query, int maxExpansions)
    {
        var prompt = $@"Generate {maxExpansions} contextual expansions for: '{query}'

Include related concepts, technical terms, and domain-specific variations.
Provide only the expanded queries, one per line, without numbering or explanation.";

        return await GenerateExpansionsWithLLM(prompt, maxExpansions);
    }

    private async Task<List<string>> GenerateExpansionsWithLLM(string prompt, int maxExpansions)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions
        {
            DeploymentName = _settings.ChatModel,
            Messages =
            {
                new ChatRequestSystemMessage("You are a helpful assistant that generates query variations. Provide only the queries without any numbering, bullets, or explanations."),
                new ChatRequestUserMessage(prompt)
            },
            Temperature = 0.7f,
            MaxTokens = 300
        };

        var response = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);
        var content = response.Value.Choices[0].Message.Content;

        var expansions = content
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim().TrimStart('-', '*', '•', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', ')').Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Take(maxExpansions)
            .ToList();

        return expansions;
    }
}

public interface IContextOptimizationService
{
    Task<ContextOptimizationResponse> OptimizeContextAsync(ContextOptimizationRequest request);
    int EstimateTokenCount(string text);
}

public class ContextOptimizationService : IContextOptimizationService
{
    private readonly RetrievalSettings _settings;
    private readonly ILogger<ContextOptimizationService> _logger;

    public ContextOptimizationService(
        IOptions<RetrievalSettings> settings,
        ILogger<ContextOptimizationService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<ContextOptimizationResponse> OptimizeContextAsync(ContextOptimizationRequest request)
    {
        await Task.CompletedTask;

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var optimizedWindow = request.Strategy switch
            {
                ContextOptimizationStrategy.MaxTokens => OptimizeByMaxTokens(request),
                ContextOptimizationStrategy.Relevance => OptimizeByRelevance(request),
                ContextOptimizationStrategy.Diversity => OptimizeByDiversity(request),
                ContextOptimizationStrategy.Recency => OptimizeByRecency(request),
                ContextOptimizationStrategy.Hybrid => OptimizeHybrid(request),
                _ => OptimizeByRelevance(request)
            };

            var originalTokens = request.Documents.Sum(d => EstimateTokenCount(d.Content));
            var optimizedTokens = optimizedWindow.TotalTokens;

            stopwatch.Stop();

            _logger.LogInformation(
                "Context optimization completed: Strategy={Strategy}, Original={Original}, Optimized={Optimized}, Compression={Ratio:F2}",
                request.Strategy, originalTokens, optimizedTokens, (float)optimizedTokens / originalTokens);

            return new ContextOptimizationResponse
            {
                OptimizedContext = optimizedWindow,
                OriginalTokenCount = originalTokens,
                OptimizedTokenCount = optimizedTokens,
                CompressionRatio = originalTokens > 0 ? (float)optimizedTokens / originalTokens : 1,
                Success = true
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Context optimization failed");

            return new ContextOptimizationResponse
            {
                Success = false
            };
        }
    }

    public int EstimateTokenCount(string text)
    {
        // Rough estimation: 1 token ≈ 4 characters for English
        // More accurate: use tiktoken library
        return (int)Math.Ceiling(text.Length / 4.0);
    }

    private ContextWindow OptimizeByMaxTokens(ContextOptimizationRequest request)
    {
        var window = new ContextWindow
        {
            MaxTokens = request.MaxTokens,
            Strategy = ContextOptimizationStrategy.MaxTokens
        };

        int currentTokens = 0;

        foreach (var doc in request.Documents.OrderByDescending(d => d.Score))
        {
            var docTokens = EstimateTokenCount(doc.Content);

            if (currentTokens + docTokens <= request.MaxTokens)
            {
                window.Documents.Add(doc);
                currentTokens += docTokens;
            }
            else
            {
                window.IsTruncated = true;
                break;
            }
        }

        window.TotalTokens = currentTokens;
        return window;
    }

    private ContextWindow OptimizeByRelevance(ContextOptimizationRequest request)
    {
        // Sort by relevance score and fit within token limit
        var sortedDocs = request.Documents
            .OrderByDescending(d => d.Score)
            .ToList();

        var optimizedRequest = new ContextOptimizationRequest
        {
            Documents = sortedDocs,
            MaxTokens = request.MaxTokens,
            Strategy = ContextOptimizationStrategy.Relevance
        };

        return OptimizeByMaxTokens(optimizedRequest);
    }

    private ContextWindow OptimizeByDiversity(ContextOptimizationRequest request)
    {
        var window = new ContextWindow
        {
            MaxTokens = request.MaxTokens,
            Strategy = ContextOptimizationStrategy.Diversity
        };

        if (!request.Documents.Any())
            return window;

        var selected = new List<RetrievedDocument>();
        var remaining = new List<RetrievedDocument>(request.Documents);
        int currentTokens = 0;

        // Select first document (highest score)
        var first = remaining.OrderByDescending(d => d.Score).First();
        var firstTokens = EstimateTokenCount(first.Content);

        if (firstTokens <= request.MaxTokens)
        {
            selected.Add(first);
            currentTokens += firstTokens;
            remaining.Remove(first);
        }

        // Select diverse documents
        while (remaining.Any() && currentTokens < request.MaxTokens)
        {
            RetrievedDocument? bestDoc = null;
            float maxMinSimilarity = float.MinValue;

            foreach (var doc in remaining)
            {
                var docTokens = EstimateTokenCount(doc.Content);
                if (currentTokens + docTokens > request.MaxTokens)
                    continue;

                // Calculate minimum similarity to selected documents
                var minSimilarity = selected
                    .Select(s => CalculateSimilarity(doc.Embedding, s.Embedding))
                    .DefaultIfEmpty(0)
                    .Min();

                if (minSimilarity > maxMinSimilarity)
                {
                    maxMinSimilarity = minSimilarity;
                    bestDoc = doc;
                }
            }

            if (bestDoc != null)
            {
                var tokens = EstimateTokenCount(bestDoc.Content);
                selected.Add(bestDoc);
                currentTokens += tokens;
                remaining.Remove(bestDoc);
            }
            else
            {
                break;
            }
        }

        window.Documents = selected;
        window.TotalTokens = currentTokens;
        window.IsTruncated = remaining.Any();

        return window;
    }

    private ContextWindow OptimizeByRecency(ContextOptimizationRequest request)
    {
        // Sort by timestamp if available
        var sortedDocs = request.Documents
            .OrderByDescending(d => d.Timestamp ?? DateTime.MinValue)
            .ThenByDescending(d => d.Score)
            .ToList();

        var optimizedRequest = new ContextOptimizationRequest
        {
            Documents = sortedDocs,
            MaxTokens = request.MaxTokens,
            Strategy = ContextOptimizationStrategy.Recency
        };

        return OptimizeByMaxTokens(optimizedRequest);
    }

    private ContextWindow OptimizeHybrid(ContextOptimizationRequest request)
    {
        // Combine relevance (60%), diversity (30%), and recency (10%)
        var scoredDocs = request.Documents.Select(doc => new
        {
            Document = doc,
            RelevanceScore = doc.Score * 0.6f,
            RecencyScore = CalculateRecencyScore(doc.Timestamp) * 0.1f,
            DiversityScore = 0f // Will calculate below
        }).ToList();

        // Calculate diversity scores
        foreach (var item in scoredDocs)
        {
            var diversityScores = scoredDocs
                .Where(other => other.Document.Id != item.Document.Id)
                .Select(other => 1 - CalculateSimilarity(item.Document.Embedding, other.Document.Embedding))
                .DefaultIfEmpty(0)
                .ToList();

            var avgDiversity = diversityScores.Any() ? diversityScores.Average() : 0;
            var newItem = new
            {
                item.Document,
                item.RelevanceScore,
                item.RecencyScore,
                DiversityScore = avgDiversity * 0.3f
            };

            var index = scoredDocs.IndexOf(item);
            scoredDocs[index] = newItem;
        }

        // Sort by combined score
        var sortedDocs = scoredDocs
            .OrderByDescending(item => item.RelevanceScore + item.RecencyScore + item.DiversityScore)
            .Select(item => item.Document)
            .ToList();

        var optimizedRequest = new ContextOptimizationRequest
        {
            Documents = sortedDocs,
            MaxTokens = request.MaxTokens,
            Strategy = ContextOptimizationStrategy.Hybrid
        };

        return OptimizeByMaxTokens(optimizedRequest);
    }

    private float CalculateSimilarity(float[]? embedding1, float[]? embedding2)
    {
        if (embedding1 == null || embedding2 == null)
            return 0;

        if (embedding1.Length != embedding2.Length)
            return 0;

        float dotProduct = 0;
        float magnitude1 = 0;
        float magnitude2 = 0;

        for (int i = 0; i < embedding1.Length; i++)
        {
            dotProduct += embedding1[i] * embedding2[i];
            magnitude1 += embedding1[i] * embedding1[i];
            magnitude2 += embedding2[i] * embedding2[i];
        }

        magnitude1 = (float)Math.Sqrt(magnitude1);
        magnitude2 = (float)Math.Sqrt(magnitude2);

        if (magnitude1 == 0 || magnitude2 == 0)
            return 0;

        return dotProduct / (magnitude1 * magnitude2);
    }

    private float CalculateRecencyScore(DateTime? timestamp)
    {
        if (!timestamp.HasValue)
            return 0;

        var age = DateTime.UtcNow - timestamp.Value;
        var days = age.TotalDays;

        // Exponential decay: newer = higher score
        return (float)Math.Exp(-days / 30.0); // Half-life of 30 days
    }
}

public interface ISelfQueryService
{
    Task<ParsedQuery> ParseNaturalLanguageQueryAsync(string query);
}

public class SelfQueryService : ISelfQueryService
{
    private readonly OpenAIClient _openAIClient;
    private readonly RetrievalSettings _settings;
    private readonly ILogger<SelfQueryService> _logger;

    public SelfQueryService(
        OpenAIClient openAIClient,
        IOptions<RetrievalSettings> settings,
        ILogger<SelfQueryService> logger)
    {
        _openAIClient = openAIClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<ParsedQuery> ParseNaturalLanguageQueryAsync(string query)
    {
        var prompt = $@"Parse this natural language query and extract:
1. The core search query (remove filters and metadata)
2. Any filters (e.g., category, author, date range)
3. Date constraints
4. Keywords

Query: ""{query}""

Respond in JSON format:
{{
  ""queryText"": ""core search query"",
  ""filters"": {{""key"": ""value""}},
  ""startDate"": ""yyyy-MM-dd or null"",
  ""endDate"": ""yyyy-MM-dd or null"",
  ""keywords"": [""keyword1"", ""keyword2""]
}}";

        var chatCompletionsOptions = new ChatCompletionsOptions
        {
            DeploymentName = _settings.ChatModel,
            Messages =
            {
                new ChatRequestSystemMessage("You are a query parser that extracts structured information from natural language queries. Always respond with valid JSON."),
                new ChatRequestUserMessage(prompt)
            },
            Temperature = 0.0f,
            MaxTokens = 200
        };

        var response = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);
        var content = response.Value.Choices[0].Message.Content;

        try
        {
            var parsed = System.Text.Json.JsonSerializer.Deserialize<ParsedQuery>(content);
            return parsed ?? new ParsedQuery { QueryText = query };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse query, using original: {Query}", query);
            return new ParsedQuery { QueryText = query };
        }
    }
}
