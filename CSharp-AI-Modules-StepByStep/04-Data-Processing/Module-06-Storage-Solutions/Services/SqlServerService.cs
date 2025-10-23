using Dapper;
using Microsoft.Data.SqlClient;
using Module_06_Storage_Solutions.Models;
using System.Data;
using System.Diagnostics;

namespace Module_06_Storage_Solutions.Services;

public interface ISqlServerService
{
    Task<SqlQueryResult> ExecuteQueryAsync(SqlQuery query);
    Task<T?> ExecuteScalarAsync<T>(SqlQuery query);
    Task<List<T>> QueryAsync<T>(SqlQuery query);
    Task<int> ExecuteAsync(SqlQuery query);
    Task<SqlQueryResult> ExecuteStoredProcedureAsync(string procedureName, Dictionary<string, object?> parameters);
    Task<bool> BulkInsertAsync<T>(List<T> data, BulkInsertOptions options);
    Task<bool> ExecuteTransactionAsync(List<SqlQuery> queries);
    Task<QueryPerformanceStats> AnalyzeQueryPerformanceAsync(string query);
    Task<List<IndexSuggestion>> GetIndexSuggestionsAsync(string tableName);
}

public class SqlServerService : ISqlServerService
{
    private readonly ILogger<SqlServerService> _logger;
    private readonly string _connectionString;

    public SqlServerService(ILogger<SqlServerService> logger, IConfiguration configuration)
    {
        _logger = logger;
        var config = configuration.GetSection("SqlServer").Get<SqlServerConfig>()
            ?? throw new InvalidOperationException("SqlServer configuration not found");
        
        _connectionString = BuildConnectionString(config);
    }

    private string BuildConnectionString(SqlServerConfig config)
    {
        return $"Server={config.Server};Database={config.Database};User Id={config.UserId};" +
               $"Password={config.Password};TrustServerCertificate={config.TrustServerCertificate};" +
               $"MultipleActiveResultSets={config.MultipleActiveResultSets};" +
               $"Max Pool Size={config.MaxPoolSize};Min Pool Size={config.MinPoolSize};" +
               $"Connect Timeout={config.CommandTimeout};";
    }

    public async Task<SqlQueryResult> ExecuteQueryAsync(SqlQuery query)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new SqlQueryResult();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(query.Query, connection)
            {
                CommandTimeout = query.Timeout ?? 30
            };

