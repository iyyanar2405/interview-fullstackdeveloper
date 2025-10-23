using AI.DataFormats.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Diagnostics;
using System.Globalization;

namespace AI.DataFormats.Services;

public interface ICsvService
{
    Task<DataFormatResponse<CsvParseResult>> ParseCsvAsync(string csvContent, CsvOptions? options = null);
    Task<DataFormatResponse<CsvParseResult>> ReadCsvFileAsync(string filePath, CsvOptions? options = null);
    Task<DataFormatResponse<string>> WriteCsvAsync(List<Dictionary<string, object>> data, CsvOptions? options = null);
    Task<DataFormatResponse<bool>> WriteCsvFileAsync(string filePath, List<Dictionary<string, object>> data, CsvOptions? options = null);
    Task<DataFormatResponse<List<T>>> ParseCsvToObjectsAsync<T>(string csvContent, CsvOptions? options = null);
}

public class CsvService : ICsvService
{
    private readonly ILogger<CsvService> _logger;

    public CsvService(ILogger<CsvService> logger)
    {
        _logger = logger;
    }

    public async Task<DataFormatResponse<CsvParseResult>> ParseCsvAsync(string csvContent, CsvOptions? options = null)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<CsvParseResult>();
        var result = new CsvParseResult();

        try
        {
            options ??= new CsvOptions();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = options.Delimiter.ToString(),
                HasHeaderRecord = options.HasHeader,
                TrimOptions = options.TrimFields ? TrimOptions.Trim : TrimOptions.None,
                BadDataFound = context =>
                {
                    result.Errors.Add(new CsvError
                    {
                        RowNumber = context.Context.Parser.Row,
                        Error = "Bad data found",
                        RawValue = context.RawRecord
                    });
                }
            };

            using var reader = new StringReader(csvContent);
            using var csv = new CsvReader(reader, config);

            // Read header
            await csv.ReadAsync();
            csv.ReadHeader();
            result.Headers = csv.HeaderRecord?.ToList() ?? new List<string>();

            // Read records
            while (await csv.ReadAsync())
            {
                try
                {
                    var record = new Dictionary<string, object>();

                    foreach (var header in result.Headers)
                    {
                        var value = csv.GetField(header);
                        record[header] = value ?? string.Empty;
                    }

                    if (!options.SkipEmptyRows || record.Values.Any(v => !string.IsNullOrWhiteSpace(v.ToString())))
                    {
                        result.Records.Add(record);
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorRows++;
                    result.Errors.Add(new CsvError
                    {
                        RowNumber = csv.Context.Parser.Row,
                        Error = ex.Message
                    });
                }
            }

            result.TotalRows = result.Records.Count;

            response.Success = true;
            response.Data = result;
            response.RecordCount = result.TotalRows;
            response.SizeBytes = System.Text.Encoding.UTF8.GetByteCount(csvContent);

            _logger.LogInformation("Parsed CSV: {Rows} rows, {Errors} errors",
                result.TotalRows, result.ErrorRows);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error parsing CSV");
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<CsvParseResult>> ReadCsvFileAsync(string filePath, CsvOptions? options = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<CsvParseResult>();

        try
        {
            var csvContent = await File.ReadAllTextAsync(filePath);
            var parseResult = await ParseCsvAsync(csvContent, options);

            response.Success = parseResult.Success;
            response.Data = parseResult.Data;
            response.Error = parseResult.Error;
            response.RecordCount = parseResult.RecordCount;
            response.SizeBytes = parseResult.SizeBytes;

            response.Metadata["FilePath"] = filePath;
            response.Metadata["FileName"] = Path.GetFileName(filePath);

            _logger.LogInformation("Read CSV file: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error reading CSV file: {FilePath}", filePath);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<string>> WriteCsvAsync(
        List<Dictionary<string, object>> data,
        CsvOptions? options = null)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<string>();

        try
        {
            options ??= new CsvOptions();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = options.Delimiter.ToString(),
                HasHeaderRecord = options.HasHeader
            };

            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, config);

            if (data.Count == 0)
            {
                response.Success = true;
                response.Data = string.Empty;
                return response;
            }

            // Write header
            var headers = data.First().Keys.ToList();
            foreach (var header in headers)
            {
                csv.WriteField(header);
            }
            await csv.NextRecordAsync();

            // Write rows
            foreach (var row in data)
            {
                foreach (var header in headers)
                {
                    var value = row.ContainsKey(header) ? row[header] : string.Empty;
                    csv.WriteField(value);
                }
                await csv.NextRecordAsync();
            }

            var csvContent = writer.ToString();

            response.Success = true;
            response.Data = csvContent;
            response.RecordCount = data.Count;
            response.SizeBytes = System.Text.Encoding.UTF8.GetByteCount(csvContent);

            _logger.LogInformation("Generated CSV: {Rows} rows, {Size} bytes",
                data.Count, response.SizeBytes);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error writing CSV");
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<bool>> WriteCsvFileAsync(
        string filePath,
        List<Dictionary<string, object>> data,
        CsvOptions? options = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<bool>();

        try
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var writeResult = await WriteCsvAsync(data, options);

            if (!writeResult.Success)
            {
                response.Success = false;
                response.Error = writeResult.Error;
                return response;
            }

            await File.WriteAllTextAsync(filePath, writeResult.Data);

            response.Success = true;
            response.Data = true;
            response.RecordCount = writeResult.RecordCount;
            response.SizeBytes = writeResult.SizeBytes;

            _logger.LogInformation("Wrote CSV file: {FilePath}, {Rows} rows", filePath, data.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error writing CSV file: {FilePath}", filePath);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<List<T>>> ParseCsvToObjectsAsync<T>(string csvContent, CsvOptions? options = null)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<List<T>>();

        try
        {
            options ??= new CsvOptions();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = options.Delimiter.ToString(),
                HasHeaderRecord = options.HasHeader,
                TrimOptions = options.TrimFields ? TrimOptions.Trim : TrimOptions.None
            };

            using var reader = new StringReader(csvContent);
            using var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<T>().ToList();

            response.Success = true;
            response.Data = records;
            response.RecordCount = records.Count;

            _logger.LogInformation("Parsed CSV to {Type}: {Count} records",
                typeof(T).Name, records.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error parsing CSV to {Type}", typeof(T).Name);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }
}
