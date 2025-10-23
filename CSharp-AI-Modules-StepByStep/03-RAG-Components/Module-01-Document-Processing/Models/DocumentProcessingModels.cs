namespace AI.DocumentProcessing.Models;

#region Enums

public enum DocumentType
{
    PDF,
    Word,
    Excel,
    PowerPoint,
    Text,
    HTML,
    Markdown,
    Image,
    CSV,
    JSON,
    XML,
    Unknown
}

public enum ChunkingStrategy
{
    FixedSize,           // Fixed character/token count
    Sentence,            // Split by sentences
    Paragraph,           // Split by paragraphs
    Semantic,            // Semantic similarity-based
    SlidingWindow,       // Overlapping chunks
    Recursive,           // Hierarchical splitting
    Custom               // Custom logic
}

public enum PreprocessingAction
{
    RemoveWhitespace,
    NormalizeWhitespace,
    RemoveSpecialCharacters,
    Lowercase,
    RemoveStopWords,
    Lemmatize,
    Stem,
    RemoveUrls,
    RemoveEmails,
    RemoveNumbers
}

#endregion

#region Document Models

public class Document
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FileName { get; set; } = string.Empty;
    public DocumentType Type { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DocumentMetadata Metadata { get; set; } = new();
}

public class DocumentMetadata
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public DateTime? CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public string? Subject { get; set; }
    public string? Keywords { get; set; }
    public int? PageCount { get; set; }
    public int? WordCount { get; set; }
    public string? Language { get; set; }
    public Dictionary<string, object> CustomMetadata { get; set; } = new();
}

#endregion

#region Extraction Models

public class ExtractionRequest
{
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public DocumentType Type { get; set; }
    public string? FilePath { get; set; }
    public ExtractionOptions Options { get; set; } = new();
}

public class ExtractionOptions
{
    public bool ExtractImages { get; set; } = false;
    public bool ExtractTables { get; set; } = false;
    public bool ExtractMetadata { get; set; } = true;
    public bool UseOCR { get; set; } = false;
    public bool PreserveFormatting { get; set; } = false;
    public int? PageStart { get; set; }
    public int? PageEnd { get; set; }
}

