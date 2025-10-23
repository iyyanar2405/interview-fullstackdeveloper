using AI.DocumentProcessing.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace AI.DocumentProcessing.Services;

#region Preprocessing Service

public interface IPreprocessingService
{
    Task<PreprocessingResult> PreprocessAsync(PreprocessingRequest request);
    Task<string> RemoveWhitespaceAsync(string text, bool preserveParagraphs = true);
    Task<string> NormalizeWhitespaceAsync(string text);
    Task<string> RemoveSpecialCharactersAsync(string text);
    Task<string> RemoveUrlsAsync(string text);
    Task<string> RemoveEmailsAsync(string text);
    Task<string> ToLowercaseAsync(string text);
}

public class PreprocessingService : IPreprocessingService
{
    public async Task<PreprocessingResult> PreprocessAsync(PreprocessingRequest request)
    {
        var result = new PreprocessingResult
        {
            OriginalText = request.Text,
            ProcessedText = request.Text
        };

        var originalLength = request.Text.Length;
        var originalWordCount = CountWords(request.Text);

        foreach (var action in request.Actions)
        {
            result.ProcessedText = action switch
            {
                PreprocessingAction.RemoveWhitespace => await RemoveWhitespaceAsync(result.ProcessedText, request.Options.PreserveParagraphs),
                PreprocessingAction.NormalizeWhitespace => await NormalizeWhitespaceAsync(result.ProcessedText),
                PreprocessingAction.RemoveSpecialCharacters => await RemoveSpecialCharactersAsync(result.ProcessedText),
                PreprocessingAction.Lowercase => await ToLowercaseAsync(result.ProcessedText),
                PreprocessingAction.RemoveUrls => await RemoveUrlsAsync(result.ProcessedText),
                PreprocessingAction.RemoveEmails => await RemoveEmailsAsync(result.ProcessedText),
                PreprocessingAction.RemoveNumbers => await RemoveNumbersAsync(result.ProcessedText),
                PreprocessingAction.RemoveStopWords => await RemoveStopWordsAsync(result.ProcessedText, request.Options.Locale ?? "en"),
                _ => result.ProcessedText
            };

            result.AppliedActions.Add(action.ToString());
        }

        // Calculate statistics
        result.Statistics.OriginalLength = originalLength;
        result.Statistics.ProcessedLength = result.ProcessedText.Length;
        result.Statistics.CharactersRemoved = originalLength - result.ProcessedText.Length;
        result.Statistics.WordsRemoved = originalWordCount - CountWords(result.ProcessedText);
        result.Statistics.ReductionPercentage = originalLength > 0 
            ? (double)result.Statistics.CharactersRemoved / originalLength * 100 
            : 0;

        return result;
    }

    public async Task<string> RemoveWhitespaceAsync(string text, bool preserveParagraphs = true)
    {
        if (preserveParagraphs)
        {
            // Remove extra whitespace but preserve paragraph breaks
            text = Regex.Replace(text, @"[ \t]+", " ");
            text = Regex.Replace(text, @"\n{3,}", "\n\n");
        }
        else
        {
            text = Regex.Replace(text, @"\s+", " ");
        }

        return await Task.FromResult(text.Trim());
    }

    public async Task<string> NormalizeWhitespaceAsync(string text)
    {
        // Replace all whitespace with single space
        text = Regex.Replace(text, @"\s+", " ");
        return await Task.FromResult(text.Trim());
    }

    public async Task<string> RemoveSpecialCharactersAsync(string text)
    {
        // Keep only alphanumeric, spaces, and basic punctuation
        text = Regex.Replace(text, @"[^a-zA-Z0-9\s.,!?;:'\-]", "");
        return await Task.FromResult(text);
    }

    public async Task<string> RemoveUrlsAsync(string text)
    {
        var urlPattern = @"https?://[^\s]+|www\.[^\s]+";
        text = Regex.Replace(text, urlPattern, "");
        return await Task.FromResult(text);
    }

    public async Task<string> RemoveEmailsAsync(string text)
    {
        var emailPattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b";
        text = Regex.Replace(text, emailPattern, "");
        return await Task.FromResult(text);
    }

