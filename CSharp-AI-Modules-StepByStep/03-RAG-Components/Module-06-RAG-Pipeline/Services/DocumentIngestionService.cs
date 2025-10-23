using AI.RAGPipeline.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using NPOI.XWPF.UserModel;
using System.Diagnostics;

namespace AI.RAGPipeline.Services;

public interface IDocumentIngestionService
{
    Task<DocumentIngestionResult> IngestDocumentAsync(DocumentIngestionRequest request);
    Task<List<DocumentChunk>> ExtractAndChunkDocumentAsync(DocumentIngestionRequest request);
    Task<List<float[]>> GenerateEmbeddingsAsync(List<DocumentChunk> chunks);
    Task StoreVectorsAsync(List<DocumentChunk> chunks);
}

public class DocumentIngestionService : IDocumentIngestionService
{
    private readonly IEmbeddingGenerationService _embeddingService;
    private readonly IVectorStorageService _vectorService;
    private readonly ILogger<DocumentIngestionService> _logger;

    public DocumentIngestionService(
        IEmbeddingGenerationService embeddingService,
        IVectorStorageService vectorService,
        ILogger<DocumentIngestionService> logger)
    {
        _embeddingService = embeddingService;
        _vectorService = vectorService;
        _logger = logger;
    }

    public async Task<DocumentIngestionResult> IngestDocumentAsync(DocumentIngestionRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new DocumentIngestionResult
        {
            DocumentId = request.Id
        };

        try
        {
            _logger.LogInformation("Starting document ingestion: {FileName}", request.FileName);

            // Step 1: Extract and chunk document
            var chunks = await ExtractAndChunkDocumentAsync(request);
            result.ChunksCreated = chunks.Count;
            result.ChunkIds = chunks.Select(c => c.Id).ToList();

            if (request.Config.GenerateEmbeddings)
            {
                // Step 2: Generate embeddings
                var embeddings = await GenerateEmbeddingsAsync(chunks);
                result.EmbeddingsGenerated = embeddings.Count;

                // Assign embeddings to chunks
                for (int i = 0; i < chunks.Count && i < embeddings.Count; i++)
                {
                    chunks[i].Embedding = embeddings[i];
                }
            }

            if (request.Config.StoreInVectorDb)
            {
                // Step 3: Store in vector database
                await StoreVectorsAsync(chunks);
                result.VectorsStored = chunks.Count;
            }

            stopwatch.Stop();
            result.ProcessingTime = stopwatch.Elapsed;

            _logger.LogInformation(
                "Document ingestion completed: {ChunksCreated} chunks, {Time}ms",
                result.ChunksCreated, result.ProcessingTime.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during document ingestion: {FileName}", request.FileName);
            throw;
        }
    }

    public async Task<List<DocumentChunk>> ExtractAndChunkDocumentAsync(DocumentIngestionRequest request)
    {
        var content = await ExtractTextAsync(request);
        var chunks = ChunkText(content, request.Config, request.Id);

        // Add metadata
        foreach (var chunk in chunks)
        {
            chunk.Metadata["fileName"] = request.FileName;
            chunk.Metadata["contentType"] = request.ContentType;
            chunk.Metadata["documentId"] = request.Id;
            
            if (request.KnowledgeBaseId != null)
            {
                chunk.Metadata["knowledgeBaseId"] = request.KnowledgeBaseId;
            }

            foreach (var kvp in request.Metadata)
            {
                chunk.Metadata[kvp.Key] = kvp.Value;
            }
        }

        return chunks;
    }

    private async Task<string> ExtractTextAsync(DocumentIngestionRequest request)
    {
        if (request.FileContent == null)
        {
            if (File.Exists(request.FilePath))
            {
                request.FileContent = await File.ReadAllBytesAsync(request.FilePath);
            }
            else
            {
                throw new FileNotFoundException($"File not found: {request.FilePath}");
            }
        }

        var contentType = request.ContentType.ToLowerInvariant();

        return contentType switch
        {
            "application/pdf" or ".pdf" => await ExtractFromPdfAsync(request.FileContent),
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" or ".docx" => 
                await ExtractFromDocxAsync(request.FileContent),
            "text/plain" or ".txt" => System.Text.Encoding.UTF8.GetString(request.FileContent),
            "text/html" or ".html" => ExtractFromHtml(request.FileContent),
            _ => System.Text.Encoding.UTF8.GetString(request.FileContent)
        };
    }

    private async Task<string> ExtractFromPdfAsync(byte[] content)
    {
        await Task.CompletedTask;
        using var ms = new MemoryStream(content);
        using var pdfDoc = new PdfDocument(new PdfReader(ms));
        
        var text = new System.Text.StringBuilder();
        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
        {
            var page = pdfDoc.GetPage(i);
            text.AppendLine(PdfTextExtractor.GetTextFromPage(page));
        }

        return text.ToString();
    }

    private async Task<string> ExtractFromDocxAsync(byte[] content)
    {
        await Task.CompletedTask;
        using var ms = new MemoryStream(content);
        var doc = new XWPFDocument(ms);
        
        var text = new System.Text.StringBuilder();
        foreach (var para in doc.Paragraphs)
        {
            text.AppendLine(para.Text);
        }

        return text.ToString();
    }

    private string ExtractFromHtml(byte[] content)
    {
        var html = System.Text.Encoding.UTF8.GetString(content);
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);
        return doc.DocumentNode.InnerText;
    }

