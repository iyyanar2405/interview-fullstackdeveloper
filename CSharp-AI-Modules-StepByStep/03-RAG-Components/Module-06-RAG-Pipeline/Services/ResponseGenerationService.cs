using AI.RAGPipeline.Models;
using Azure.AI.OpenAI;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace AI.RAGPipeline.Services;

public interface IResponseGenerationService
{
    Task<RAGResponse> GenerateResponseAsync(string query, RetrievedContext context, GenerationConfig config);
    IAsyncEnumerable<string> GenerateStreamingResponseAsync(string query, RetrievedContext context, GenerationConfig config);
    Task<string> SynthesizeAnswerAsync(string query, List<ContextChunk> contexts, GenerationConfig config);
}

public class ResponseGenerationService : IResponseGenerationService
{
    private readonly OpenAIClient _openAIClient;
    private readonly ILogger<ResponseGenerationService> _logger;

    public ResponseGenerationService(
        OpenAIClient openAIClient,
        ILogger<ResponseGenerationService> logger)
    {
        _openAIClient = openAIClient;
        _logger = logger;
    }

    public async Task<RAGResponse> GenerateResponseAsync(
        string query, 
        RetrievedContext context, 
        GenerationConfig config)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Generating response for query: {Query}", query);

            var answer = await SynthesizeAnswerAsync(query, context.Chunks, config);

            stopwatch.Stop();

            var response = new RAGResponse
            {
                Query = query,
                Answer = answer,
                Sources = context.Chunks,
                TokensUsed = EstimateTokens(query + answer),
                TotalTime = stopwatch.Elapsed,
                RetrievalTime = context.RetrievalTime,
                GenerationTime = stopwatch.Elapsed,
                Metadata = new Dictionary<string, object>
                {
                    { "model", config.Model },
                    { "temperature", config.Temperature },
                    { "sourcesUsed", context.Chunks.Count }
                }
            };

            _logger.LogInformation(
                "Response generated in {Time}ms, tokens: {Tokens}",
                response.GenerationTime.TotalMilliseconds, response.TokensUsed);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating response");
            throw;
        }
    }

    public async IAsyncEnumerable<string> GenerateStreamingResponseAsync(
        string query, 
        RetrievedContext context, 
        GenerationConfig config,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var contextText = BuildContextText(context.Chunks);
        var prompt = BuildPrompt(query, contextText, config.SystemPrompt);

        var chatOptions = new ChatCompletionsOptions
        {
            DeploymentName = config.Model,
            Messages =
            {
                new ChatRequestSystemMessage(config.SystemPrompt),
                new ChatRequestUserMessage(prompt)
            },
            Temperature = config.Temperature,
            MaxTokens = config.MaxTokens,
            TopP = config.TopP,
            FrequencyPenalty = config.FrequencyPenalty,
            PresencePenalty = config.PresencePenalty
        };

        var streamingResponse = await _openAIClient.GetChatCompletionsStreamingAsync(chatOptions, cancellationToken);

        await foreach (var update in streamingResponse.WithCancellation(cancellationToken))
        {
            if (update.ContentUpdate != null)
            {
                yield return update.ContentUpdate;
            }
        }
    }

    public async Task<string> SynthesizeAnswerAsync(
        string query, 
        List<ContextChunk> contexts, 
        GenerationConfig config)
    {
        if (contexts == null || contexts.Count == 0)
        {
            return "I don't have enough information to answer this question based on the available context.";
        }

        var contextText = BuildContextText(contexts);
        var prompt = BuildPrompt(query, contextText, config.SystemPrompt);

        var chatOptions = new ChatCompletionsOptions
        {
            DeploymentName = config.Model,
            Messages =
            {
                new ChatRequestSystemMessage(config.SystemPrompt),
                new ChatRequestUserMessage(prompt)
            },
            Temperature = config.Temperature,
            MaxTokens = config.MaxTokens,
            TopP = config.TopP,
            FrequencyPenalty = config.FrequencyPenalty,
            PresencePenalty = config.PresencePenalty
        };

        var response = await _openAIClient.GetChatCompletionsAsync(chatOptions);
        var answer = response.Value.Choices[0].Message.Content;

        _logger.LogDebug("Synthesized answer (length: {Length}) from {ContextCount} contexts",
            answer.Length, contexts.Count);

        return answer;
    }

    private string BuildContextText(List<ContextChunk> contexts)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < contexts.Count; i++)
        {
            sb.AppendLine($"[Source {i + 1}]");
            sb.AppendLine(contexts[i].Content);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private string BuildPrompt(string query, string contextText, string systemPrompt)
    {
        return $@"Use the following context to answer the question. If the answer cannot be found in the context, say so.

Context:
{contextText}

Question: {query}

Answer:";
    }

    private int EstimateTokens(string text)
    {
        // Rough estimate: ~4 characters per token
        return text.Length / 4;
    }
}

