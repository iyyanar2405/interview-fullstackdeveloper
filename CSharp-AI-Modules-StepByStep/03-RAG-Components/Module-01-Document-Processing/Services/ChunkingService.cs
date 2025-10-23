using AI.DocumentProcessing.Models;
using Tesseract;
using System.Text;
using System.Text.RegularExpressions;

namespace AI.DocumentProcessing.Services;

#region Chunking Service

public interface IChunkingService
{
    Task<ChunkingResult> ChunkTextAsync(ChunkingRequest request);
    Task<ChunkingResult> ChunkByFixedSizeAsync(string text, ChunkingOptions options, DocumentMetadata? metadata = null);
    Task<ChunkingResult> ChunkBySentenceAsync(string text, ChunkingOptions options, DocumentMetadata? metadata = null);
    Task<ChunkingResult> ChunkByParagraphAsync(string text, ChunkingOptions options, DocumentMetadata? metadata = null);
    Task<ChunkingResult> ChunkBySlidingWindowAsync(string text, ChunkingOptions options, DocumentMetadata? metadata = null);
}

public class ChunkingService : IChunkingService
{
    public async Task<ChunkingResult> ChunkTextAsync(ChunkingRequest request)
    {
        return request.Strategy switch
        {
            ChunkingStrategy.FixedSize => await ChunkByFixedSizeAsync(request.Text, request.Options, request.Metadata),
            ChunkingStrategy.Sentence => await ChunkBySentenceAsync(request.Text, request.Options, request.Metadata),
            ChunkingStrategy.Paragraph => await ChunkByParagraphAsync(request.Text, request.Options, request.Metadata),
            ChunkingStrategy.SlidingWindow => await ChunkBySlidingWindowAsync(request.Text, request.Options, request.Metadata),
            ChunkingStrategy.Semantic => await ChunkBySemanticAsync(request.Text, request.Options, request.Metadata),
            ChunkingStrategy.Recursive => await ChunkRecursivelyAsync(request.Text, request.Options, request.Metadata),
            _ => throw new NotSupportedException($"Chunking strategy {request.Strategy} is not supported")
        };
    }

    public async Task<ChunkingResult> ChunkByFixedSizeAsync(string text, ChunkingOptions options, DocumentMetadata? metadata = null)
    {
        var result = new ChunkingResult { StrategyUsed = ChunkingStrategy.FixedSize };
        var chunks = new List<DocumentChunk>();
        var chunkIndex = 0;

        for (int i = 0; i < text.Length; i += options.ChunkSize - options.ChunkOverlap)
        {
            var chunkSize = Math.Min(options.ChunkSize, text.Length - i);
            var content = text.Substring(i, chunkSize);

            chunks.Add(new DocumentChunk
            {
                Content = content,
                ChunkIndex = chunkIndex++,
                StartPosition = i,
                EndPosition = i + chunkSize,
                CharacterCount = chunkSize,
                Metadata = options.IncludeMetadata ? metadata : null
            });

            if (i + chunkSize >= text.Length)
                break;
        }

        result.Chunks = chunks;
        result.TotalChunks = chunks.Count;
        result.TotalCharacters = text.Length;

        return await Task.FromResult(result);
    }

    public async Task<ChunkingResult> ChunkBySentenceAsync(string text, ChunkingOptions options, DocumentMetadata? metadata = null)
    {
        var result = new ChunkingResult { StrategyUsed = ChunkingStrategy.Sentence };
        var chunks = new List<DocumentChunk>();

        // Split by sentence boundaries
        var sentences = Regex.Split(text, @"(?<=[.!?])\s+");
        var currentChunk = new StringBuilder();
        var chunkIndex = 0;
        var startPosition = 0;

        foreach (var sentence in sentences)
        {
            if (currentChunk.Length + sentence.Length > options.MaxChunkSize && currentChunk.Length > 0)
            {
                // Create chunk
                var content = currentChunk.ToString();
                chunks.Add(new DocumentChunk
                {
                    Content = content,
                    ChunkIndex = chunkIndex++,
                    StartPosition = startPosition,
                    EndPosition = startPosition + content.Length,
                    CharacterCount = content.Length,
                    Metadata = options.IncludeMetadata ? metadata : null
                });

                currentChunk.Clear();
                startPosition += content.Length;
            }

            currentChunk.Append(sentence).Append(' ');
        }

        // Add remaining chunk
        if (currentChunk.Length > 0)
        {
            var content = currentChunk.ToString().Trim();
            chunks.Add(new DocumentChunk
            {
                Content = content,
                ChunkIndex = chunkIndex++,
                StartPosition = startPosition,
                EndPosition = startPosition + content.Length,
                CharacterCount = content.Length,
                Metadata = options.IncludeMetadata ? metadata : null
            });
        }

        result.Chunks = chunks;
        result.TotalChunks = chunks.Count;
        result.TotalCharacters = text.Length;

        return await Task.FromResult(result);
    }

