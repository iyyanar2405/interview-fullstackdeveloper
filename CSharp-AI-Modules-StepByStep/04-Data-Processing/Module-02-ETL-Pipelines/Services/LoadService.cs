using AI.ETL.Models;
using Microsoft.Data.SqlClient;
using Npgsql;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics;
using System.Data;

namespace AI.ETL.Services;

public interface ILoadService
{
    Task<TransformationResult> LoadToSqlServerAsync(
        List<Dictionary<string, object>> data,
        DataSourceConfig config);
    Task<TransformationResult> LoadToPostgreSqlAsync(
        List<Dictionary<string, object>> data,
        DataSourceConfig config);
    Task<TransformationResult> LoadToMongoDbAsync(
        List<Dictionary<string, object>> data,
        DataSourceConfig config);
    Task<TransformationResult> LoadToCsvFileAsync(
        List<Dictionary<string, object>> data,
        string filePath);
    Task<TransformationResult> LoadToJsonFileAsync(
        List<Dictionary<string, object>> data,
        string filePath);
}

public class LoadService : ILoadService
{
    private readonly ILogger<LoadService> _logger;

    public LoadService(ILogger<LoadService> logger)
    {
        _logger = logger;
    }

    public async Task<TransformationResult> LoadToSqlServerAsync(
        List<Dictionary<string, object>> data,
        DataSourceConfig config)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            using var connection = new SqlConnection(config.ConnectionString);
            await connection.OpenAsync();

            // Create DataTable
            var dataTable = CreateDataTable(data);

            // Bulk insert
            using var bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = config.TableName,
                BatchSize = 1000,
                BulkCopyTimeout = config.TimeoutSeconds
            };

            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(dataTable);

            result.Success = true;
            result.RecordsProcessed = data.Count;

            _logger.LogInformation("Loaded {Count} records to SQL Server table {Table}",
                data.Count, config.TableName);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.RecordsFailed = data.Count;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error loading to SQL Server");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<TransformationResult> LoadToPostgreSqlAsync(
        List<Dictionary<string, object>> data,
        DataSourceConfig config)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            using var connection = new NpgsqlConnection(config.ConnectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Get column names from first record
                var columns = data.First().Keys.ToList();
                var columnList = string.Join(", ", columns.Select(c => $"\"{c}\""));
                var parameterList = string.Join(", ", columns.Select((c, i) => $"@p{i}"));

                var insertQuery = $"INSERT INTO {config.TableName} ({columnList}) VALUES ({parameterList})";

                foreach (var row in data)
                {
                    using var command = new NpgsqlCommand(insertQuery, connection, transaction);

                    for (int i = 0; i < columns.Count; i++)
                    {
                        var value = row.ContainsKey(columns[i]) ? row[columns[i]] : DBNull.Value;
                        command.Parameters.AddWithValue($"@p{i}", value ?? DBNull.Value);
                    }

                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();

                result.Success = true;
                result.RecordsProcessed = data.Count;

                _logger.LogInformation("Loaded {Count} records to PostgreSQL table {Table}",
                    data.Count, config.TableName);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.RecordsFailed = data.Count;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error loading to PostgreSQL");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<TransformationResult> LoadToMongoDbAsync(
        List<Dictionary<string, object>> data,
        DataSourceConfig config)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            var client = new MongoClient(config.ConnectionString);
            var database = client.GetDatabase(config.Properties.GetValueOrDefault("Database", ""));
            var collection = database.GetCollection<BsonDocument>(config.Properties.GetValueOrDefault("Collection", ""));

            var documents = data.Select(row =>
            {
                var doc = new BsonDocument();
                foreach (var kvp in row)
                {
                    doc[kvp.Key] = BsonValue.Create(kvp.Value);
                }
                return doc;
            }).ToList();

            await collection.InsertManyAsync(documents);

            result.Success = true;
            result.RecordsProcessed = data.Count;

            _logger.LogInformation("Loaded {Count} records to MongoDB collection {Collection}",
                data.Count, config.Properties.GetValueOrDefault("Collection", ""));
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.RecordsFailed = data.Count;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error loading to MongoDB");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<TransformationResult> LoadToCsvFileAsync(
        List<Dictionary<string, object>> data,
        string filePath)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var writer = new StreamWriter(filePath);
            using var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture);

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
                    var value = row.ContainsKey(header) ? row[header] : "";
                    csv.WriteField(value);
                }
                await csv.NextRecordAsync();
            }

            result.Success = true;
            result.RecordsProcessed = data.Count;

            _logger.LogInformation("Loaded {Count} records to CSV file {File}", data.Count, filePath);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.RecordsFailed = data.Count;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error loading to CSV file");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<TransformationResult> LoadToJsonFileAsync(
        List<Dictionary<string, object>> data,
        string filePath)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = System.Text.Json.JsonSerializer.Serialize(data, options);
            await File.WriteAllTextAsync(filePath, json);

            result.Success = true;
            result.RecordsProcessed = data.Count;

            _logger.LogInformation("Loaded {Count} records to JSON file {File}", data.Count, filePath);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.RecordsFailed = data.Count;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error loading to JSON file");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    // Helper method to create DataTable from dictionary list
    private DataTable CreateDataTable(List<Dictionary<string, object>> data)
    {
        var dataTable = new DataTable();

        if (data.Count == 0)
            return dataTable;

        // Add columns
        foreach (var key in data.First().Keys)
        {
            dataTable.Columns.Add(key, typeof(object));
        }

        // Add rows
        foreach (var row in data)
        {
            var dataRow = dataTable.NewRow();
            foreach (var kvp in row)
            {
                dataRow[kvp.Key] = kvp.Value ?? DBNull.Value;
            }
            dataTable.Rows.Add(dataRow);
        }

        return dataTable;
    }
}
