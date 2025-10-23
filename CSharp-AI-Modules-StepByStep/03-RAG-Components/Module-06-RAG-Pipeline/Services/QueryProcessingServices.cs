using AI.RAGPipeline.Models;
using Azure.AI.OpenAI;
using System.Text.RegularExpressions;

namespace AI.RAGPipeline.Services;

public interface IQueryProcessingService
{
    Task<ProcessedQuery> ProcessQueryAsync(string query);
    Task<string> RewriteQueryAsync(string query, List<ConversationMessage> history);
    Task<List<string>> ExpandQueryAsync(string query);
    Task<List<string>> ExtractKeywordsAsync(string query);
}

public class QueryProcessingService : IQueryProcessingService
{
    private readonly IEmbeddingGenerationService _embeddingService;
    private readonly OpenAIClient _openAIClient;
    private readonly ILogger<QueryProcessingService> _logger;
    private readonly string _chatModel;

    public QueryProcessingService(
        IEmbeddingGenerationService embeddingService,
        OpenAIClient openAIClient,
        ILogger<QueryProcessingService> logger,
        IConfiguration configuration)
    {
        _embeddingService = embeddingService;
        _openAIClient = openAIClient;
        _logger = logger;
        _chatModel = configuration.GetValue<string>("RAGPipeline:ChatModel") ?? "gpt-4";
    }

    public async Task<ProcessedQuery> ProcessQueryAsync(string query)
    {
        _logger.LogInformation("Processing query: {Query}", query);

        var processedQuery = new ProcessedQuery
        {
            OriginalQuery = query,
            RewrittenQuery = query
        };

        try
        {
            // Generate query embedding
            processedQuery.QueryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);

            // Extract keywords
            processedQuery.Keywords = await ExtractKeywordsAsync(query);

            // Expand query with synonyms and related terms
            processedQuery.ExpandedQueries = await ExpandQueryAsync(query);

            _logger.LogDebug("Query processing completed: {KeywordCount} keywords, {ExpandedCount} expanded queries",
                processedQuery.Keywords.Count, processedQuery.ExpandedQueries.Count);

            return processedQuery;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing query: {Query}", query);
            throw;
        }
    }

    public async Task<string> RewriteQueryAsync(string query, List<ConversationMessage> history)
    {
        if (history == null || history.Count == 0)
        {
            return query;
        }

        try
        {
            var systemPrompt = @"You are a query rewriting assistant. Your task is to rewrite the user's question 
to be standalone and self-contained, incorporating relevant context from the conversation history.
Only output the rewritten query, nothing else.";

            var conversationContext = string.Join("\n", 
                history.TakeLast(3).Select(m => $"{m.Role}: {m.Content}"));

            var userPrompt = $@"Conversation history:
{conversationContext}

Current question: {query}

Rewrite the current question to be standalone:";

            var chatOptions = new ChatCompletionsOptions
            {
                DeploymentName = _chatModel,
                Messages =
                {
                    new ChatRequestSystemMessage(systemPrompt),
                    new ChatRequestUserMessage(userPrompt)
                },
                Temperature = 0.3f,
                MaxTokens = 200
            };

            var response = await _openAIClient.GetChatCompletionsAsync(chatOptions);
            var rewritten = response.Value.Choices[0].Message.Content.Trim();

            _logger.LogDebug("Query rewritten: '{Original}' -> '{Rewritten}'", query, rewritten);

            return rewritten;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error rewriting query, using original: {Query}", query);
            return query;
        }
    }

    public async Task<List<string>> ExpandQueryAsync(string query)
    {
        var expandedQueries = new List<string> { query };

        try
        {
            var systemPrompt = @"Generate 2-3 alternative phrasings of the given query that maintain the same intent 
but use different words. Output only the alternative queries, one per line.";

            var chatOptions = new ChatCompletionsOptions
            {
                DeploymentName = _chatModel,
                Messages =
                {
                    new ChatRequestSystemMessage(systemPrompt),
                    new ChatRequestUserMessage(query)
                },
                Temperature = 0.7f,
                MaxTokens = 150
            };

            var response = await _openAIClient.GetChatCompletionsAsync(chatOptions);
            var alternatives = response.Value.Choices[0].Message.Content
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(q => q.Trim().TrimStart('-', '*', '1', '2', '3', '.', ' '))
                .Where(q => !string.IsNullOrWhiteSpace(q))
                .ToList();

            expandedQueries.AddRange(alternatives);

            _logger.LogDebug("Query expanded to {Count} variations", expandedQueries.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error expanding query: {Query}", query);
        }

        return expandedQueries;
    }

    public async Task<List<string>> ExtractKeywordsAsync(string query)
    {
        await Task.CompletedTask;

        // Simple keyword extraction (can be enhanced with NLP)
        var keywords = new List<string>();

        // Remove common stop words
        var stopWords = new HashSet<string>
        {
            "a", "an", "the", "is", "are", "was", "were", "be", "been", "being",
            "have", "has", "had", "do", "does", "did", "will", "would", "could",
            "should", "may", "might", "can", "of", "for", "to", "in", "on", "at",
            "by", "with", "from", "about", "what", "how", "why", "when", "where"
        };

        var words = Regex.Split(query.ToLowerInvariant(), @"\W+")
            .Where(w => w.Length > 2 && !stopWords.Contains(w))
            .Distinct()
            .ToList();

        keywords.AddRange(words);

        return keywords;
    }
}

