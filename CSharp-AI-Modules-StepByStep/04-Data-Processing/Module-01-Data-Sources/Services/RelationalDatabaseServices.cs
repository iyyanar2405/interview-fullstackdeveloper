using AI.DataSources.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace AI.DataSources.Services;

public interface ISqlServerService
{
    Task<ConnectionTestResult> TestConnectionAsync(string connectionString);
    Task<QueryResult> ExecuteQueryAsync(string connectionString, QueryRequest request);
    Task<int> ExecuteNonQueryAsync(string connectionString, string query, Dictionary<string, object>? parameters = null);
    Task<T?> ExecuteScalarAsync<T>(string connectionString, string query, Dictionary<string, object>? parameters = null);
    Task<BulkInsertResult> BulkInsertAsync(string connectionString, BulkInsertRequest request);
}

public class SqlServerService : ISqlServerService
{
    private readonly ILogger<SqlServerService> _logger;

    public SqlServerService(ILogger<SqlServerService> logger)
    {
        _logger = logger;
    }

    public async Task<ConnectionTestResult> TestConnectionAsync(string connectionString)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new ConnectionTestResult();

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var version = connection.ServerVersion;
            result.IsSuccessful = true;
            result.Message = "Connection successful";
            result.Version = version;
            result.Metadata["Database"] = connection.Database;
            result.Metadata["DataSource"] = connection.DataSource;

