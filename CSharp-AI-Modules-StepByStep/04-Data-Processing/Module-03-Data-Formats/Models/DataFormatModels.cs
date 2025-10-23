namespace AI.DataFormats.Models;

// Enums
public enum DataFormat
{
    Json,
    Csv,
    Excel,
    Parquet,
    Xml,
    Html,
    MessagePack,
    Protobuf,
    Avro,
    Binary
}

public enum ExcelFormat
{
    Xlsx,
    Xls,
    Csv
}

public enum CompressionType
{
    None,
    Gzip,
    Zip,
    Lz4,
    Snappy
}

public enum Encoding
{
    UTF8,
    UTF16,
    UTF32,
    ASCII,
    Latin1
}

// Base Models
public class DataFormatRequest
{
    public DataFormat Format { get; set; }
    public string? FilePath { get; set; }
    public byte[]? FileContent { get; set; }
    public string? Content { get; set; }
    public Dictionary<string, object> Options { get; set; } = new();
}

public class DataFormatResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public int RecordCount { get; set; }
    public long SizeBytes { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

// JSON Models
public class JsonProcessingOptions
{
    public bool Pretty { get; set; } = true;
    public bool IgnoreNullValues { get; set; } = false;
    public bool CamelCase { get; set; } = true;
    public int MaxDepth { get; set; } = 64;
    public bool ValidateSchema { get; set; } = false;
    public string? SchemaPath { get; set; }
}

public class JsonValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

// CSV Models
public class CsvOptions
{
    public char Delimiter { get; set; } = ',';
    public char Quote { get; set; } = '"';
    public bool HasHeader { get; set; } = true;
    public string Encoding { get; set; } = "UTF-8";
    public bool TrimFields { get; set; } = true;
    public bool SkipEmptyRows { get; set; } = true;
    public string? DateFormat { get; set; }
}

public class CsvParseResult
{
    public List<Dictionary<string, object>> Records { get; set; } = new();
    public List<string> Headers { get; set; } = new();
    public int TotalRows { get; set; }
    public int ErrorRows { get; set; }
    public List<CsvError> Errors { get; set; } = new();
}

public class CsvError
{
    public int RowNumber { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public string RawValue { get; set; } = string.Empty;
}

// Excel Models
public class ExcelOptions
{
    public ExcelFormat Format { get; set; } = ExcelFormat.Xlsx;
    public string? SheetName { get; set; }
    public int SheetIndex { get; set; } = 0;
    public bool HasHeader { get; set; } = true;
    public int StartRow { get; set; } = 1;
    public int? EndRow { get; set; }
    public bool SkipEmptyRows { get; set; } = true;
    public bool ReadAllSheets { get; set; } = false;
}

public class ExcelReadResult
{
    public Dictionary<string, List<Dictionary<string, object>>> Sheets { get; set; } = new();
    public List<string> SheetNames { get; set; } = new();
    public int TotalSheets { get; set; }
    public int TotalRows { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ExcelWriteOptions
{
    public string SheetName { get; set; } = "Sheet1";
    public bool AutoFitColumns { get; set; } = true;
    public bool ApplyStyles { get; set; } = false;
    public string? TemplatePath { get; set; }
}

// Parquet Models
public class ParquetOptions
{
    public CompressionType Compression { get; set; } = CompressionType.Snappy;
    public int RowGroupSize { get; set; } = 5000;
    public bool UseDataField { get; set; } = true;
}

public class ParquetSchema
{
    public List<ParquetField> Fields { get; set; } = new();
}

public class ParquetField
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; } = true;
    public bool IsRepeated { get; set; } = false;
}

// XML Models
public class XmlOptions
{
    public string RootElement { get; set; } = "Root";
    public string ItemElement { get; set; } = "Item";
    public bool Pretty { get; set; } = true;
    public bool OmitXmlDeclaration { get; set; } = false;
    public string Encoding { get; set; } = "UTF-8";
    public bool ValidateSchema { get; set; } = false;
    public string? XsdSchemaPath { get; set; }
}

public class XmlParseResult
{
    public Dictionary<string, object> Data { get; set; } = new();
    public List<Dictionary<string, object>> Items { get; set; } = new();
    public string RootElement { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}

// HTML Models
public class HtmlOptions
{
    public string? Selector { get; set; }
    public bool ExtractTables { get; set; } = false;
    public bool ExtractLinks { get; set; } = false;
    public bool ExtractImages { get; set; } = false;
    public bool CleanHtml { get; set; } = false;
}

public class HtmlParseResult
{
    public string Content { get; set; } = string.Empty;
    public List<HtmlTable> Tables { get; set; } = new();
    public List<HtmlLink> Links { get; set; } = new();
    public List<HtmlImage> Images { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class HtmlTable
{
    public List<string> Headers { get; set; } = new();
    public List<List<string>> Rows { get; set; } = new();
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }
}

public class HtmlLink
{
    public string Href { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? Title { get; set; }
}

public class HtmlImage
{
    public string Src { get; set; } = string.Empty;
    public string? Alt { get; set; }
    public string? Title { get; set; }
}

// Binary Format Models
public class BinaryFormatOptions
{
    public DataFormat Format { get; set; } = DataFormat.MessagePack;
    public CompressionType Compression { get; set; } = CompressionType.None;
    public bool UseCompression { get; set; } = false;
}

public class MessagePackOptions
{
    public bool UseLZ4Compression { get; set; } = false;
    public bool OldSpec { get; set; } = false;
}

// Conversion Models
public class FormatConversionRequest
{
    public DataFormat SourceFormat { get; set; }
    public DataFormat TargetFormat { get; set; }
    public string? SourcePath { get; set; }
    public string? TargetPath { get; set; }
    public byte[]? SourceContent { get; set; }
    public Dictionary<string, object> SourceOptions { get; set; } = new();
    public Dictionary<string, object> TargetOptions { get; set; } = new();
}

public class FormatConversionResult
{
    public bool Success { get; set; }
    public byte[]? ConvertedContent { get; set; }
    public string? ConvertedFilePath { get; set; }
    public long SourceSizeBytes { get; set; }
    public long TargetSizeBytes { get; set; }
    public int RecordsProcessed { get; set; }
    public TimeSpan ConversionTime { get; set; }
    public string? Error { get; set; }
}

// Schema Detection Models
public class SchemaDetectionResult
{
    public List<FieldSchema> Fields { get; set; } = new();
    public int SampleSize { get; set; }
    public Dictionary<string, TypeStatistics> TypeStatistics { get; set; } = new();
}

public class FieldSchema
{
    public string Name { get; set; } = string.Empty;
    public string DetectedType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public int NullCount { get; set; }
    public int TotalCount { get; set; }
    public List<object> SampleValues { get; set; } = new();
}

public class TypeStatistics
{
    public Dictionary<string, int> TypeCounts { get; set; } = new();
    public string MostCommonType { get; set; } = string.Empty;
    public double TypeConfidence { get; set; }
}

// Streaming Models
public class StreamingOptions
{
    public int ChunkSize { get; set; } = 1000;
    public bool ProcessInParallel { get; set; } = false;
    public int MaxDegreeOfParallelism { get; set; } = 4;
}

public class StreamingProgress
{
    public int RecordsProcessed { get; set; }
    public int TotalRecords { get; set; }
    public double PercentComplete { get; set; }
    public TimeSpan Elapsed { get; set; }
    public double RecordsPerSecond { get; set; }
}

// Batch Processing Models
public class BatchProcessingRequest
{
    public List<string> FilePaths { get; set; } = new();
    public DataFormat Format { get; set; }
    public Dictionary<string, object> Options { get; set; } = new();
    public bool ProcessInParallel { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = 4;
}

public class BatchProcessingResult
{
    public bool Success { get; set; }
    public int TotalFiles { get; set; }
    public int ProcessedFiles { get; set; }
    public int FailedFiles { get; set; }
    public List<string> SuccessfulFiles { get; set; } = new();
    public List<BatchFileError> Errors { get; set; } = new();
    public TimeSpan TotalProcessingTime { get; set; }
}

public class BatchFileError
{
    public string FilePath { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public DateTime ErrorTime { get; set; } = DateTime.UtcNow;
}

// API Response Model
public class ApiResponseModel<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponseModel<T> SuccessResponse(T data)
    {
        return new ApiResponseModel<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ApiResponseModel<T> ErrorResponse(string error)
    {
        return new ApiResponseModel<T>
        {
            Success = false,
            Error = error
        };
    }
}