            foreach (var param in query.Parameters)
            {
                command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
            }

            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                result.Data.Add(row);
            }

            result.RowsAffected = reader.RecordsAffected;
            result.Success = true;

            _logger.LogInformation("Executed SQL query successfully. Rows: {Rows}, Time: {Time}ms",
                result.Data.Count, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing SQL query");
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<T?> ExecuteScalarAsync<T>(SqlQuery query)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            foreach (var param in query.Parameters)
            {
                parameters.Add($"@{param.Key}", param.Value);
            }

            var result = await connection.ExecuteScalarAsync<T>(
                query.Query,
                parameters,
                commandTimeout: query.Timeout ?? 30);

            _logger.LogInformation("Executed scalar query successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing scalar query");
            throw;
        }
    }

    public async Task<List<T>> QueryAsync<T>(SqlQuery query)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            foreach (var param in query.Parameters)
            {
                parameters.Add($"@{param.Key}", param.Value);
            }

            var result = await connection.QueryAsync<T>(
                query.Query,
                parameters,
                commandTimeout: query.Timeout ?? 30);

            var list = result.ToList();
            _logger.LogInformation("Query returned {Count} records", list.Count);
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query");
            throw;
        }
    }

    public async Task<int> ExecuteAsync(SqlQuery query)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            foreach (var param in query.Parameters)
            {
                parameters.Add($"@{param.Key}", param.Value);
            }

            IDbTransaction? transaction = null;
            if (query.UseTransaction)
            {
                transaction = connection.BeginTransaction();
            }

            var rowsAffected = await connection.ExecuteAsync(
                query.Query,
                parameters,
                transaction,
                query.Timeout ?? 30);

            transaction?.Commit();

            _logger.LogInformation("Executed command, {Rows} rows affected", rowsAffected);
            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command");
            throw;
        }
    }

    public async Task<SqlQueryResult> ExecuteStoredProcedureAsync(string procedureName, Dictionary<string, object?> parameters)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new SqlQueryResult();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
            }

            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                result.Data.Add(row);
            }

            result.RowsAffected = reader.RecordsAffected;
            result.Success = true;

            _logger.LogInformation("Executed stored procedure {Procedure} successfully", procedureName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure {Procedure}", procedureName);
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<bool> BulkInsertAsync<T>(List<T> data, BulkInsertOptions options)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var dataTable = ConvertToDataTable(data);

            using var bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = options.TableName,
                BatchSize = options.BatchSize,
                BulkCopyTimeout = options.Timeout,
                EnableStreaming = options.EnableStreaming
            };

            if (options.CheckConstraints)
                bulkCopy.SqlRowsCopied += (sender, e) => 
                    _logger.LogDebug("Copied {Rows} rows", e.RowsCopied);

            await bulkCopy.WriteToServerAsync(dataTable);

            _logger.LogInformation("Bulk inserted {Count} records into {Table}",
                data.Count, options.TableName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk insert into {Table}", options.TableName);
            return false;
        }
    }

    public async Task<bool> ExecuteTransactionAsync(List<SqlQuery> queries)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            
            try
            {
                foreach (var query in queries)
                {
                    var parameters = new DynamicParameters();
                    foreach (var param in query.Parameters)
                    {
                        parameters.Add($"@{param.Key}", param.Value);
                    }

                    await connection.ExecuteAsync(
                        query.Query,
                        parameters,
                        transaction,
                        query.Timeout ?? 30);
                }

                transaction.Commit();
                _logger.LogInformation("Transaction completed successfully with {Count} queries", queries.Count);
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing transaction");
            return false;
        }
    }

    public async Task<QueryPerformanceStats> AnalyzeQueryPerformanceAsync(string query)
    {
        var stats = new QueryPerformanceStats { Query = query };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Enable statistics
            await connection.ExecuteAsync("SET STATISTICS IO ON");
            await connection.ExecuteAsync("SET STATISTICS TIME ON");

            var result = await connection.QueryAsync<dynamic>(query);
            stats.RowsAffected = result.Count();

            stopwatch.Stop();
            stats.ExecutionTime = stopwatch.Elapsed;

            // Get execution plan
            var planQuery = $"SET SHOWPLAN_XML ON; {query}; SET SHOWPLAN_XML OFF;";
            // Note: Actual implementation would parse XML execution plan

            _logger.LogInformation("Query performance analysis completed in {Time}ms",
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing query performance");
            stopwatch.Stop();
            stats.ExecutionTime = stopwatch.Elapsed;
        }

        return stats;
    }

    public async Task<List<IndexSuggestion>> GetIndexSuggestionsAsync(string tableName)
    {
        var suggestions = new List<IndexSuggestion>();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    migs.avg_user_impact * (migs.user_seeks + migs.user_scans) AS Impact,
                    mid.statement AS TableName,
                    mid.equality_columns,
                    mid.inequality_columns,
                    mid.included_columns
                FROM sys.dm_db_missing_index_groups mig
                INNER JOIN sys.dm_db_missing_index_group_stats migs ON migs.group_handle = mig.index_group_handle
                INNER JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
                WHERE mid.statement LIKE @TableName
                ORDER BY Impact DESC";

            var results = await connection.QueryAsync<dynamic>(
                query,
                new { TableName = $"%{tableName}%" });

            foreach (var result in results)
            {
                var suggestion = new IndexSuggestion
                {
                    TableName = tableName,
                    EstimatedImprovement = result.Impact,
                    Reason = "Missing index detected by SQL Server"
                };

                if (!string.IsNullOrEmpty(result.equality_columns))
                    suggestion.Columns.AddRange(result.equality_columns.Split(','));
                if (!string.IsNullOrEmpty(result.inequality_columns))
                    suggestion.Columns.AddRange(result.inequality_columns.Split(','));

                suggestions.Add(suggestion);
            }

            _logger.LogInformation("Found {Count} index suggestions for {Table}",
                suggestions.Count, tableName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting index suggestions for {Table}", tableName);
        }

        return suggestions;
    }

    private DataTable ConvertToDataTable<T>(List<T> data)
    {
        var dataTable = new DataTable();
        var properties = typeof(T).GetProperties();

        foreach (var prop in properties)
        {
            dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        foreach (var item in data)
        {
            var row = dataTable.NewRow();
            foreach (var prop in properties)
            {
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            }
            dataTable.Rows.Add(row);
        }

        return dataTable;
    }
}
