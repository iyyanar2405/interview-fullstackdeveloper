using Dapper;
using Npgsql;
using Module_06_Storage_Solutions.Models;
using System.Data;
using System.Diagnostics;

namespace Module_06_Storage_Solutions.Services;

public interface IPostgreSqlService
{
    Task<SqlQueryResult> ExecuteQueryAsync(PostgreSqlQuery query);
    Task<T?> ExecuteScalarAsync<T>(PostgreSqlQuery query);
    Task<List<T>> QueryAsync<T>(PostgreSqlQuery query);
    Task<int> ExecuteAsync(PostgreSqlQuery query);
    Task<bool> BulkInsertAsync<T>(List<T> data, string tableName);
    Task<bool> ExecuteTransactionAsync(List<PostgreSqlQuery> queries);
    Task<List<string>> GetTableNamesAsync();
    Task<bool> CreateIndexAsync(string tableName, List<string> columns, string indexName);
    Task<QueryPerformanceStats> ExplainQueryAsync(string query);
}

public class PostgreSqlService : IPostgreSqlService
{
    private readonly ILogger<PostgreSqlService> _logger;
    private readonly string _connectionString;

    public PostgreSqlService(ILogger<PostgreSqlService> logger, IConfiguration configuration)
    {
        _logger = logger;
        var config = configuration.GetSection("PostgreSQL").Get<PostgreSqlConfig>()
            ?? throw new InvalidOperationException("PostgreSQL configuration not found");
        
        _connectionString = BuildConnectionString(config);
    }

    private string BuildConnectionString(PostgreSqlConfig config)
    {
        return $"Host={config.Host};Port={config.Port};Database={config.Database};" +
               $"Username={config.Username};Password={config.Password};" +
               $"Pooling={config.Pooling};Maximum Pool Size={config.MaxPoolSize};" +
               $"Minimum Pool Size={config.MinPoolSize};Command Timeout={config.CommandTimeout};";
    }

    public async Task<SqlQueryResult> ExecuteQueryAsync(PostgreSqlQuery query)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new SqlQueryResult();

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand(query.Query, connection)
            {
                CommandTimeout = query.Timeout ?? 30
            };

            if (query.UsePreparedStatement)
            {
                await command.PrepareAsync();
            }

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

            _logger.LogInformation("Executed PostgreSQL query successfully. Rows: {Rows}, Time: {Time}ms",
                result.Data.Count, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing PostgreSQL query");
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

    public async Task<T?> ExecuteScalarAsync<T>(PostgreSqlQuery query)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
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

    public async Task<List<T>> QueryAsync<T>(PostgreSqlQuery query)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
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

    public async Task<int> ExecuteAsync(PostgreSqlQuery query)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            foreach (var param in query.Parameters)
            {
                parameters.Add($"@{param.Key}", param.Value);
            }

            var rowsAffected = await connection.ExecuteAsync(
                query.Query,
                parameters,
                commandTimeout: query.Timeout ?? 30);

            _logger.LogInformation("Executed command, {Rows} rows affected", rowsAffected);
            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command");
            throw;
        }
    }

    public async Task<bool> BulkInsertAsync<T>(List<T> data, string tableName)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var properties = typeof(T).GetProperties();
            var columnNames = string.Join(", ", properties.Select(p => p.Name));
            var tempTableName = $"temp_{tableName}_{Guid.NewGuid():N}";

            // Create temp table
            var createTempTableQuery = $@"
                CREATE TEMP TABLE {tempTableName} AS 
                SELECT * FROM {tableName} LIMIT 0";
            
            await connection.ExecuteAsync(createTempTableQuery);

            // Use COPY for bulk insert
            using (var writer = connection.BeginBinaryImport($"COPY {tempTableName} ({columnNames}) FROM STDIN (FORMAT BINARY)"))
            {
                foreach (var item in data)
                {
                    writer.StartRow();
                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(item);
                        writer.Write(value ?? DBNull.Value);
                    }
                }
                await writer.CompleteAsync();
            }

            // Insert from temp table to actual table
            await connection.ExecuteAsync($"INSERT INTO {tableName} SELECT * FROM {tempTableName}");

            _logger.LogInformation("Bulk inserted {Count} records into {Table}", data.Count, tableName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk insert into {Table}", tableName);
            return false;
        }
    }

    public async Task<bool> ExecuteTransactionAsync(List<PostgreSqlQuery> queries)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
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

                await transaction.CommitAsync();
                _logger.LogInformation("Transaction completed successfully with {Count} queries", queries.Count);
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing transaction");
            return false;
        }
    }

    public async Task<List<string>> GetTableNamesAsync()
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT table_name 
                FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_type = 'BASE TABLE'
                ORDER BY table_name";

            var tables = await connection.QueryAsync<string>(query);
            var tableList = tables.ToList();

            _logger.LogInformation("Retrieved {Count} table names", tableList.Count);
            return tableList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting table names");
            throw;
        }
    }

    public async Task<bool> CreateIndexAsync(string tableName, List<string> columns, string indexName)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var columnList = string.Join(", ", columns);
            var query = $"CREATE INDEX CONCURRENTLY IF NOT EXISTS {indexName} ON {tableName} ({columnList})";

            await connection.ExecuteAsync(query);

            _logger.LogInformation("Created index {Index} on {Table} with columns: {Columns}",
                indexName, tableName, columnList);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating index {Index} on {Table}", indexName, tableName);
            return false;
        }
    }

    public async Task<QueryPerformanceStats> ExplainQueryAsync(string query)
    {
        var stats = new QueryPerformanceStats { Query = query };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Get execution plan
            var explainQuery = $"EXPLAIN (ANALYZE, BUFFERS, FORMAT JSON) {query}";
            var planJson = await connection.QueryFirstAsync<string>(explainQuery);

            // Execute the actual query
            var result = await connection.QueryAsync<dynamic>(query);
            stats.RowsAffected = result.Count();

            stopwatch.Stop();
            stats.ExecutionTime = stopwatch.Elapsed;

            _logger.LogInformation("Query explain analysis completed in {Time}ms",
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error explaining query");
            stopwatch.Stop();
            stats.ExecutionTime = stopwatch.Elapsed;
        }

        return stats;
    }
}