public interface IEvaluationService
{
    Task<RAGEvaluation> EvaluateResponseAsync(EvaluationRequest request);
    Task<float> CalculateFaithfulnessAsync(string answer, List<ContextChunk> context);
    Task<float> CalculateRelevanceAsync(string query, string answer);
    Task<float> CalculateAnswerQualityAsync(string answer);
}

public class EvaluationService : IEvaluationService
{
    private readonly OpenAIClient _openAIClient;
    private readonly ILogger<EvaluationService> _logger;
    private readonly string _chatModel;

    public EvaluationService(
        OpenAIClient openAIClient,
        ILogger<EvaluationService> logger,
        IConfiguration configuration)
    {
        _openAIClient = openAIClient;
        _logger = logger;
        _chatModel = configuration.GetValue<string>("RAGPipeline:ChatModel") ?? "gpt-4";
    }

    public async Task<RAGEvaluation> EvaluateResponseAsync(EvaluationRequest request)
    {
        _logger.LogInformation("Evaluating RAG response");

        var evaluation = new RAGEvaluation
        {
            QueryId = Guid.NewGuid().ToString()
        };

        try
        {
            // Calculate metrics in parallel
            var faithfulnessTask = CalculateFaithfulnessAsync(request.Answer, request.Context);
            var relevanceTask = CalculateRelevanceAsync(request.Query, request.Answer);
            var qualityTask = CalculateAnswerQualityAsync(request.Answer);

            await Task.WhenAll(faithfulnessTask, relevanceTask, qualityTask);

            evaluation.FaithfulnessScore = await faithfulnessTask;
            evaluation.RelevanceScore = await relevanceTask;
            evaluation.AnswerQualityScore = await qualityTask;

            // Calculate context metrics
            evaluation.ContextPrecisionScore = CalculateContextPrecision(request.Context);
            evaluation.ContextRecallScore = CalculateContextRecall(request.Answer, request.Context);

            // Calculate overall score
            evaluation.OverallScore = (
                evaluation.FaithfulnessScore * 0.3f +
                evaluation.RelevanceScore * 0.3f +
                evaluation.AnswerQualityScore * 0.2f +
                evaluation.ContextPrecisionScore * 0.1f +
                evaluation.ContextRecallScore * 0.1f
            );

            // Determine quality
            evaluation.Quality = evaluation.OverallScore switch
            {
                >= 0.8f => ResponseQuality.Excellent,
                >= 0.6f => ResponseQuality.Good,
                >= 0.4f => ResponseQuality.Fair,
                _ => ResponseQuality.Poor
            };

            // Add suggestions
            if (evaluation.FaithfulnessScore < 0.7f)
            {
                evaluation.Issues.Add("Low faithfulness - answer may not be grounded in context");
                evaluation.Suggestions.Add("Review retrieved context for relevance");
            }

            if (evaluation.RelevanceScore < 0.7f)
            {
                evaluation.Issues.Add("Low relevance - answer may not address the query");
                evaluation.Suggestions.Add("Refine query processing and retrieval strategy");
            }

            if (evaluation.ContextPrecisionScore < 0.5f)
            {
                evaluation.Suggestions.Add("Improve retrieval to get more relevant context");
            }

            _logger.LogInformation(
                "Evaluation completed: Overall={Overall}, Quality={Quality}",
                evaluation.OverallScore, evaluation.Quality);

            return evaluation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating response");
            throw;
        }
    }