    public async Task<ChunkingResult> ChunkByParagraphAsync(string text, ChunkingOptions options, DocumentMetadata? metadata = null)
    {
        var result = new ChunkingResult { StrategyUsed = ChunkingStrategy.Paragraph };
        var chunks = new List<DocumentChunk>();

        // Split by paragraph (double newline or more)
        var paragraphs = Regex.Split(text, @"\n\s*\n");
        var chunkIndex = 0;
        var currentPosition = 0;

        foreach (var paragraph in paragraphs)
        {
            if (string.IsNullOrWhiteSpace(paragraph))
                continue;

            var trimmedParagraph = paragraph.Trim();

            // If paragraph is too large, split it further
            if (trimmedParagraph.Length > options.MaxChunkSize)
            {
                var subChunks = await ChunkBySentenceAsync(trimmedParagraph, options, metadata);
                foreach (var subChunk in subChunks.Chunks)
                {
                    chunks.Add(new DocumentChunk
                    {
                        Content = subChunk.Content,
                        ChunkIndex = chunkIndex++,
                        StartPosition = currentPosition + subChunk.StartPosition,
                        EndPosition = currentPosition + subChunk.EndPosition,
                        CharacterCount = subChunk.CharacterCount,
                        Metadata = options.IncludeMetadata ? metadata : null
                    });
                }
            }
            else if (trimmedParagraph.Length >= options.MinChunkSize)
            {
                chunks.Add(new DocumentChunk
                {
                    Content = trimmedParagraph,
                    ChunkIndex = chunkIndex++,
                    StartPosition = currentPosition,
                    EndPosition = currentPosition + trimmedParagraph.Length,
                    CharacterCount = trimmedParagraph.Length,
                    Metadata = options.IncludeMetadata ? metadata : null
                });
            }

            currentPosition += paragraph.Length + 2; // +2 for paragraph separator
        }

        result.Chunks = chunks;
        result.TotalChunks = chunks.Count;
        result.TotalCharacters = text.Length;

        return await Task.FromResult(result);
    }

    public async Task<ChunkingResult> ChunkBySlidingWindowAsync(string text, ChunkingOptions options, DocumentMetadata? metadata = null)
    {
        var result = new ChunkingResult { StrategyUsed = ChunkingStrategy.SlidingWindow };
        var chunks = new List<DocumentChunk>();
        var chunkIndex = 0;

        var step = options.ChunkSize - options.ChunkOverlap;

        for (int i = 0; i < text.Length; i += step)
        {
            var chunkSize = Math.Min(options.ChunkSize, text.Length - i);
            var content = text.Substring(i, chunkSize);

            chunks.Add(new DocumentChunk
            {
                Content = content,
                ChunkIndex = chunkIndex++,
                StartPosition = i,
                EndPosition = i + chunkSize,
                CharacterCount = chunkSize,
                Metadata = options.IncludeMetadata ? metadata : null,
                ChunkMetadata = new Dictionary<string, object>
                {
                    ["OverlapSize"] = options.ChunkOverlap,
                    ["StepSize"] = step
                }
            });

            if (i + chunkSize >= text.Length)
                break;
        }

        result.Chunks = chunks;
        result.TotalChunks = chunks.Count;
        result.TotalCharacters = text.Length;

        return await Task.FromResult(result);
    }

    private async Task<ChunkingResult> ChunkBySemanticAsync(string text, ChunkingOptions options, DocumentMetadata? metadata = null)
    {
        // Semantic chunking would require embeddings and similarity calculations
        // For now, fall back to sentence-based chunking
        return await ChunkBySentenceAsync(text, options, metadata);
    }

    private async Task<ChunkingResult> ChunkRecursivelyAsync(string text, ChunkingOptions options, DocumentMetadata? metadata = null)
    {
        var result = new ChunkingResult { StrategyUsed = ChunkingStrategy.Recursive };
        var chunks = new List<DocumentChunk>();

        // Recursive chunking: try paragraph -> sentence -> fixed size
        var separators = new[] { "\n\n", "\n", ". ", " " };
        chunks.AddRange(await RecursiveSplitAsync(text, separators, 0, options, metadata, 0));

        result.Chunks = chunks;
        result.TotalChunks = chunks.Count;
        result.TotalCharacters = text.Length;

        return result;
    }