    private List<DocumentChunk> ChunkText(string text, IngestionConfig config, string documentId)
    {
        var chunks = new List<DocumentChunk>();

        switch (config.Strategy)
        {
            case ChunkingStrategy.FixedSize:
                chunks = ChunkByFixedSize(text, config.ChunkSize, config.ChunkOverlap, documentId);
                break;

            case ChunkingStrategy.Sentence:
                chunks = ChunkBySentence(text, config.ChunkSize, documentId);
                break;

            case ChunkingStrategy.Paragraph:
                chunks = ChunkByParagraph(text, config.ChunkSize, documentId);
                break;

            case ChunkingStrategy.Recursive:
                chunks = ChunkRecursively(text, config.ChunkSize, config.ChunkOverlap, documentId);
                break;

            default:
                chunks = ChunkByFixedSize(text, config.ChunkSize, config.ChunkOverlap, documentId);
                break;
        }

        return chunks;
    }

    private List<DocumentChunk> ChunkByFixedSize(string text, int chunkSize, int overlap, string documentId)
    {
        var chunks = new List<DocumentChunk>();
        int start = 0;
        int index = 0;

        while (start < text.Length)
        {
            int end = Math.Min(start + chunkSize, text.Length);
            var chunkText = text.Substring(start, end - start);

            chunks.Add(new DocumentChunk
            {
                DocumentId = documentId,
                Content = chunkText,
                ChunkIndex = index++,
                StartPosition = start,
                EndPosition = end
            });

            start += chunkSize - overlap;
        }

        return chunks;
    }

    private List<DocumentChunk> ChunkBySentence(string text, int maxChunkSize, string documentId)
    {
        var chunks = new List<DocumentChunk>();
        var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        
        var currentChunk = new System.Text.StringBuilder();
        int index = 0;
        int position = 0;

        foreach (var sentence in sentences)
        {
            var trimmed = sentence.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            if (currentChunk.Length + trimmed.Length > maxChunkSize && currentChunk.Length > 0)
            {
                var chunkText = currentChunk.ToString();
                chunks.Add(new DocumentChunk
                {
                    DocumentId = documentId,
                    Content = chunkText,
                    ChunkIndex = index++,
                    StartPosition = position,
                    EndPosition = position + chunkText.Length
                });

                position += chunkText.Length;
                currentChunk.Clear();
            }

            currentChunk.Append(trimmed).Append(". ");
        }

        if (currentChunk.Length > 0)
        {
            var chunkText = currentChunk.ToString();
            chunks.Add(new DocumentChunk
            {
                DocumentId = documentId,
                Content = chunkText,
                ChunkIndex = index,
                StartPosition = position,
                EndPosition = position + chunkText.Length
            });
        }

        return chunks;
    }

