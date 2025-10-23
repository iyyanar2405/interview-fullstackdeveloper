using AI.RetrievalStrategies.Models;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace AI.RetrievalStrategies.Services;

public interface IRetrievalService
{
    Task<RetrievalResponse> SemanticSearchAsync(RetrievalQuery query);
    Task<RetrievalResponse> HybridSearchAsync(HybridSearchRequest request);
    Task<MultiQueryResponse> MultiQueryRetrievalAsync(MultiQueryRequest request);
    Task<RetrievalResponse> HyDERetrievalAsync(HyDERequest request);
    Task<ParentDocumentResponse> ParentDocumentRetrievalAsync(ParentDocumentRequest request);
}

public class RetrievalService : IRetrievalService
{
    private readonly OpenAIClient _openAIClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RetrievalSettings _settings;
    private readonly ILogger<RetrievalService> _logger;

    public RetrievalService(
        OpenAIClient openAIClient,
        IHttpClientFactory httpClientFactory,
        IOptions<RetrievalSettings> settings,
        ILogger<RetrievalService> logger)
    {
        _openAIClient = openAIClient;
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<RetrievalResponse> SemanticSearchAsync(RetrievalQuery query)
    {
        var response = new RetrievalResponse
        {
            Query = query.QueryText,
            Strategy = RetrievalStrategy.Semantic
        };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Generate query embedding if not provided
            float[] queryEmbedding;
            if (query.QueryEmbedding != null && query.QueryEmbedding.Length > 0)
            {
                queryEmbedding = Array.ConvertAll(query.QueryEmbedding, float.Parse);
            }
            else
            {
                queryEmbedding = await GenerateEmbeddingAsync(query.QueryText);
            }

            // Search vector database
            var searchResults = await SearchVectorDatabaseAsync(
                queryEmbedding,
                query.TopK,
                query.ScoreThreshold,
                query.Filters);

            response.Documents = searchResults;
            response.TotalFound = searchResults.Count;
            response.Success = true;

            _logger.LogInformation(
                "Semantic search completed: Query='{Query}', Found={Count}, Duration={Duration}ms",
                query.QueryText, searchResults.Count, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
            _logger.LogError(ex, "Semantic search failed for query: {Query}", query.QueryText);
        }

        stopwatch.Stop();
        response.Duration = stopwatch.Elapsed;
        return response;
    }

    public async Task<RetrievalResponse> HybridSearchAsync(HybridSearchRequest request)
    {
        var response = new RetrievalResponse
        {
            Query = request.QueryText,
            Strategy = RetrievalStrategy.Hybrid
        };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Generate query embedding if not provided
            float[] queryEmbedding;
            if (request.QueryEmbedding != null && request.QueryEmbedding.Length > 0)
            {
                queryEmbedding = request.QueryEmbedding;
            }
            else
            {
                queryEmbedding = await GenerateEmbeddingAsync(request.QueryText);
            }

            // Perform semantic search
            var semanticResults = await SearchVectorDatabaseAsync(
                queryEmbedding,
                request.TopK * 2, // Get more for fusion
                null,
                request.Filters);

            // Perform keyword search (BM25)
            var keywordResults = await KeywordSearchAsync(
                request.QueryText,
                request.CollectionName,
                request.TopK * 2);

            // Merge results with weighted scores
            var mergedResults = MergeHybridResults(
                semanticResults,
                keywordResults,
                request.SemanticWeight,
                request.KeywordWeight,
                request.TopK);

            response.Documents = mergedResults;
            response.TotalFound = mergedResults.Count;
            response.Success = true;
            response.Metadata["semanticCount"] = semanticResults.Count;
            response.Metadata["keywordCount"] = keywordResults.Count;

            _logger.LogInformation(
                "Hybrid search completed: Query='{Query}', Semantic={Semantic}, Keyword={Keyword}, Merged={Merged}",
                request.QueryText, semanticResults.Count, keywordResults.Count, mergedResults.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
            _logger.LogError(ex, "Hybrid search failed for query: {Query}", request.QueryText);
        }

        stopwatch.Stop();
        response.Duration = stopwatch.Elapsed;
        return response;
    }

    public async Task<MultiQueryResponse> MultiQueryRetrievalAsync(MultiQueryRequest request)
    {
        var response = new MultiQueryResponse
        {
            OriginalQuery = request.OriginalQuery,
            TotalQueries = request.ExpandedQueries.Count
        };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var allResults = new List<RetrievalResponse>();

            // Execute all queries in parallel
            var tasks = request.ExpandedQueries.Select(async expandedQuery =>
            {
                var query = new RetrievalQuery
                {
                    QueryText = expandedQuery,
                    TopK = request.TopK,
                    Filters = request.Filters
                };
                return await SemanticSearchAsync(query);
            });

            allResults = (await Task.WhenAll(tasks)).ToList();

            // Merge and deduplicate results
            var mergedDocs = new Dictionary<string, RetrievedDocument>();

            foreach (var result in allResults)
            {
                foreach (var doc in result.Documents)
                {
                    if (!mergedDocs.ContainsKey(doc.Id))
                    {
                        mergedDocs[doc.Id] = doc;
                    }
                    else
                    {
                        // Average scores for duplicate documents
                        mergedDocs[doc.Id].Score = (mergedDocs[doc.Id].Score + doc.Score) / 2;
                    }
                }
            }

            response.QueryResults = allResults;
            response.MergedDocuments = mergedDocs.Values
                .OrderByDescending(d => d.Score)
                .Take(request.TopK)
                .ToList();
            response.Success = true;

            _logger.LogInformation(
                "Multi-query retrieval completed: Original='{Query}', Queries={Count}, Merged={Merged}",
                request.OriginalQuery, request.ExpandedQueries.Count, response.MergedDocuments.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            _logger.LogError(ex, "Multi-query retrieval failed for: {Query}", request.OriginalQuery);
        }

        stopwatch.Stop();
        response.Duration = stopwatch.Elapsed;
        return response;
    }

    public async Task<RetrievalResponse> HyDERetrievalAsync(HyDERequest request)
    {
        var response = new RetrievalResponse
        {
            Query = request.Query,
            Strategy = RetrievalStrategy.HyDE
        };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Generate hypothetical documents
            var hypotheticalDocs = await GenerateHypotheticalDocumentsAsync(
                request.Query,
                request.NumHypotheticalDocs);

            // Generate embeddings for hypothetical docs
            var embeddingTasks = hypotheticalDocs.Select(doc => GenerateEmbeddingAsync(doc));
            var embeddings = await Task.WhenAll(embeddingTasks);

            // Average the embeddings
            var avgEmbedding = AverageEmbeddings(embeddings.ToList());

            // Search with averaged embedding
            var searchResults = await SearchVectorDatabaseAsync(
                avgEmbedding,
                request.TopK,
                null,
                new Dictionary<string, string>());

            response.Documents = searchResults;
            response.TotalFound = searchResults.Count;
            response.Success = true;
            response.Metadata["hypotheticalDocs"] = hypotheticalDocs.Count;

            _logger.LogInformation(
                "HyDE retrieval completed: Query='{Query}', Hypothetical={Hypo}, Found={Found}",
                request.Query, hypotheticalDocs.Count, searchResults.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
            _logger.LogError(ex, "HyDE retrieval failed for query: {Query}", request.Query);
        }

        stopwatch.Stop();
        response.Duration = stopwatch.Elapsed;
        return response;
    }

    public async Task<ParentDocumentResponse> ParentDocumentRetrievalAsync(ParentDocumentRequest request)
    {
        var response = new ParentDocumentResponse();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // First, search child chunks
            var queryEmbedding = await GenerateEmbeddingAsync(request.Query);
            var childChunks = await SearchVectorDatabaseAsync(
                queryEmbedding,
                request.TopK,
                null,
                new Dictionary<string, string>());

            response.ChildChunks = childChunks;

            // If requested, retrieve full parent documents
            if (request.RetrieveFullParent)
            {
                var parentIds = childChunks
                    .Select(c => c.Metadata.GetValueOrDefault("parentDocumentId")?.ToString())
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Distinct()
                    .ToList();

                var parentDocs = new List<RetrievedDocument>();
                foreach (var parentId in parentIds)
                {
                    if (parentId != null)
                    {
                        var parent = await RetrieveParentDocumentAsync(parentId);
                        if (parent != null)
                        {
                            parentDocs.Add(parent);
                        }
                    }
                }

                response.ParentDocuments = parentDocs;
            }

            response.Success = true;

            _logger.LogInformation(
                "Parent document retrieval completed: Chunks={Chunks}, Parents={Parents}",
                response.ChildChunks.Count, response.ParentDocuments.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            _logger.LogError(ex, "Parent document retrieval failed");
        }

        stopwatch.Stop();
        response.Duration = stopwatch.Elapsed;
        return response;
    }

    #region Helper Methods

    private async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var options = new EmbeddingsOptions(_settings.EmbeddingModel, new[] { text });
        var result = await _openAIClient.GetEmbeddingsAsync(options);
        return result.Value.Data[0].Embedding.ToArray();
    }

