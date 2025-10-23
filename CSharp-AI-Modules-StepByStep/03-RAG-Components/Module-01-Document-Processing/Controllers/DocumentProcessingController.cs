using AI.DocumentProcessing.Models;
using AI.DocumentProcessing.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.DocumentProcessing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentProcessingController : ControllerBase
{
    private readonly IDocumentExtractionService _extractionService;
    private readonly IChunkingService _chunkingService;
    private readonly IPreprocessingService _preprocessingService;
    private readonly IMetadataService _metadataService;
    private readonly IBatchProcessingService _batchProcessingService;
    private readonly IOCRService _ocrService;

    public DocumentProcessingController(
        IDocumentExtractionService extractionService,
        IChunkingService chunkingService,
        IPreprocessingService preprocessingService,
        IMetadataService metadataService,
        IBatchProcessingService batchProcessingService,
        IOCRService ocrService)
    {
        _extractionService = extractionService;
        _chunkingService = chunkingService;
        _preprocessingService = preprocessingService;
        _metadataService = metadataService;
        _batchProcessingService = batchProcessingService;
        _ocrService = ocrService;
    }

    #region Document Extraction Endpoints

    /// <summary>
    /// Extract text from a document
    /// </summary>
    [HttpPost("extract")]
    public async Task<ActionResult<ExtractionResult>> ExtractText([FromBody] ExtractionRequest request)
    {
        var result = await _extractionService.ExtractAsync(request);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Extract text from an uploaded file
    /// </summary>
    [HttpPost("extract/upload")]
    public async Task<ActionResult<ExtractionResult>> ExtractFromFile(
        IFormFile file,
        [FromQuery] bool extractImages = false,
        [FromQuery] bool extractTables = false,
        [FromQuery] bool useOCR = false)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var request = new ExtractionRequest
        {
            Content = memoryStream.ToArray(),
            Type = _extractionService.DetectDocumentType(file.FileName),
            Options = new ExtractionOptions
            {
                ExtractImages = extractImages,
                ExtractTables = extractTables,
                UseOCR = useOCR,
                ExtractMetadata = true
            }
        };

        var result = await _extractionService.ExtractAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Extract text from a PDF with page range
    /// </summary>
    [HttpPost("extract/pdf")]
    public async Task<ActionResult<ExtractionResult>> ExtractFromPdf(
        IFormFile file,
        [FromQuery] int? pageStart = null,
        [FromQuery] int? pageEnd = null,
        [FromQuery] bool useOCR = false)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var request = new ExtractionRequest
        {
            Content = memoryStream.ToArray(),
            Type = DocumentType.PDF,
            Options = new ExtractionOptions
            {
                PageStart = pageStart,
                PageEnd = pageEnd,
                UseOCR = useOCR,
                ExtractMetadata = true
            }
        };

        var result = await _extractionService.ExtractAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    #endregion

    #region Chunking Endpoints

    /// <summary>
    /// Chunk text using specified strategy
    /// </summary>
    [HttpPost("chunk")]
    public async Task<ActionResult<ChunkingResult>> ChunkText([FromBody] ChunkingRequest request)
    {
        var result = await _chunkingService.ChunkTextAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Chunk text using fixed-size strategy
    /// </summary>
    [HttpPost("chunk/fixed-size")]
    public async Task<ActionResult<ChunkingResult>> ChunkByFixedSize(
        [FromBody] string text,
        [FromQuery] int chunkSize = 1000,
        [FromQuery] int chunkOverlap = 200)
    {
        var options = new ChunkingOptions
        {
            ChunkSize = chunkSize,
            ChunkOverlap = chunkOverlap
        };

        var result = await _chunkingService.ChunkByFixedSizeAsync(text, options);
        return Ok(result);
    }

    /// <summary>
    /// Chunk text by sentences
    /// </summary>
    [HttpPost("chunk/sentence")]
    public async Task<ActionResult<ChunkingResult>> ChunkBySentence(
        [FromBody] string text,
        [FromQuery] int maxChunkSize = 2000)
    {
        var options = new ChunkingOptions { MaxChunkSize = maxChunkSize };
        var result = await _chunkingService.ChunkBySentenceAsync(text, options);
        return Ok(result);
    }

    /// <summary>
    /// Chunk text by paragraphs
    /// </summary>
    [HttpPost("chunk/paragraph")]
    public async Task<ActionResult<ChunkingResult>> ChunkByParagraph(
        [FromBody] string text,
        [FromQuery] int minChunkSize = 100,
        [FromQuery] int maxChunkSize = 2000)
    {
        var options = new ChunkingOptions
        {
            MinChunkSize = minChunkSize,
            MaxChunkSize = maxChunkSize
        };

        var result = await _chunkingService.ChunkByParagraphAsync(text, options);
        return Ok(result);
    }

    /// <summary>
    /// Chunk text using sliding window
    /// </summary>
    [HttpPost("chunk/sliding-window")]
    public async Task<ActionResult<ChunkingResult>> ChunkBySlidingWindow(
        [FromBody] string text,
        [FromQuery] int chunkSize = 1000,
        [FromQuery] int chunkOverlap = 300)
    {
        var options = new ChunkingOptions
        {
            ChunkSize = chunkSize,
            ChunkOverlap = chunkOverlap
        };

        var result = await _chunkingService.ChunkBySlidingWindowAsync(text, options);
        return Ok(result);
    }

    #endregion

    #region Preprocessing Endpoints

    /// <summary>
    /// Preprocess text with specified actions
    /// </summary>
    [HttpPost("preprocess")]
    public async Task<ActionResult<PreprocessingResult>> PreprocessText([FromBody] PreprocessingRequest request)
    {
        var result = await _preprocessingService.PreprocessAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Remove whitespace from text
    /// </summary>
    [HttpPost("preprocess/whitespace")]
    public async Task<ActionResult<string>> RemoveWhitespace(
        [FromBody] string text,
        [FromQuery] bool preserveParagraphs = true)
    {
        var result = await _preprocessingService.RemoveWhitespaceAsync(text, preserveParagraphs);
        return Ok(result);
    }

    /// <summary>
    /// Normalize whitespace in text
    /// </summary>
    [HttpPost("preprocess/normalize")]
    public async Task<ActionResult<string>> NormalizeWhitespace([FromBody] string text)
    {
        var result = await _preprocessingService.NormalizeWhitespaceAsync(text);
        return Ok(result);
    }

    /// <summary>
    /// Remove URLs from text
    /// </summary>
    [HttpPost("preprocess/remove-urls")]
    public async Task<ActionResult<string>> RemoveUrls([FromBody] string text)
    {
        var result = await _preprocessingService.RemoveUrlsAsync(text);
        return Ok(result);
    }

    /// <summary>
    /// Convert text to lowercase
    /// </summary>
    [HttpPost("preprocess/lowercase")]
    public async Task<ActionResult<string>> ToLowercase([FromBody] string text)
    {
        var result = await _preprocessingService.ToLowercaseAsync(text);
        return Ok(result);
    }

    #endregion

    #region OCR Endpoints

    /// <summary>
    /// Extract text from image using OCR
    /// </summary>
    [HttpPost("ocr")]
    public async Task<ActionResult<OCRResult>> ExtractTextFromImage([FromBody] OCRRequest request)
    {
        var result = await _ocrService.ExtractTextAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Extract text from uploaded image
    /// </summary>
    [HttpPost("ocr/upload")]
    public async Task<ActionResult<OCRResult>> ExtractTextFromImageFile(
        IFormFile file,
        [FromQuery] string language = "eng",
        [FromQuery] int dpi = 300)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var request = new OCRRequest
        {
            ImageData = memoryStream.ToArray(),
            Options = new OCROptions
            {
                Language = language,
                DPI = dpi
            }
        };

        var result = await _ocrService.ExtractTextAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    #endregion

    #region Metadata Endpoints

    /// <summary>
    /// Extract metadata from document
    /// </summary>
    [HttpPost("metadata/extract")]
    public async Task<ActionResult<DocumentMetadata>> ExtractMetadata([FromBody] Document document)
    {
        var result = await _metadataService.ExtractMetadataAsync(document);
        return Ok(result);
    }

    /// <summary>
    /// Enrich metadata with text analysis
    /// </summary>
    [HttpPost("metadata/enrich")]
    public async Task<ActionResult<DocumentMetadata>> EnrichMetadata(
        [FromBody] string text,
        [FromQuery] string? title = null)
    {
        var metadata = new DocumentMetadata { Title = title };
        var result = await _metadataService.EnrichMetadataAsync(metadata, text);
        return Ok(result);
    }

    /// <summary>
    /// Detect language of text
    /// </summary>
    [HttpPost("metadata/language")]
    public async Task<ActionResult<string>> DetectLanguage([FromBody] string text)
    {
        var result = await _metadataService.DetectLanguageAsync(text);
        return Ok(result);
    }

    #endregion

    #region Batch Processing Endpoints

    /// <summary>
    /// Process multiple documents in batch
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult<BatchProcessingResult>> ProcessBatch([FromBody] BatchProcessingRequest request)
    {
        var result = await _batchProcessingService.ProcessBatchAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Process multiple uploaded files
    /// </summary>
    [HttpPost("batch/upload")]
    public async Task<ActionResult<BatchProcessingResult>> ProcessBatchUpload(
        List<IFormFile> files,
        [FromQuery] ChunkingStrategy chunkingStrategy = ChunkingStrategy.FixedSize,
        [FromQuery] int chunkSize = 1000,
        [FromQuery] bool enableOCR = false)
    {
        if (files == null || !files.Any())
            return BadRequest("No files uploaded");

        var documents = new List<Document>();

        foreach (var file in files)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            documents.Add(new Document
            {
                FileName = file.FileName,
                Type = _extractionService.DetectDocumentType(file.FileName),
                Content = memoryStream.ToArray(),
                Size = file.Length
            });
        }

        var request = new BatchProcessingRequest
        {
            Documents = documents,
            ChunkingStrategy = chunkingStrategy,
            ChunkingOptions = new ChunkingOptions { ChunkSize = chunkSize },
            EnableOCR = enableOCR
        };

        var result = await _batchProcessingService.ProcessBatchAsync(request);
        return Ok(result);
    }

    #endregion

    #region Complete Pipeline Endpoint

    /// <summary>
    /// Complete document processing pipeline: extract, preprocess, and chunk
    /// </summary>
    [HttpPost("pipeline")]
    public async Task<ActionResult<object>> ProcessPipeline(
        IFormFile file,
        [FromQuery] ChunkingStrategy chunkingStrategy = ChunkingStrategy.FixedSize,
        [FromQuery] int chunkSize = 1000,
        [FromQuery] int chunkOverlap = 200,
        [FromQuery] bool useOCR = false,
        [FromQuery] bool removeUrls = false,
        [FromQuery] bool normalizeWhitespace = true)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        // Step 1: Extract
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var extractionRequest = new ExtractionRequest
        {
            Content = memoryStream.ToArray(),
            Type = _extractionService.DetectDocumentType(file.FileName),
            Options = new ExtractionOptions
            {
                ExtractMetadata = true,
                UseOCR = useOCR
            }
        };

        var extractionResult = await _extractionService.ExtractAsync(extractionRequest);

        if (!extractionResult.Success)
            return BadRequest(extractionResult);

        // Step 2: Preprocess
        var preprocessingActions = new List<PreprocessingAction>();
        if (normalizeWhitespace) preprocessingActions.Add(PreprocessingAction.NormalizeWhitespace);
        if (removeUrls) preprocessingActions.Add(PreprocessingAction.RemoveUrls);

        var processedText = extractionResult.Text;

        if (preprocessingActions.Any())
        {
            var preprocessingRequest = new PreprocessingRequest
            {
                Text = extractionResult.Text,
                Actions = preprocessingActions
            };

            var preprocessingResult = await _preprocessingService.PreprocessAsync(preprocessingRequest);
            processedText = preprocessingResult.ProcessedText;
        }

        // Step 3: Chunk
        var chunkingRequest = new ChunkingRequest
        {
            Text = processedText,
            Strategy = chunkingStrategy,
            Options = new ChunkingOptions
            {
                ChunkSize = chunkSize,
                ChunkOverlap = chunkOverlap
            },
            Metadata = extractionResult.Metadata
        };

        var chunkingResult = await _chunkingService.ChunkTextAsync(chunkingRequest);

        // Step 4: Enrich metadata
        var enrichedMetadata = await _metadataService.EnrichMetadataAsync(
            extractionResult.Metadata,
            extractionResult.Text);

        return Ok(new
        {
            FileName = file.FileName,
            Metadata = enrichedMetadata,
            ExtractedText = extractionResult.Text,
            ProcessedText = processedText,
            Chunks = chunkingResult.Chunks,
            TotalChunks = chunkingResult.TotalChunks,
            Pages = extractionResult.Pages.Count,
            Images = extractionResult.Images.Count,
            Tables = extractionResult.Tables.Count
        });
    }

    #endregion
}