    private List<DocumentChunk> ChunkByParagraph(string text, int maxChunkSize, string documentId)
    {
        var chunks = new List<DocumentChunk>();
        var paragraphs = text.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        int index = 0;
        int position = 0;

        foreach (var para in paragraphs)
        {
            var trimmed = para.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            if (trimmed.Length > maxChunkSize)
            {
                // Split large paragraphs
                var subChunks = ChunkByFixedSize(trimmed, maxChunkSize, 100, documentId);
                foreach (var subChunk in subChunks)
                {
                    subChunk.ChunkIndex = index++;
                    chunks.Add(subChunk);
                }
            }
            else
            {
                chunks.Add(new DocumentChunk
                {
                    DocumentId = documentId,
                    Content = trimmed,
                    ChunkIndex = index++,
                    StartPosition = position,
                    EndPosition = position + trimmed.Length
                });
            }

            position += trimmed.Length;
        }

        return chunks;
    }

    private List<DocumentChunk> ChunkRecursively(string text, int chunkSize, int overlap, string documentId)
    {
        var separators = new[] { "\n\n", "\n", ". ", " " };
        return ChunkRecursivelyInternal(text, chunkSize, overlap, separators, 0, documentId, 0).chunks;
    }

    private (List<DocumentChunk> chunks, int index) ChunkRecursivelyInternal(
        string text, int chunkSize, int overlap, string[] separators, 
        int separatorIndex, string documentId, int startIndex)
    {
        var chunks = new List<DocumentChunk>();
        int currentIndex = startIndex;

        if (text.Length <= chunkSize)
        {
            chunks.Add(new DocumentChunk
            {
                DocumentId = documentId,
                Content = text,
                ChunkIndex = currentIndex++,
                StartPosition = 0,
                EndPosition = text.Length
            });
            return (chunks, currentIndex);
        }

        if (separatorIndex >= separators.Length)
        {
            return (ChunkByFixedSize(text, chunkSize, overlap, documentId), currentIndex);
        }

        var parts = text.Split(new[] { separators[separatorIndex] }, StringSplitOptions.None);
        var currentChunk = new System.Text.StringBuilder();

        foreach (var part in parts)
        {
            if (currentChunk.Length + part.Length > chunkSize && currentChunk.Length > 0)
            {
                var chunkText = currentChunk.ToString();
                chunks.Add(new DocumentChunk
                {
                    DocumentId = documentId,
                    Content = chunkText,
                    ChunkIndex = currentIndex++,
                    StartPosition = 0,
                    EndPosition = chunkText.Length
                });
                currentChunk.Clear();
            }

            currentChunk.Append(part).Append(separators[separatorIndex]);
        }

        if (currentChunk.Length > 0)
        {
            chunks.Add(new DocumentChunk
            {
                DocumentId = documentId,
                Content = currentChunk.ToString(),
                ChunkIndex = currentIndex++,
                StartPosition = 0,
                EndPosition = currentChunk.Length
            });
        }

        return (chunks, currentIndex);
    }

    public async Task<List<float[]>> GenerateEmbeddingsAsync(List<DocumentChunk> chunks)
    {
        var embeddings = new List<float[]>();
        var batchSize = 10;

        for (int i = 0; i < chunks.Count; i += batchSize)
        {
            var batch = chunks.Skip(i).Take(batchSize).ToList();
            var texts = batch.Select(c => c.Content).ToList();

            var batchEmbeddings = await _embeddingService.GenerateBatchEmbeddingsAsync(texts);
            embeddings.AddRange(batchEmbeddings);

            _logger.LogDebug("Generated embeddings for batch {BatchNum}/{TotalBatches}",
                i / batchSize + 1, (chunks.Count + batchSize - 1) / batchSize);
        }

        return embeddings;
    }

    public async Task StoreVectorsAsync(List<DocumentChunk> chunks)
    {
        await _vectorService.UpsertVectorsAsync(chunks);
        _logger.LogInformation("Stored {Count} vectors in database", chunks.Count);
    }
}