    private async Task<List<RetrievedDocument>> SearchVectorDatabaseAsync(
        float[] queryEmbedding,
        int topK,
        float? scoreThreshold,
        Dictionary<string, string> filters)
    {
        // Mock implementation - In production, call actual vector database
        var httpClient = _httpClientFactory.CreateClient();
        
        var searchRequest = new
        {
            collectionName = "documents",
            queryVector = queryEmbedding,
            limit = topK,
            scoreThreshold = scoreThreshold,
            filter = filters,
            withPayload = true
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync(
                $"{_settings.VectorDatabaseEndpoint}/api/VectorDatabase/search",
                searchRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<VectorSearchResponse>();
                return result?.Results?.Select(r => new RetrievedDocument
                {
                    Id = r.Id,
                    Content = r.Payload.GetValueOrDefault("content")?.ToString() ?? "",
                    Score = r.Score,
                    Metadata = r.Payload,
                    Source = r.Payload.GetValueOrDefault("source")?.ToString() ?? ""
                }).ToList() ?? new List<RetrievedDocument>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Vector database search failed, returning empty results");
        }

        return new List<RetrievedDocument>();
    }

    private async Task<List<RetrievedDocument>> KeywordSearchAsync(
        string query,
        string collectionName,
        int topK)
    {
        // Simple keyword search implementation
        // In production, use Lucene.NET or Elasticsearch
        var results = new List<RetrievedDocument>();

        // Mock implementation
        await Task.Delay(50); // Simulate API call

        return results;
    }

    private List<RetrievedDocument> MergeHybridResults(
        List<RetrievedDocument> semanticResults,
        List<RetrievedDocument> keywordResults,
        float semanticWeight,
        float keywordWeight,
        int topK)
    {
        var merged = new Dictionary<string, RetrievedDocument>();

        // Add semantic results
        foreach (var doc in semanticResults)
        {
            doc.Score *= semanticWeight;
            merged[doc.Id] = doc;
        }

        // Merge keyword results
        foreach (var doc in keywordResults)
        {
            if (merged.ContainsKey(doc.Id))
            {
                merged[doc.Id].Score += doc.Score * keywordWeight;
            }
            else
            {
                doc.Score *= keywordWeight;
                merged[doc.Id] = doc;
            }
        }

        return merged.Values
            .OrderByDescending(d => d.Score)
            .Take(topK)
            .ToList();
    }

    private async Task<List<string>> GenerateHypotheticalDocumentsAsync(string query, int count)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions
        {
            DeploymentName = _settings.ChatModel,
            Messages =
            {
                new ChatRequestSystemMessage("You are a helpful assistant that generates hypothetical documents that would answer the user's query."),
                new ChatRequestUserMessage($"Generate {count} hypothetical documents that would answer this query: {query}\n\nProvide each document separated by '---'")
            },
            Temperature = 0.7f,
            MaxTokens = 500
        };

        var response = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);
        var content = response.Value.Choices[0].Message.Content;

        var documents = content.Split("---", StringSplitOptions.RemoveEmptyEntries)
            .Select(d => d.Trim())
            .Take(count)
            .ToList();

        return documents;
    }

    private float[] AverageEmbeddings(List<float[]> embeddings)
    {
        if (!embeddings.Any())
            return Array.Empty<float>();

        var dimension = embeddings[0].Length;
        var averaged = new float[dimension];

        foreach (var embedding in embeddings)
        {
            for (int i = 0; i < dimension; i++)
            {
                averaged[i] += embedding[i];
            }
        }

        for (int i = 0; i < dimension; i++)
        {
            averaged[i] /= embeddings.Count;
        }

        return averaged;
    }

    private async Task<RetrievedDocument?> RetrieveParentDocumentAsync(string parentId)
    {
        // Mock implementation - retrieve from database
        await Task.Delay(10);
        return null;
    }

    #endregion
}

#region Helper Classes

internal class VectorSearchResponse
{
    public List<VectorSearchResult> Results { get; set; } = new();
}

internal class VectorSearchResult
{
    public string Id { get; set; } = string.Empty;
    public float Score { get; set; }
    public Dictionary<string, object> Payload { get; set; } = new();
}

#endregion