            _logger.LogInformation("SQL Server connection test successful: {DataSource}", connection.DataSource);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.Message = $"Connection failed: {ex.Message}";
            _logger.LogError(ex, "SQL Server connection test failed");
        }
        finally
        {
            stopwatch.Stop();
            result.ResponseTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<QueryResult> ExecuteQueryAsync(string connectionString, QueryRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new QueryResult();

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var dynamicParams = new DynamicParameters();
            foreach (var param in request.Parameters)
            {
                dynamicParams.Add(param.Key, param.Value);
            }

            var data = await connection.QueryAsync(request.QueryText, dynamicParams, 
                commandTimeout: request.Timeout ?? 30);

            result.Rows = data.Select(row => (IDictionary<string, object>)row)
                .Select(dict => new Dictionary<string, object>(dict))
                .ToList();

            result.RowCount = result.Rows.Count;
            result.IsSuccessful = true;

            _logger.LogInformation("Query executed successfully: {RowCount} rows returned", result.RowCount);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error executing SQL Server query");
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<int> ExecuteNonQueryAsync(string connectionString, string query, 
        Dictionary<string, object>? parameters = null)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var dynamicParams = new DynamicParameters();
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    dynamicParams.Add(param.Key, param.Value);
                }
            }

            var rowsAffected = await connection.ExecuteAsync(query, dynamicParams);

            _logger.LogInformation("Non-query executed: {RowsAffected} rows affected", rowsAffected);

            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing non-query");
            throw;
        }
    }

    public async Task<T?> ExecuteScalarAsync<T>(string connectionString, string query, 
        Dictionary<string, object>? parameters = null)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var dynamicParams = new DynamicParameters();
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    dynamicParams.Add(param.Key, param.Value);
                }
            }

            var result = await connection.ExecuteScalarAsync<T>(query, dynamicParams);

            _logger.LogDebug("Scalar query executed successfully");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing scalar query");
            throw;
        }
    }

    public async Task<BulkInsertResult> BulkInsertAsync(string connectionString, BulkInsertRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new BulkInsertResult();

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var dataTable = ConvertToDataTable(request.Data);

            using var bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = request.TableName,
                BatchSize = request.BatchSize
            };

            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(dataTable);

            result.IsSuccessful = true;
            result.RowsInserted = request.Data.Count;

            _logger.LogInformation("Bulk insert completed: {RowsInserted} rows", result.RowsInserted);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.Errors.Add(ex.Message);
            _logger.LogError(ex, "Error during bulk insert");
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
        }

        return result;
    }

    private DataTable ConvertToDataTable(List<Dictionary<string, object>> data)
    {
        var dataTable = new DataTable();

        if (data.Count == 0)
            return dataTable;

        // Add columns
        foreach (var key in data[0].Keys)
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

public interface IPostgreSqlService
{
    Task<ConnectionTestResult> TestConnectionAsync(string connectionString);
    Task<QueryResult> ExecuteQueryAsync(string connectionString, QueryRequest request);
    Task<int> ExecuteNonQueryAsync(string connectionString, string query, Dictionary<string, object>? parameters = null);
    Task<BulkInsertResult> BulkInsertAsync(string connectionString, BulkInsertRequest request);
}

public class PostgreSqlService : IPostgreSqlService
{
    private readonly ILogger<PostgreSqlService> _logger;

    public PostgreSqlService(ILogger<PostgreSqlService> logger)
    {
        _logger = logger;
    }

    public async Task<ConnectionTestResult> TestConnectionAsync(string connectionString)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new ConnectionTestResult();

        try
        {
            using var connection = new Npgsql.NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            result.IsSuccessful = true;
            result.Message = "Connection successful";
            result.Version = connection.ServerVersion;
            result.Metadata["Database"] = connection.Database;
            result.Metadata["Host"] = connection.Host;

            _logger.LogInformation("PostgreSQL connection test successful: {Host}", connection.Host);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.Message = $"Connection failed: {ex.Message}";
            _logger.LogError(ex, "PostgreSQL connection test failed");
        }
        finally
        {
            stopwatch.Stop();
            result.ResponseTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<QueryResult> ExecuteQueryAsync(string connectionString, QueryRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new QueryResult();

        try
        {
            using var connection = new Npgsql.NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var dynamicParams = new DynamicParameters();
            foreach (var param in request.Parameters)
            {
                dynamicParams.Add(param.Key, param.Value);
            }

            var data = await connection.QueryAsync(request.QueryText, dynamicParams,
                commandTimeout: request.Timeout ?? 30);

            result.Rows = data.Select(row => (IDictionary<string, object>)row)
                .Select(dict => new Dictionary<string, object>(dict))
                .ToList();

            result.RowCount = result.Rows.Count;
            result.IsSuccessful = true;

            _logger.LogInformation("PostgreSQL query executed: {RowCount} rows", result.RowCount);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error executing PostgreSQL query");
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<int> ExecuteNonQueryAsync(string connectionString, string query,
        Dictionary<string, object>? parameters = null)
    {
        try
        {
            using var connection = new Npgsql.NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var dynamicParams = new DynamicParameters();
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    dynamicParams.Add(param.Key, param.Value);
                }
            }

            var rowsAffected = await connection.ExecuteAsync(query, dynamicParams);

            _logger.LogInformation("PostgreSQL non-query executed: {RowsAffected} rows", rowsAffected);

            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing PostgreSQL non-query");
            throw;
        }
    }

    public async Task<BulkInsertResult> BulkInsertAsync(string connectionString, BulkInsertRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new BulkInsertResult();

        try
        {
            using var connection = new Npgsql.NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var transaction = request.UseTransaction ? await connection.BeginTransactionAsync() : null;

            try
            {
                int inserted = 0;
                foreach (var batch in request.Data.Chunk(request.BatchSize))
                {
                    var columns = string.Join(", ", batch[0].Keys);
                    var values = string.Join(", ", Enumerable.Range(0, batch[0].Keys.Count)
                        .Select(i => $"@p{i}"));

                    var query = $"INSERT INTO {request.TableName} ({columns}) VALUES ({values})";

                    foreach (var row in batch)
                    {
                        var dynamicParams = new DynamicParameters();
                        int paramIndex = 0;
                        foreach (var value in row.Values)
                        {
                            dynamicParams.Add($"p{paramIndex++}", value);
                        }

                        await connection.ExecuteAsync(query, dynamicParams, transaction);
                        inserted++;
                    }
                }

                if (transaction != null)
                    await transaction.CommitAsync();

                result.IsSuccessful = true;
                result.RowsInserted = inserted;

                _logger.LogInformation("PostgreSQL bulk insert completed: {RowsInserted} rows", inserted);
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.Errors.Add(ex.Message);
            _logger.LogError(ex, "Error during PostgreSQL bulk insert");
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
        }

        return result;
    }
}
