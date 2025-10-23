using AI.RetrievalStrategies.Models;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AI.RetrievalStrategies.Services;

public interface IReRankingService
{
    Task<ReRankResponse> ReRankAsync(ReRankRequest request);
    Task<ReRankResponse> ReciprocalRankFusionAsync(List<List<RetrievedDocument>> rankedLists, int topK);
    Task<MMRResult> MaximalMarginalRelevanceAsync(DiversityRequest request);
    Task<List<RetrievedDocument>> BM25ReRankAsync(string query, List<RetrievedDocument> documents, int topK);
}

public class ReRankingService : IReRankingService
{
    private readonly RetrievalSettings _settings;
    private readonly ILogger<ReRankingService> _logger;

    public ReRankingService(
        IOptions<RetrievalSettings> settings,
        ILogger<ReRankingService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<ReRankResponse> ReRankAsync(ReRankRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var reRanked = request.Algorithm switch
            {
                ReRankingAlgorithm.ReciprocalRankFusion => await ReRankWithRRFAsync(request),
                ReRankingAlgorithm.Diversity => await ReRankWithDiversityAsync(request),
                ReRankingAlgorithm.MaximalMarginalRelevance => await ReRankWithMMRAsync(request),
                ReRankingAlgorithm.BM25 => await ReRankWithBM25Async(request),
                _ => request.Documents.OrderByDescending(d => d.Score).Take(request.TopK).ToList()
            };

            stopwatch.Stop();

            return new ReRankResponse
            {
                ReRankedDocuments = reRanked,
                Algorithm = request.Algorithm,
                Duration = stopwatch.Elapsed,
                Success = true,
                ScoreDistribution = CalculateScoreDistribution(reRanked)
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Re-ranking failed with algorithm: {Algorithm}", request.Algorithm);

            return new ReRankResponse
            {
                Success = false,
                Algorithm = request.Algorithm,
                Duration = stopwatch.Elapsed
            };
        }
    }

    public async Task<ReRankResponse> ReciprocalRankFusionAsync(
        List<List<RetrievedDocument>> rankedLists,
        int topK)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var k = _settings.RRFConstant; // Default: 60
            var fusionScores = new Dictionary<string, float>();

            // Calculate RRF scores
            foreach (var rankedList in rankedLists)
            {
                for (int rank = 0; rank < rankedList.Count; rank++)
                {
                    var doc = rankedList[rank];
                    var rrfScore = 1.0f / (k + rank + 1);

                    if (fusionScores.ContainsKey(doc.Id))
                    {
                        fusionScores[doc.Id] += rrfScore;
                    }
                    else
                    {
                        fusionScores[doc.Id] = rrfScore;
                    }
                }
            }

            // Get all unique documents and update scores
            var allDocs = rankedLists
                .SelectMany(list => list)
                .GroupBy(d => d.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var doc in allDocs)
            {
                if (fusionScores.ContainsKey(doc.Id))
                {
                    doc.Score = fusionScores[doc.Id];
                }
            }

            var reRanked = allDocs
                .OrderByDescending(d => d.Score)
                .Take(topK)
                .ToList();

            stopwatch.Stop();

            _logger.LogInformation(
                "RRF completed: Lists={Lists}, Unique={Unique}, TopK={TopK}",
                rankedLists.Count, allDocs.Count, topK);

            return new ReRankResponse
            {
                ReRankedDocuments = reRanked,
                Algorithm = ReRankingAlgorithm.ReciprocalRankFusion,
                Duration = stopwatch.Elapsed,
                Success = true,
                ScoreDistribution = CalculateScoreDistribution(reRanked)
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "RRF failed");

            return new ReRankResponse
            {
                Success = false,
                Duration = stopwatch.Elapsed
            };
        }
    }

    public async Task<MMRResult> MaximalMarginalRelevanceAsync(DiversityRequest request)
    {
        await Task.CompletedTask; // For async signature

        var lambda = request.LambdaParameter;
        var selected = new List<RetrievedDocument>();
        var remaining = new List<RetrievedDocument>(request.Documents);

        if (!remaining.Any())
            return new MMRResult { DiverseDocuments = new List<RetrievedDocument>() };

        // Select first document (highest relevance)
        var first = remaining.OrderByDescending(d => d.Score).First();
        selected.Add(first);
        remaining.Remove(first);

        // Iteratively select documents maximizing MMR
        while (selected.Count < request.TopK && remaining.Any())
        {
            float maxScore = float.MinValue;
            RetrievedDocument? bestDoc = null;

            foreach (var doc in remaining)
            {
                // Calculate max similarity to already selected documents
                var maxSimilarity = selected
                    .Select(s => CalculateSimilarity(doc.Embedding, s.Embedding))
                    .DefaultIfEmpty(0)
                    .Max();

                // MMR score: λ * relevance - (1-λ) * max_similarity
                var mmrScore = lambda * doc.Score - (1 - lambda) * maxSimilarity;

                if (mmrScore > maxScore)
                {
                    maxScore = mmrScore;
                    bestDoc = doc;
                }
            }

            if (bestDoc != null)
            {
                selected.Add(bestDoc);
                remaining.Remove(bestDoc);
            }
            else
            {
                break;
            }
        }

        var diversityScores = new Dictionary<string, float>();
        for (int i = 0; i < selected.Count; i++)
        {
            diversityScores[selected[i].Id] = selected[i].Score;
        }

        var avgDiversity = selected.Count > 1
            ? CalculateAverageDiversity(selected)
            : 0;

        _logger.LogInformation(
            "MMR completed: Input={Input}, Selected={Selected}, AvgDiversity={Diversity:F4}",
            request.Documents.Count, selected.Count, avgDiversity);

        return new MMRResult
        {
            DiverseDocuments = selected,
            AverageDiversity = avgDiversity,
            DiversityScores = diversityScores
        };
    }

    public async Task<List<RetrievedDocument>> BM25ReRankAsync(
        string query,
        List<RetrievedDocument> documents,
        int topK)
    {
        await Task.CompletedTask;

        var parameters = new BM25Parameters
        {
            K1 = 1.5f,
            B = 0.75f
        };

        // Tokenize query
        var queryTerms = Tokenize(query.ToLower());

        // Calculate document lengths
        var docLengths = documents.Select(d => Tokenize(d.Content.ToLower()).Count).ToList();
        var avgDocLength = docLengths.Any() ? docLengths.Average() : 0;

        // Calculate term frequencies
        var scores = new List<BM25Result>();

        foreach (var doc in documents)
        {
            var docTokens = Tokenize(doc.Content.ToLower());
            var docLength = docTokens.Count;

            float bm25Score = 0;
            var termScores = new Dictionary<string, float>();

            foreach (var term in queryTerms.Distinct())
            {
                var tf = docTokens.Count(t => t == term);
                if (tf == 0) continue;

                // Document frequency
                var df = documents.Count(d => Tokenize(d.Content.ToLower()).Contains(term));
                var idf = (float)Math.Log((documents.Count - df + 0.5) / (df + 0.5) + 1);

                // BM25 formula
                var numerator = tf * (parameters.K1 + 1);
                var denominator = tf + parameters.K1 * (1 - parameters.B + parameters.B * (docLength / avgDocLength));
                var termScore = idf * (numerator / denominator);

                bm25Score += termScore;
                termScores[term] = termScore;
            }

            scores.Add(new BM25Result
            {
                DocumentId = doc.Id,
                Score = bm25Score,
                TermScores = termScores
            });
        }

        // Update document scores and sort
        for (int i = 0; i < documents.Count; i++)
        {
            documents[i].Score = scores[i].Score;
        }

        var reRanked = documents
            .OrderByDescending(d => d.Score)
            .Take(topK)
            .ToList();

        _logger.LogInformation(
            "BM25 re-ranking completed: Query='{Query}', Docs={Docs}, TopK={TopK}",
            query, documents.Count, topK);

        return reRanked;
    }

    #region Private Methods

    private async Task<List<RetrievedDocument>> ReRankWithRRFAsync(ReRankRequest request)
    {
        // Create multiple ranked lists (e.g., by different criteria)
        var rankedLists = new List<List<RetrievedDocument>>
        {
            request.Documents.OrderByDescending(d => d.Score).ToList()
        };

        var result = await ReciprocalRankFusionAsync(rankedLists, request.TopK);
        return result.ReRankedDocuments;
    }

    private async Task<List<RetrievedDocument>> ReRankWithDiversityAsync(ReRankRequest request)
    {
        var diversityRequest = new DiversityRequest
        {
            Documents = request.Documents,
            TopK = request.TopK,
            LambdaParameter = request.Parameters.GetValueOrDefault("lambda") as float? ?? _settings.MMRLambda,
            QueryText = request.Query
        };

        var result = await MaximalMarginalRelevanceAsync(diversityRequest);
        return result.DiverseDocuments;
    }

    private async Task<List<RetrievedDocument>> ReRankWithMMRAsync(ReRankRequest request)
    {
        return await ReRankWithDiversityAsync(request);
    }

    private async Task<List<RetrievedDocument>> ReRankWithBM25Async(ReRankRequest request)
    {
        return await BM25ReRankAsync(request.Query, request.Documents, request.TopK);
    }

    private float CalculateSimilarity(float[]? embedding1, float[]? embedding2)
    {
        if (embedding1 == null || embedding2 == null)
            return 0;

        if (embedding1.Length != embedding2.Length)
            return 0;

        // Cosine similarity
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

    private float CalculateAverageDiversity(List<RetrievedDocument> documents)
    {
        if (documents.Count < 2)
            return 0;

        float totalDiversity = 0;
        int comparisons = 0;

        for (int i = 0; i < documents.Count; i++)
        {
            for (int j = i + 1; j < documents.Count; j++)
            {
                var similarity = CalculateSimilarity(documents[i].Embedding, documents[j].Embedding);
                totalDiversity += 1 - similarity; // Diversity = 1 - Similarity
                comparisons++;
            }
        }

        return comparisons > 0 ? totalDiversity / comparisons : 0;
    }

    private Dictionary<string, float> CalculateScoreDistribution(List<RetrievedDocument> documents)
    {
        if (!documents.Any())
            return new Dictionary<string, float>();

        return new Dictionary<string, float>
        {
            { "min", documents.Min(d => d.Score) },
            { "max", documents.Max(d => d.Score) },
            { "avg", documents.Average(d => d.Score) },
            { "median", CalculateMedian(documents.Select(d => d.Score).ToList()) }
        };
    }

    private float CalculateMedian(List<float> scores)
    {
        var sorted = scores.OrderBy(s => s).ToList();
        int count = sorted.Count;

        if (count == 0)
            return 0;

        if (count % 2 == 0)
            return (sorted[count / 2 - 1] + sorted[count / 2]) / 2;

        return sorted[count / 2];
    }

    private List<string> Tokenize(string text)
    {
        // Simple tokenization - split by whitespace and punctuation
        return text
            .Split(new[] { ' ', '\t', '\n', '\r', '.', ',', ';', ':', '!', '?' },
                   StringSplitOptions.RemoveEmptyEntries)
            .Where(t => t.Length > 2) // Filter short tokens
            .ToList();
    }

    #endregion
}