    private async Task<List<DocumentChunk>> RecursiveSplitAsync(
        string text, 
        string[] separators, 
        int separatorIndex, 
        ChunkingOptions options, 
        DocumentMetadata? metadata,
        int startPosition)
    {
        var chunks = new List<DocumentChunk>();

        if (text.Length <= options.MaxChunkSize)
        {
            if (text.Length >= options.MinChunkSize)
            {
                chunks.Add(new DocumentChunk
                {
                    Content = text,
                    ChunkIndex = chunks.Count,
                    StartPosition = startPosition,
                    EndPosition = startPosition + text.Length,
                    CharacterCount = text.Length,
                    Metadata = options.IncludeMetadata ? metadata : null
                });
            }
            return chunks;
        }

        if (separatorIndex >= separators.Length)
        {
            // No more separators, use fixed-size chunking
            var fixedChunks = await ChunkByFixedSizeAsync(text, options, metadata);
            chunks.AddRange(fixedChunks.Chunks);
            return chunks;
        }

        var parts = text.Split(new[] { separators[separatorIndex] }, StringSplitOptions.None);
        var currentChunk = new StringBuilder();
        var currentStartPosition = startPosition;

        foreach (var part in parts)
        {
            if (currentChunk.Length + part.Length + separators[separatorIndex].Length > options.MaxChunkSize)
            {
                if (currentChunk.Length > 0)
                {
                    var subChunks = await RecursiveSplitAsync(
                        currentChunk.ToString(), 
                        separators, 
                        separatorIndex + 1, 
                        options, 
                        metadata,
                        currentStartPosition);
                    chunks.AddRange(subChunks);
                    currentStartPosition += currentChunk.Length + separators[separatorIndex].Length;
                    currentChunk.Clear();
                }
            }

            currentChunk.Append(part);
            if (part != parts[^1])
                currentChunk.Append(separators[separatorIndex]);
        }

        if (currentChunk.Length > 0)
        {
            var subChunks = await RecursiveSplitAsync(
                currentChunk.ToString(), 
                separators, 
                separatorIndex + 1, 
                options, 
                metadata,
                currentStartPosition);
            chunks.AddRange(subChunks);
        }

        return chunks;
    }
}

#endregion

#region OCR Service

public interface IOCRService
{
    Task<OCRResult> ExtractTextAsync(OCRRequest request);
    Task<OCRResult> ExtractTextFromPathAsync(string imagePath, OCROptions? options = null);
}

public class OCRService : IOCRService
{
    private readonly string _tessDataPath;

    public OCRService(string? tessDataPath = null)
    {
        _tessDataPath = tessDataPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
    }

    public async Task<OCRResult> ExtractTextAsync(OCRRequest request)
    {
        var result = new OCRResult();

        try
        {
            byte[] imageData = request.ImageData;
            
            if (!string.IsNullOrEmpty(request.ImagePath))
            {
                imageData = await File.ReadAllBytesAsync(request.ImagePath);
            }

            using var engine = new TesseractEngine(_tessDataPath, request.Options.Language, EngineMode.Default);
            
            // Configure engine options
            engine.DefaultPageSegMode = PageSegMode.Auto;
            
            using var img = Pix.LoadFromMemory(imageData);
            using var page = engine.Process(img);

            result.Text = page.GetText();
            result.Confidence = page.GetMeanConfidence();
            result.Success = true;

            // Extract word-level information
            using var iter = page.GetIterator();
            iter.Begin();

            do
            {
                if (iter.TryGetBoundingBox(PageIteratorLevel.Word, out var bounds))
                {
                    var wordText = iter.GetText(PageIteratorLevel.Word);
                    var wordConfidence = iter.GetConfidence(PageIteratorLevel.Word);

                    result.Words.Add(new OCRWord
                    {
                        Text = wordText,
                        Confidence = wordConfidence,
                        BoundingBox = new BoundingBox
                        {
                            X = bounds.X1,
                            Y = bounds.Y1,
                            Width = bounds.Width,
                            Height = bounds.Height
                        }
                    });
                }
            } while (iter.Next(PageIteratorLevel.Word));

            // Extract line-level information
            var lines = result.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                result.Lines.Add(new OCRLine
                {
                    Text = line.Trim(),
                    Confidence = result.Confidence // Approximation
                });
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = $"OCR extraction failed: {ex.Message}";
        }

        return result;
    }

    public async Task<OCRResult> ExtractTextFromPathAsync(string imagePath, OCROptions? options = null)
    {
        if (!File.Exists(imagePath))
            throw new FileNotFoundException($"Image file not found: {imagePath}");

        var request = new OCRRequest
        {
            ImagePath = imagePath,
            Options = options ?? new OCROptions()
        };

        return await ExtractTextAsync(request);
    }
}

#endregion
