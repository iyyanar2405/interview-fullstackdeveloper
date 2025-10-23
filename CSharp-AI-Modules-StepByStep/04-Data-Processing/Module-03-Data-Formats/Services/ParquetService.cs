using AI.DataFormats.Models;
using Parquet;
using Parquet.Data;
using Parquet.Schema;
using System.Diagnostics;

namespace AI.DataFormats.Services;

public interface IParquetService
{
    Task<DataFormatResponse<List<Dictionary<string, object>>>> ReadParquetFileAsync(string filePath, ParquetOptions? options = null);
    Task<DataFormatResponse<bool>> WriteParquetFileAsync(string filePath, List<Dictionary<string, object>> data, ParquetOptions? options = null);
    Task<DataFormatResponse<ParquetSchema>> GetSchemaAsync(string filePath);
}

public class ParquetService : IParquetService
{
    private readonly ILogger<ParquetService> _logger;

    public ParquetService(ILogger<ParquetService> logger)
    {
        _logger = logger;
    }

    public async Task<DataFormatResponse<List<Dictionary<string, object>>>> ReadParquetFileAsync(
        string filePath,
        ParquetOptions? options = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<List<Dictionary<string, object>>>();

        try
        {
            options ??= new ParquetOptions();
            var records = new List<Dictionary<string, object>>();

            using var stream = File.OpenRead(filePath);
            using var parquetReader = await ParquetReader.CreateAsync(stream);

            // Read all row groups
            for (int i = 0; i < parquetReader.RowGroupCount; i++)
            {
                using var rowGroupReader = parquetReader.OpenRowGroupReader(i);

                // Get all fields
                var dataFields = parquetReader.Schema.GetDataFields();
                var columnData = new Dictionary<string, Array>();

                // Read each column
                foreach (var field in dataFields)
                {
                    var column = await rowGroupReader.ReadColumnAsync(field);
                    columnData[field.Name] = column.Data;
                }

                // Convert to dictionary records
                var rowCount = columnData.First().Value.Length;
                for (int row = 0; row < rowCount; row++)
                {
                    var record = new Dictionary<string, object>();

                    foreach (var field in dataFields)
                    {
                        var value = columnData[field.Name].GetValue(row);
                        record[field.Name] = value ?? string.Empty;
                    }

                    records.Add(record);
                }
            }

            response.Success = true;
            response.Data = records;
            response.RecordCount = records.Count;
            response.SizeBytes = new FileInfo(filePath).Length;

            _logger.LogInformation("Read Parquet file: {FilePath}, {Rows} rows",
                filePath, records.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error reading Parquet file: {FilePath}", filePath);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<bool>> WriteParquetFileAsync(
        string filePath,
        List<Dictionary<string, object>> data,
        ParquetOptions? options = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<bool>();

        try
        {
            options ??= new ParquetOptions();

            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (data.Count == 0)
            {
                response.Success = false;
                response.Error = "No data to write";
                return response;
            }

            // Infer schema from first record
            var firstRecord = data.First();
            var fields = new List<DataField>();

            foreach (var kvp in firstRecord)
            {
                var fieldType = InferParquetType(kvp.Value);
                fields.Add(new DataField(kvp.Key, fieldType));
            }

            var schema = new Parquet.Schema.ParquetSchema(fields);

            using var stream = File.Create(filePath);
            using var parquetWriter = await ParquetWriter.CreateAsync(schema, stream);

            // Write in row groups
            var rowGroupSize = options.RowGroupSize;
            for (int i = 0; i < data.Count; i += rowGroupSize)
            {
                var batch = data.Skip(i).Take(rowGroupSize).ToList();

                using var groupWriter = parquetWriter.CreateRowGroup();

                // Write each column
                foreach (var field in fields)
                {
                    var columnData = batch.Select(r => r.ContainsKey(field.Name) ? r[field.Name] : null).ToArray();
                    await groupWriter.WriteColumnAsync(new DataColumn(field, columnData));
                }
            }

            response.Success = true;
            response.Data = true;
            response.RecordCount = data.Count;
            response.SizeBytes = new FileInfo(filePath).Length;

            _logger.LogInformation("Wrote Parquet file: {FilePath}, {Rows} rows",
                filePath, data.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error writing Parquet file: {FilePath}", filePath);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<ParquetSchema>> GetSchemaAsync(string filePath)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<ParquetSchema>();

        try
        {
            using var stream = File.OpenRead(filePath);
            using var parquetReader = await ParquetReader.CreateAsync(stream);

            var schema = new ParquetSchema();
            var dataFields = parquetReader.Schema.GetDataFields();

            foreach (var field in dataFields)
            {
                schema.Fields.Add(new ParquetField
                {
                    Name = field.Name,
                    DataType = field.DataType.ToString(),
                    IsNullable = field.IsNullable,
                    IsRepeated = field.IsArray
                });
            }

            response.Success = true;
            response.Data = schema;

            _logger.LogInformation("Retrieved Parquet schema: {FilePath}, {Fields} fields",
                filePath, schema.Fields.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error getting Parquet schema: {FilePath}", filePath);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    private Type InferParquetType(object value)
    {
        if (value == null)
            return typeof(string);

        return value switch
        {
            int => typeof(int),
            long => typeof(long),
            float => typeof(float),
            double => typeof(double),
            bool => typeof(bool),
            DateTime => typeof(DateTime),
            decimal => typeof(decimal),
            byte[] => typeof(byte[]),
            _ => typeof(string)
        };
    }
}
