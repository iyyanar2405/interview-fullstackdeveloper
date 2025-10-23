using AI.ETL.Models;
using Microsoft.Data.SqlClient;
using Npgsql;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics;

namespace AI.ETL.Services;

public interface IExtractionService
{
    Task<TransformationResult> ExtractFromSqlServerAsync(DataSourceConfig config);
    Task<TransformationResult> ExtractFromPostgreSqlAsync(DataSourceConfig config);
    Task<TransformationResult> ExtractFromMongoDbAsync(DataSourceConfig config);
    Task<TransformationResult> ExtractFromCsvFileAsync(string filePath);
    Task<TransformationResult> ExtractFromJsonFileAsync(string filePath);
}

public class ExtractionService : IExtractionService
{
    private readonly ILogger<ExtractionService> _logger;

    public ExtractionService(ILogger<ExtractionService> logger)
    {
        _logger = logger;
    }

    public async Task<TransformationResult> ExtractFromSqlServerAsync(DataSourceConfig config)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            using var connection = new SqlConnection(config.ConnectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(config.Query, connection)
            {
                CommandTimeout = config.TimeoutSeconds
            };

            using var reader = await command.ExecuteReaderAsync();
            var data = new List<Dictionary<string, object>>();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var fieldName = reader.GetName(i);
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[fieldName] = value!;
                }
                data.Add(row);
            }

            result.Success = true;
            result.Data = data;
            result.RecordsProcessed = data.Count;

            _logger.LogInformation("Extracted {Count} records from SQL Server", data.Count);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error extracting from SQL Server");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<TransformationResult> ExtractFromPostgreSqlAsync(DataSourceConfig config)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            using var connection = new NpgsqlConnection(config.ConnectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand(config.Query, connection)
            {
                CommandTimeout = config.TimeoutSeconds
            };

            using var reader = await command.ExecuteReaderAsync();
            var data = new List<Dictionary<string, object>>();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var fieldName = reader.GetName(i);
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[fieldName] = value!;
                }
                data.Add(row);
            }

            result.Success = true;
            result.Data = data;
            result.RecordsProcessed = data.Count;

            _logger.LogInformation("Extracted {Count} records from PostgreSQL", data.Count);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error extracting from PostgreSQL");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<TransformationResult> ExtractFromMongoDbAsync(DataSourceConfig config)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            var client = new MongoClient(config.ConnectionString);
            var database = client.GetDatabase(config.Properties.GetValueOrDefault("Database", ""));
            var collection = database.GetCollection<BsonDocument>(config.Properties.GetValueOrDefault("Collection", ""));

            var filter = string.IsNullOrEmpty(config.Query)
                ? Builders<BsonDocument>.Filter.Empty
                : BsonDocument.Parse(config.Query);

            var documents = await collection.Find(filter).ToListAsync();
            var data = new List<Dictionary<string, object>>();

            foreach (var doc in documents)
            {
                var row = new Dictionary<string, object>();
                foreach (var element in doc.Elements)
                {
                    row[element.Name] = BsonTypeMapper.MapToDotNetValue(element.Value);
                }
                data.Add(row);
            }

            result.Success = true;
            result.Data = data;
            result.RecordsProcessed = data.Count;

            _logger.LogInformation("Extracted {Count} records from MongoDB", data.Count);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error extracting from MongoDB");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<TransformationResult> ExtractFromCsvFileAsync(string filePath)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);

            await csv.ReadAsync();
            csv.ReadHeader();
            var headers = csv.HeaderRecord;

            var data = new List<Dictionary<string, object>>();

            while (await csv.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                foreach (var header in headers!)
                {
                    row[header] = csv.GetField(header) ?? string.Empty;
                }
                data.Add(row);
            }

            result.Success = true;
            result.Data = data;
            result.RecordsProcessed = data.Count;

            _logger.LogInformation("Extracted {Count} records from CSV file", data.Count);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error extracting from CSV file");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<TransformationResult> ExtractFromJsonFileAsync(string filePath)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            var jsonContent = await File.ReadAllTextAsync(filePath);
            var data = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonContent);

            if (data == null)
            {
                data = new List<Dictionary<string, object>>();
                var singleObject = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
                if (singleObject != null)
                {
                    data.Add(singleObject);
                }
            }

            result.Success = true;
            result.Data = data;
            result.RecordsProcessed = data.Count;

            _logger.LogInformation("Extracted {Count} records from JSON file", data.Count);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error extracting from JSON file");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }
}
