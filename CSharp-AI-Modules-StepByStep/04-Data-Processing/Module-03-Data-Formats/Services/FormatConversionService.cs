using AI.DataFormats.Models;
using System.Diagnostics;

namespace AI.DataFormats.Services;

public interface IFormatConversionService
{
    Task<FormatConversionResult> ConvertAsync(FormatConversionRequest request);
    Task<DataFormatResponse<SchemaDetectionResult>> DetectSchemaAsync(string filePath, DataFormat format);
}

public class FormatConversionService : IFormatConversionService
{
    private readonly IJsonService _jsonService;
    private readonly ICsvService _csvService;
    private readonly IExcelService _excelService;
    private readonly IParquetService _parquetService;
    private readonly IXmlService _xmlService;
    private readonly ILogger<FormatConversionService> _logger;

    public FormatConversionService(
        IJsonService jsonService,
        ICsvService csvService,
        IExcelService excelService,
        IParquetService parquetService,
        IXmlService xmlService,
        ILogger<FormatConversionService> logger)
    {
        _jsonService = jsonService;
        _csvService = csvService;
        _excelService = excelService;
        _parquetService = parquetService;
        _xmlService = xmlService;
        _logger = logger;
    }

    public async Task<FormatConversionResult> ConvertAsync(FormatConversionRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new FormatConversionResult();

        try
        {
            _logger.LogInformation("Converting from {Source} to {Target}",
                request.SourceFormat, request.TargetFormat);

            // Step 1: Read source data
            List<Dictionary<string, object>> data;

            if (!string.IsNullOrEmpty(request.SourcePath))
            {
                data = await ReadSourceFileAsync(request.SourcePath, request.SourceFormat);
            }
            else if (request.SourceContent != null)
            {
                var content = System.Text.Encoding.UTF8.GetString(request.SourceContent);
                data = await ReadSourceContentAsync(content, request.SourceFormat);
            }
            else
            {
                result.Error = "No source data provided";
                return result;
            }

            result.SourceSizeBytes = request.SourceContent?.Length ?? new FileInfo(request.SourcePath!).Length;
            result.RecordsProcessed = data.Count;

            // Step 2: Write target data
            if (!string.IsNullOrEmpty(request.TargetPath))
            {
                await WriteTargetFileAsync(request.TargetPath, data, request.TargetFormat);
                result.ConvertedFilePath = request.TargetPath;
                result.TargetSizeBytes = new FileInfo(request.TargetPath).Length;
            }
            else
            {
                var content = await WriteTargetContentAsync(data, request.TargetFormat);
                result.ConvertedContent = System.Text.Encoding.UTF8.GetBytes(content);
                result.TargetSizeBytes = result.ConvertedContent.Length;
            }

            result.Success = true;

            _logger.LogInformation("Conversion completed: {Records} records, {SourceSize} -> {TargetSize} bytes",
                result.RecordsProcessed, result.SourceSizeBytes, result.TargetSizeBytes);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = ex.Message;
            _logger.LogError(ex, "Error during format conversion");
        }
        finally
        {
            stopwatch.Stop();
            result.ConversionTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<DataFormatResponse<SchemaDetectionResult>> DetectSchemaAsync(string filePath, DataFormat format)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<SchemaDetectionResult>();

        try
        {
            var data = await ReadSourceFileAsync(filePath, format);
            var result = DetectSchema(data);

            response.Success = true;
            response.Data = result;
            response.RecordCount = data.Count;

            _logger.LogInformation("Schema detected: {Fields} fields from {Records} records",
                result.Fields.Count, data.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error detecting schema");
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    // Helper methods
    private async Task<List<Dictionary<string, object>>> ReadSourceFileAsync(string filePath, DataFormat format)
    {
        return format switch
        {
            DataFormat.Json => await ReadJsonAsync(filePath),
            DataFormat.Csv => await ReadCsvAsync(filePath),
            DataFormat.Excel => await ReadExcelAsync(filePath),
            DataFormat.Parquet => await ReadParquetAsync(filePath),
            DataFormat.Xml => await ReadXmlAsync(filePath),
            _ => throw new NotSupportedException($"Format {format} not supported for reading")
        };
    }

    private async Task<List<Dictionary<string, object>>> ReadSourceContentAsync(string content, DataFormat format)
    {
        return format switch
        {
            DataFormat.Json => await ReadJsonContentAsync(content),
            DataFormat.Csv => await ReadCsvContentAsync(content),
            DataFormat.Xml => await ReadXmlContentAsync(content),
            _ => throw new NotSupportedException($"Format {format} not supported for content reading")
        };
    }

    private async Task WriteTargetFileAsync(string filePath, List<Dictionary<string, object>> data, DataFormat format)
    {
        switch (format)
        {
            case DataFormat.Json:
                await _jsonService.WriteJsonFileAsync(filePath, data);
                break;
            case DataFormat.Csv:
                await _csvService.WriteCsvFileAsync(filePath, data);
                break;
            case DataFormat.Excel:
                var sheets = new Dictionary<string, List<Dictionary<string, object>>> { ["Data"] = data };
                await _excelService.WriteExcelFileAsync(filePath, sheets);
                break;
            case DataFormat.Parquet:
                await _parquetService.WriteParquetFileAsync(filePath, data);
                break;
            case DataFormat.Xml:
                await _xmlService.WriteXmlFileAsync(filePath, data);
                break;
            default:
                throw new NotSupportedException($"Format {format} not supported for writing");
        }
    }

    private async Task<string> WriteTargetContentAsync(List<Dictionary<string, object>> data, DataFormat format)
    {
        return format switch
        {
            DataFormat.Json => (await _jsonService.SerializeToJsonAsync(data)).Data ?? string.Empty,
            DataFormat.Csv => (await _csvService.WriteCsvAsync(data)).Data ?? string.Empty,
            DataFormat.Xml => (await _xmlService.SerializeToXmlAsync(data)).Data ?? string.Empty,
            _ => throw new NotSupportedException($"Format {format} not supported for content writing")
        };
    }

    private async Task<List<Dictionary<string, object>>> ReadJsonAsync(string filePath)
    {
        var result = await _jsonService.ReadJsonFileAsync(filePath);
        if (result.Data?.Type == Newtonsoft.Json.Linq.JTokenType.Array)
        {
            return result.Data.ToObject<List<Dictionary<string, object>>>() ?? new();
        }
        return new List<Dictionary<string, object>> { result.Data?.ToObject<Dictionary<string, object>>() ?? new() };
    }

    private async Task<List<Dictionary<string, object>>> ReadJsonContentAsync(string content)
    {
        var result = await _jsonService.ParseJsonAsync(content);
        if (result.Data?.Type == Newtonsoft.Json.Linq.JTokenType.Array)
        {
            return result.Data.ToObject<List<Dictionary<string, object>>>() ?? new();
        }
        return new List<Dictionary<string, object>> { result.Data?.ToObject<Dictionary<string, object>>() ?? new() };
    }

    private async Task<List<Dictionary<string, object>>> ReadCsvAsync(string filePath)
    {
        var result = await _csvService.ReadCsvFileAsync(filePath);
        return result.Data?.Records ?? new();
    }

    private async Task<List<Dictionary<string, object>>> ReadCsvContentAsync(string content)
    {
        var result = await _csvService.ParseCsvAsync(content);
        return result.Data?.Records ?? new();
    }

    private async Task<List<Dictionary<string, object>>> ReadExcelAsync(string filePath)
    {
        var result = await _excelService.ReadExcelFileAsync(filePath);
        return result.Data?.Sheets.Values.FirstOrDefault() ?? new();
    }

    private async Task<List<Dictionary<string, object>>> ReadParquetAsync(string filePath)
    {
        var result = await _parquetService.ReadParquetFileAsync(filePath);
        return result.Data ?? new();
    }

    private async Task<List<Dictionary<string, object>>> ReadXmlAsync(string filePath)
    {
        var result = await _xmlService.ReadXmlFileAsync(filePath);
        return result.Data?.Items ?? new();
    }

    private async Task<List<Dictionary<string, object>>> ReadXmlContentAsync(string content)
    {
        var result = await _xmlService.ParseXmlAsync(content);
        return result.Data?.Items ?? new();
    }

    private SchemaDetectionResult DetectSchema(List<Dictionary<string, object>> data)
    {
        var result = new SchemaDetectionResult
        {
            SampleSize = data.Count
        };

        if (data.Count == 0)
            return result;

        var allFields = data.SelectMany(r => r.Keys).Distinct().ToList();

        foreach (var fieldName in allFields)
        {
            var fieldSchema = new FieldSchema
            {
                Name = fieldName,
                TotalCount = data.Count
            };

            var typeCounts = new Dictionary<string, int>();

            foreach (var record in data)
            {
                if (!record.ContainsKey(fieldName) || record[fieldName] == null)
                {
                    fieldSchema.NullCount++;
                    continue;
                }

                var value = record[fieldName];
                var typeName = GetTypeName(value);
                typeCounts[typeName] = typeCounts.GetValueOrDefault(typeName, 0) + 1;

                if (fieldSchema.SampleValues.Count < 5)
                {
                    fieldSchema.SampleValues.Add(value);
                }
            }

            fieldSchema.IsNullable = fieldSchema.NullCount > 0;

            if (typeCounts.Count > 0)
            {
                var mostCommon = typeCounts.OrderByDescending(kvp => kvp.Value).First();
                fieldSchema.DetectedType = mostCommon.Key;

                result.TypeStatistics[fieldName] = new TypeStatistics
                {
                    TypeCounts = typeCounts,
                    MostCommonType = mostCommon.Key,
                    TypeConfidence = (double)mostCommon.Value / (data.Count - fieldSchema.NullCount)
                };
            }

            result.Fields.Add(fieldSchema);
        }

        return result;
    }

    private string GetTypeName(object value)
    {
        return value switch
        {
            int => "Integer",
            long => "Long",
            float => "Float",
            double => "Double",
            decimal => "Decimal",
            bool => "Boolean",
            DateTime => "DateTime",
            string => "String",
            _ => "Object"
        };
    }
}