    public async Task<float> CalculateFaithfulnessAsync(string answer, List<ContextChunk> context)
    {
        if (string.IsNullOrWhiteSpace(answer) || context == null || context.Count == 0)
        {
            return 0f;
        }

        try
        {
            var contextText = string.Join("\n\n", context.Select(c => c.Content));

            var prompt = $@"Evaluate if the following answer is faithful to (grounded in) the provided context.
Score from 0 to 1, where 1 means completely faithful and 0 means not faithful at all.
Only respond with a number between 0 and 1.

Context:
{contextText}

Answer:
{answer}

Faithfulness Score:";

            var chatOptions = new ChatCompletionsOptions
            {
                DeploymentName = _chatModel,
                Messages = { new ChatRequestUserMessage(prompt) },
                Temperature = 0.0f,
                MaxTokens = 10
            };

            var response = await _openAIClient.GetChatCompletionsAsync(chatOptions);
            var scoreText = response.Value.Choices[0].Message.Content.Trim();

            if (float.TryParse(scoreText, out var score))
            {
                return Math.Clamp(score, 0f, 1f);
            }

            _logger.LogWarning("Failed to parse faithfulness score: {ScoreText}", scoreText);
            return 0.5f; // Default if parsing fails
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error calculating faithfulness, returning default");
            return 0.5f;
        }
    }

    public async Task<float> CalculateRelevanceAsync(string query, string answer)
    {
        if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(answer))
        {
            return 0f;
        }

        try
        {
            var prompt = $@"Evaluate how relevant the following answer is to the question.
Score from 0 to 1, where 1 means highly relevant and 0 means not relevant at all.
Only respond with a number between 0 and 1.

Question:
{query}

Answer:
{answer}

Relevance Score:";

            var chatOptions = new ChatCompletionsOptions
            {
                DeploymentName = _chatModel,
                Messages = { new ChatRequestUserMessage(prompt) },
                Temperature = 0.0f,
                MaxTokens = 10
            };

            var response = await _openAIClient.GetChatCompletionsAsync(chatOptions);
            var scoreText = response.Value.Choices[0].Message.Content.Trim();

            if (float.TryParse(scoreText, out var score))
            {
                return Math.Clamp(score, 0f, 1f);
            }

            return 0.5f;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error calculating relevance, returning default");
            return 0.5f;
        }
    }

    public async Task<float> CalculateAnswerQualityAsync(string answer)
    {
        await Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(answer))
        {
            return 0f;
        }

        var quality = 0f;

        // Length check (not too short, not too long)
        if (answer.Length >= 50 && answer.Length <= 2000)
        {
            quality += 0.3f;
        }
        else if (answer.Length > 20)
        {
            quality += 0.1f;
        }

        // Structure check (has sentences)
        var sentenceCount = answer.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
        if (sentenceCount >= 2)
        {
            quality += 0.3f;
        }

        // No "I don't know" variations
        var dontKnowPhrases = new[] { "i don't know", "i cannot", "i can't", "not enough information" };
        if (!dontKnowPhrases.Any(phrase => answer.ToLowerInvariant().Contains(phrase)))
        {
            quality += 0.4f;
        }

        return Math.Clamp(quality, 0f, 1f);
    }

    private float CalculateContextPrecision(List<ContextChunk> context)
    {
        if (context == null || context.Count == 0)
        {
            return 0f;
        }

        // Higher scores for top-ranked chunks indicate good precision
        var avgTopScores = context.Take(3).Average(c => c.Score);
        return Math.Clamp(avgTopScores, 0f, 1f);
    }

    private float CalculateContextRecall(string answer, List<ContextChunk> context)
    {
        if (string.IsNullOrWhiteSpace(answer) || context == null || context.Count == 0)
        {
            return 0f;
        }

        // Check how many context chunks were "used" in the answer
        var answerWords = answer.ToLowerInvariant().Split(' ').ToHashSet();
        var usedChunks = 0;

        foreach (var chunk in context)
        {
            var chunkWords = chunk.Content.ToLowerInvariant().Split(' ').ToHashSet();
            var overlap = answerWords.Intersect(chunkWords).Count();
            
            if (overlap > 5) // At least 5 words overlap
            {
                usedChunks++;
            }
        }

        return (float)usedChunks / context.Count;
    }
}