public class ExtractionResult
{
    public string Text { get; set; } = string.Empty;
    public DocumentMetadata Metadata { get; set; } = new();
    public List<ExtractedImage> Images { get; set; } = new();
    public List<ExtractedTable> Tables { get; set; } = new();
    public List<ExtractedPage> Pages { get; set; } = new();
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class ExtractedImage
{
    public int PageNumber { get; set; }
    public byte[] ImageData { get; set; } = Array.Empty<byte>();
    public string Format { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public string? OCRText { get; set; }
}

public class ExtractedTable
{
    public int PageNumber { get; set; }
    public List<string> Headers { get; set; } = new();
    public List<List<string>> Rows { get; set; } = new();
}

public class ExtractedPage
{
    public int PageNumber { get; set; }
    public string Text { get; set; } = string.Empty;
    public int CharacterCount { get; set; }
    public List<ExtractedImage> Images { get; set; } = new();
    public List<ExtractedTable> Tables { get; set; } = new();
}

#endregion

#region Chunking Models

public class ChunkingRequest
{
    public string Text { get; set; } = string.Empty;
    public ChunkingStrategy Strategy { get; set; }
    public ChunkingOptions Options { get; set; } = new();
    public DocumentMetadata? Metadata { get; set; }
}

public class ChunkingOptions
{
    public int ChunkSize { get; set; } = 1000;
    public int ChunkOverlap { get; set; } = 200;
    public int MinChunkSize { get; set; } = 100;
    public int MaxChunkSize { get; set; } = 2000;
    public string Separator { get; set; } = "\n\n";
    public bool PreserveStructure { get; set; } = true;
    public bool IncludeMetadata { get; set; } = true;
}

public class ChunkingResult
{
    public List<DocumentChunk> Chunks { get; set; } = new();
    public int TotalChunks { get; set; }
    public int TotalCharacters { get; set; }
    public ChunkingStrategy StrategyUsed { get; set; }
}

public class DocumentChunk
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public int CharacterCount { get; set; }
    public DocumentMetadata? Metadata { get; set; }
    public Dictionary<string, object> ChunkMetadata { get; set; } = new();
}

#endregion

#region Preprocessing Models

public class PreprocessingRequest
{
    public string Text { get; set; } = string.Empty;
    public List<PreprocessingAction> Actions { get; set; } = new();
    public PreprocessingOptions Options { get; set; } = new();
}

public class PreprocessingOptions
{
    public bool PreserveParagraphs { get; set; } = true;
    public bool RemoveDuplicateSpaces { get; set; } = true;
    public string? CustomStopWords { get; set; }
    public string? Locale { get; set; } = "en";
}

public class PreprocessingResult
{
    public string ProcessedText { get; set; } = string.Empty;
    public string OriginalText { get; set; } = string.Empty;
    public List<string> AppliedActions { get; set; } = new();
    public PreprocessingStatistics Statistics { get; set; } = new();
}

public class PreprocessingStatistics
{
    public int OriginalLength { get; set; }
    public int ProcessedLength { get; set; }
    public int CharactersRemoved { get; set; }
    public int WordsRemoved { get; set; }
    public double ReductionPercentage { get; set; }
}

#endregion

#region OCR Models

public class OCRRequest
{
    public byte[] ImageData { get; set; } = Array.Empty<byte>();
    public string? ImagePath { get; set; }
    public OCROptions Options { get; set; } = new();
}

public class OCROptions
{
    public string Language { get; set; } = "eng";
    public int DPI { get; set; } = 300;
    public bool AutoRotate { get; set; } = true;
    public bool RemoveNoise { get; set; } = true;
    public OCREngine Engine { get; set; } = OCREngine.Tesseract;
}

public enum OCREngine
{
    Tesseract,
    Azure,
    Google,
    AWS
}

public class OCRResult
{
    public string Text { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public List<OCRWord> Words { get; set; } = new();
    public List<OCRLine> Lines { get; set; } = new();
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public class OCRWord
{
    public string Text { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public BoundingBox BoundingBox { get; set; } = new();
}

public class OCRLine
{
    public string Text { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public List<OCRWord> Words { get; set; } = new();
    public BoundingBox BoundingBox { get; set; } = new();
}

public class BoundingBox
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

#endregion

#region Batch Processing Models

public class BatchProcessingRequest
{
    public List<Document> Documents { get; set; } = new();
    public ChunkingStrategy ChunkingStrategy { get; set; } = ChunkingStrategy.FixedSize;
    public ChunkingOptions ChunkingOptions { get; set; } = new();
    public List<PreprocessingAction> PreprocessingActions { get; set; } = new();
    public bool EnableOCR { get; set; } = false;
}

public class BatchProcessingResult
{
    public List<ProcessedDocument> ProcessedDocuments { get; set; } = new();
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class ProcessedDocument
{
    public string DocumentId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public ExtractionResult ExtractionResult { get; set; } = new();
    public ChunkingResult ChunkingResult { get; set; } = new();
    public PreprocessingResult? PreprocessingResult { get; set; }
    public bool Success { get; set; }
    public TimeSpan ProcessingDuration { get; set; }
}

#endregion

#region Configuration Models

public class DocumentProcessingSettings
{
    public ExtractionOptions DefaultExtractionOptions { get; set; } = new();
    public ChunkingOptions DefaultChunkingOptions { get; set; } = new();
    public PreprocessingOptions DefaultPreprocessingOptions { get; set; } = new();
    public OCROptions DefaultOCROptions { get; set; } = new();
    public string TempDirectory { get; set; } = Path.GetTempPath();
    public int MaxFileSizeMB { get; set; } = 100;
    public bool EnableCaching { get; set; } = true;
    public int CacheDurationMinutes { get; set; } = 60;
}

#endregion