public interface IEmbeddingGenerationService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
    Task<List<float[]>> GenerateBatchEmbeddingsAsync(List<string> texts);
}

public class EmbeddingGenerationService : IEmbeddingGenerationService
{
    private readonly OpenAIClient _openAIClient;
    private readonly ILogger<EmbeddingGenerationService> _logger;
    private readonly string _embeddingModel;

    public EmbeddingGenerationService(
        OpenAIClient openAIClient,
        ILogger<EmbeddingGenerationService> logger,
        IConfiguration configuration)
    {
        _openAIClient = openAIClient;
        _logger = logger;
        _embeddingModel = configuration.GetValue<string>("RAGPipeline:EmbeddingModel") ?? "text-embedding-3-small";
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        try
        {
            var options = new EmbeddingsOptions(_embeddingModel, new[] { text });
            var response = await _openAIClient.GetEmbeddingsAsync(options);
            var embedding = response.Value.Data[0].Embedding.ToArray();

            _logger.LogDebug("Generated embedding for text (length: {Length})", text.Length);

            return embedding;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding");
            throw;
        }
    }

    public async Task<List<float[]>> GenerateBatchEmbeddingsAsync(List<string> texts)
    {
        try
        {
            var options = new EmbeddingsOptions(_embeddingModel, texts);
            var response = await _openAIClient.GetEmbeddingsAsync(options);
            var embeddings = response.Value.Data
                .OrderBy(d => d.Index)
                .Select(d => d.Embedding.ToArray())
                .ToList();

            _logger.LogDebug("Generated {Count} embeddings", embeddings.Count);

            return embeddings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating batch embeddings");
            throw;
        }
    }
}

public interface IVectorStorageService
{
    Task UpsertVectorsAsync(List<DocumentChunk> chunks);
    Task<List<DocumentChunk>> SearchVectorsAsync(float[] queryEmbedding, int topK, Dictionary<string, object>? filters = null);
    Task DeleteVectorsAsync(string documentId);
}

public class VectorStorageService : IVectorStorageService
{
    private readonly ILogger<VectorStorageService> _logger;
    private readonly string _collectionName;
    // In production, inject Qdrant client
    private static readonly Dictionary<string, DocumentChunk> _inMemoryStore = new();

    public VectorStorageService(
        ILogger<VectorStorageService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _collectionName = configuration.GetValue<string>("RAGPipeline:CollectionName") ?? "rag_documents";
    }

    public async Task UpsertVectorsAsync(List<DocumentChunk> chunks)
    {
        await Task.CompletedTask;

        foreach (var chunk in chunks)
        {
            _inMemoryStore[chunk.Id] = chunk;
        }

        _logger.LogInformation("Upserted {Count} vectors to collection: {Collection}", 
            chunks.Count, _collectionName);
    }

    public async Task<List<DocumentChunk>> SearchVectorsAsync(
        float[] queryEmbedding, 
        int topK, 
        Dictionary<string, object>? filters = null)
    {
        await Task.CompletedTask;

        var results = _inMemoryStore.Values
            .Where(chunk => chunk.Embedding != null && chunk.Embedding.Length > 0)
            .Select(chunk => new
            {
                Chunk = chunk,
                Score = CosineSimilarity(queryEmbedding, chunk.Embedding)
            })
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .Select(x =>
            {
                var result = new DocumentChunk
                {
                    Id = x.Chunk.Id,
                    DocumentId = x.Chunk.DocumentId,
                    Content = x.Chunk.Content,
                    ChunkIndex = x.Chunk.ChunkIndex,
                    Metadata = new Dictionary<string, object>(x.Chunk.Metadata)
                };
                result.Metadata["score"] = x.Score;
                return result;
            })
            .ToList();

        _logger.LogDebug("Vector search returned {Count} results", results.Count);

        return results;
    }

    public async Task DeleteVectorsAsync(string documentId)
    {
        await Task.CompletedTask;

        var keysToRemove = _inMemoryStore
            .Where(kvp => kvp.Value.DocumentId == documentId)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _inMemoryStore.Remove(key);
        }

        _logger.LogInformation("Deleted {Count} vectors for document: {DocumentId}", 
            keysToRemove.Count, documentId);
    }

    private float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length) return 0;

        float dotProduct = 0;
        float magnitudeA = 0;
        float magnitudeB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            magnitudeA += a[i] * a[i];
            magnitudeB += b[i] * b[i];
        }

        magnitudeA = (float)Math.Sqrt(magnitudeA);
        magnitudeB = (float)Math.Sqrt(magnitudeB);

        if (magnitudeA == 0 || magnitudeB == 0) return 0;

        return dotProduct / (magnitudeA * magnitudeB);
    }
}