    public async Task<string> ToLowercaseAsync(string text)
    {
        return await Task.FromResult(text.ToLowerInvariant());
    }

    private async Task<string> RemoveNumbersAsync(string text)
    {
        text = Regex.Replace(text, @"\b\d+\b", "");
        return await Task.FromResult(text);
    }

    private async Task<string> RemoveStopWordsAsync(string text, string locale)
    {
        var stopWords = GetStopWords(locale);
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var filteredWords = words.Where(w => !stopWords.Contains(w.ToLowerInvariant()));
        
        return await Task.FromResult(string.Join(" ", filteredWords));
    }

    private HashSet<string> GetStopWords(string locale)
    {
        // Common English stop words
        return new HashSet<string>
        {
            "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
            "has", "he", "in", "is", "it", "its", "of", "on", "that", "the",
            "to", "was", "will", "with", "the", "this", "but", "they", "have",
            "had", "what", "when", "where", "who", "which", "why", "how"
        };
    }

    private int CountWords(string text)
    {
        return text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}

#endregion

#region Metadata Service

public interface IMetadataService
{
    Task<DocumentMetadata> ExtractMetadataAsync(Document document);
    Task<DocumentMetadata> EnrichMetadataAsync(DocumentMetadata metadata, string text);
    Task<string> DetectLanguageAsync(string text);
}

public class MetadataService : IMetadataService
{
    public async Task<DocumentMetadata> ExtractMetadataAsync(Document document)
    {
        var metadata = document.Metadata;

        // Add file-level metadata
        metadata.CustomMetadata["FileSize"] = document.Size;
        metadata.CustomMetadata["FileName"] = document.FileName;
        metadata.CustomMetadata["FileType"] = document.Type.ToString();
        metadata.CustomMetadata["ProcessedDate"] = DateTime.UtcNow;

        return await Task.FromResult(metadata);
    }

    public async Task<DocumentMetadata> EnrichMetadataAsync(DocumentMetadata metadata, string text)
    {
        // Calculate text statistics
        var wordCount = CountWords(text);
        var charCount = text.Length;
        var sentenceCount = CountSentences(text);
        var paragraphCount = CountParagraphs(text);

        metadata.WordCount = wordCount;
        metadata.CustomMetadata["CharacterCount"] = charCount;
        metadata.CustomMetadata["SentenceCount"] = sentenceCount;
        metadata.CustomMetadata["ParagraphCount"] = paragraphCount;
        metadata.CustomMetadata["AverageWordLength"] = wordCount > 0 ? charCount / (double)wordCount : 0;
        metadata.CustomMetadata["AverageSentenceLength"] = sentenceCount > 0 ? wordCount / (double)sentenceCount : 0;

        // Detect language
        metadata.Language = await DetectLanguageAsync(text);

        // Extract keywords (simple frequency-based)
        var keywords = ExtractKeywords(text, topN: 10);
        metadata.Keywords = string.Join(", ", keywords);

        return metadata;
    }

    public async Task<string> DetectLanguageAsync(string text)
    {
        // Simple language detection based on character patterns
        // In production, use a proper language detection library

        if (string.IsNullOrWhiteSpace(text))
            return "unknown";

        // Check for common English words
        var englishIndicators = new[] { "the", "is", "and", "or", "in", "on", "at" };
        var lowerText = text.ToLowerInvariant();
        var englishCount = englishIndicators.Count(word => lowerText.Contains($" {word} "));

        if (englishCount >= 2)
            return "en";

        return await Task.FromResult("unknown");
    }

    private int CountWords(string text)
    {
        return text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private int CountSentences(string text)
    {
        return Regex.Matches(text, @"[.!?]+").Count;
    }

    private int CountParagraphs(string text)
    {
        return Regex.Split(text, @"\n\s*\n").Count(p => !string.IsNullOrWhiteSpace(p));
    }

    private List<string> ExtractKeywords(string text, int topN = 10)
    {
        // Simple keyword extraction based on word frequency
        var words = text.ToLowerInvariant()
            .Split(new[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 3) // Filter short words
            .Where(w => !IsStopWord(w));

        var wordFrequency = words
            .GroupBy(w => w)
            .OrderByDescending(g => g.Count())
            .Take(topN)
            .Select(g => g.Key)
            .ToList();

        return wordFrequency;
    }

    private bool IsStopWord(string word)
    {
        var stopWords = new HashSet<string>
        {
            "the", "and", "for", "that", "this", "with", "from", "have", "will", "would",
            "there", "their", "what", "which", "when", "where", "who", "how", "than"
        };

        return stopWords.Contains(word);
    }
}

#endregion

#region Batch Processing Service

public interface IBatchProcessingService
{
    Task<BatchProcessingResult> ProcessBatchAsync(BatchProcessingRequest request);
}

public class BatchProcessingService : IBatchProcessingService
{
    private readonly IDocumentExtractionService _extractionService;
    private readonly IChunkingService _chunkingService;
    private readonly IPreprocessingService _preprocessingService;
    private readonly IMetadataService _metadataService;

    public BatchProcessingService(
        IDocumentExtractionService extractionService,
        IChunkingService chunkingService,
        IPreprocessingService preprocessingService,
        IMetadataService metadataService)
    {
        _extractionService = extractionService;
        _chunkingService = chunkingService;
        _preprocessingService = preprocessingService;
        _metadataService = metadataService;
    }

    public async Task<BatchProcessingResult> ProcessBatchAsync(BatchProcessingRequest request)
    {
        var result = new BatchProcessingResult();
        var startTime = DateTime.UtcNow;

        var tasks = request.Documents.Select(async document =>
        {
            var processedDoc = new ProcessedDocument
            {
                DocumentId = document.Id,
                FileName = document.FileName
            };

            var docStartTime = DateTime.UtcNow;

            try
            {
                // Extract text
                var extractionRequest = new ExtractionRequest
                {
                    Content = document.Content,
                    Type = document.Type,
                    Options = new ExtractionOptions
                    {
                        ExtractMetadata = true,
                        UseOCR = request.EnableOCR
                    }
                };

                processedDoc.ExtractionResult = await _extractionService.ExtractAsync(extractionRequest);

                if (!processedDoc.ExtractionResult.Success)
                {
                    processedDoc.Success = false;
                    return processedDoc;
                }

                // Preprocess if requested
                if (request.PreprocessingActions.Any())
                {
                    var preprocessingRequest = new PreprocessingRequest
                    {
                        Text = processedDoc.ExtractionResult.Text,
                        Actions = request.PreprocessingActions
                    };

                    processedDoc.PreprocessingResult = await _preprocessingService.PreprocessAsync(preprocessingRequest);
                }

                // Chunk text
                var textToChunk = processedDoc.PreprocessingResult?.ProcessedText 
                    ?? processedDoc.ExtractionResult.Text;

                var chunkingRequest = new ChunkingRequest
                {
                    Text = textToChunk,
                    Strategy = request.ChunkingStrategy,
                    Options = request.ChunkingOptions,
                    Metadata = processedDoc.ExtractionResult.Metadata
                };

                processedDoc.ChunkingResult = await _chunkingService.ChunkTextAsync(chunkingRequest);

                // Enrich metadata
                processedDoc.ExtractionResult.Metadata = await _metadataService.EnrichMetadataAsync(
                    processedDoc.ExtractionResult.Metadata,
                    processedDoc.ExtractionResult.Text);

                processedDoc.Success = true;
            }
            catch (Exception ex)
            {
                processedDoc.Success = false;
                result.Errors.Add($"Document {document.FileName}: {ex.Message}");
            }
            finally
            {
                processedDoc.ProcessingDuration = DateTime.UtcNow - docStartTime;
            }

            return processedDoc;
        });

        result.ProcessedDocuments = (await Task.WhenAll(tasks)).ToList();
        result.SuccessCount = result.ProcessedDocuments.Count(d => d.Success);
        result.FailureCount = result.ProcessedDocuments.Count(d => !d.Success);
        result.TotalDuration = DateTime.UtcNow - startTime;

        return result;
    }
}

#endregion
