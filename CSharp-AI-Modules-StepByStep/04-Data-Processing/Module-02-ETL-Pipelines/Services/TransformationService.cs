using AI.ETL.Models;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace AI.ETL.Services;

public interface ITransformationService
{
    Task<TransformationResult> ApplyTransformationsAsync(
        List<Dictionary<string, object>> data,
        List<TransformationStep> steps);
    Task<TransformationResult> MapFieldsAsync(
        List<Dictionary<string, object>> data,
        SchemaMapping mapping);
    Task<TransformationResult> FilterDataAsync(
        List<Dictionary<string, object>> data,
        Dictionary<string, object> filterCriteria);
    Task<TransformationResult> AggregateDataAsync(
        List<Dictionary<string, object>> data,
        string groupByField,
        Dictionary<string, string> aggregations);
}

public class TransformationService : ITransformationService
{
    private readonly ILogger<TransformationService> _logger;

    public TransformationService(ILogger<TransformationService> logger)
    {
        _logger = logger;
    }

    public async Task<TransformationResult> ApplyTransformationsAsync(
        List<Dictionary<string, object>> data,
        List<TransformationStep> steps)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult { Data = data };

        await Task.CompletedTask;

        try
        {
            var orderedSteps = steps.Where(s => s.Enabled).OrderBy(s => s.Order);

            foreach (var step in orderedSteps)
            {
                _logger.LogInformation("Applying transformation: {StepName}", step.Name);

                switch (step.Type)
                {
                    case TransformationType.Map:
                        result.Data = ApplyMapping(result.Data, step.Parameters);
                        break;

                    case TransformationType.Filter:
                        result.Data = ApplyFilter(result.Data, step.Parameters);
                        break;

                    case TransformationType.Cleanse:
                        result.Data = ApplyCleansing(result.Data, step.Parameters);
                        break;

                    case TransformationType.Validate:
                        var validationErrors = ApplyValidation(result.Data, step.Parameters);
                        result.Errors.AddRange(validationErrors);
                        break;

                    case TransformationType.Enrich:
                        result.Data = ApplyEnrichment(result.Data, step.Parameters);
                        break;

                    default:
                        _logger.LogWarning("Unknown transformation type: {Type}", step.Type);
                        break;
                }
            }

            result.Success = true;
            result.RecordsProcessed = result.Data.Count;
            result.RecordsFailed = result.Errors.Count;

            _logger.LogInformation("Transformations completed: {Processed} processed, {Failed} failed",
                result.RecordsProcessed, result.RecordsFailed);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error applying transformations");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<TransformationResult> MapFieldsAsync(
        List<Dictionary<string, object>> data,
        SchemaMapping mapping)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            var mappedData = new List<Dictionary<string, object>>();

            foreach (var row in data)
            {
                var mappedRow = new Dictionary<string, object>();

                foreach (var fieldMapping in mapping.FieldMappings)
                {
                    if (row.ContainsKey(fieldMapping.SourceField))
                    {
                        var value = row[fieldMapping.SourceField];
                        
                        // Apply transformation expression if provided
                        if (!string.IsNullOrEmpty(fieldMapping.TransformExpression))
                        {
                            value = ApplyTransformExpression(value, fieldMapping.TransformExpression);
                        }

                        mappedRow[fieldMapping.DestinationField] = value;
                    }
                    else if (!string.IsNullOrEmpty(fieldMapping.DefaultValue))
                    {
                        mappedRow[fieldMapping.DestinationField] = fieldMapping.DefaultValue;
                    }
                    else if (fieldMapping.IsRequired)
                    {
                        result.Errors.Add(new TransformationError
                        {
                            FieldName = fieldMapping.SourceField,
                            ErrorMessage = $"Required field '{fieldMapping.SourceField}' not found",
                            Severity = ValidationSeverity.Error
                        });
                    }
                }

                mappedData.Add(mappedRow);
            }

            result.Success = true;
            result.Data = mappedData;
            result.RecordsProcessed = mappedData.Count;

            _logger.LogInformation("Mapped {Count} records", mappedData.Count);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error mapping fields");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<TransformationResult> FilterDataAsync(
        List<Dictionary<string, object>> data,
        Dictionary<string, object> filterCriteria)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            var filteredData = data.Where(row =>
            {
                foreach (var criteria in filterCriteria)
                {
                    if (!row.ContainsKey(criteria.Key))
                        return false;

                    var rowValue = row[criteria.Key]?.ToString() ?? "";
                    var criteriaValue = criteria.Value?.ToString() ?? "";

                    if (!rowValue.Equals(criteriaValue, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                return true;
            }).ToList();

            result.Success = true;
            result.Data = filteredData;
            result.RecordsProcessed = filteredData.Count;

            _logger.LogInformation("Filtered data: {Original} -> {Filtered} records",
                data.Count, filteredData.Count);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error filtering data");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<TransformationResult> AggregateDataAsync(
        List<Dictionary<string, object>> data,
        string groupByField,
        Dictionary<string, string> aggregations)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var result = new TransformationResult();

        try
        {
            var grouped = data.GroupBy(row => row.ContainsKey(groupByField) 
                ? row[groupByField]?.ToString() ?? "" 
                : "");

            var aggregatedData = new List<Dictionary<string, object>>();

            foreach (var group in grouped)
            {
                var aggRow = new Dictionary<string, object>
                {
                    [groupByField] = group.Key
                };

                foreach (var agg in aggregations)
                {
                    var field = agg.Key;
                    var operation = agg.Value.ToLowerInvariant();

                    switch (operation)
                    {
                        case "count":
                            aggRow[$"{field}_Count"] = group.Count();
                            break;

                        case "sum":
                            aggRow[$"{field}_Sum"] = group
                                .Where(r => r.ContainsKey(field))
                                .Sum(r => Convert.ToDouble(r[field]));
                            break;

                        case "avg":
                            aggRow[$"{field}_Avg"] = group
                                .Where(r => r.ContainsKey(field))
                                .Average(r => Convert.ToDouble(r[field]));
                            break;

                        case "min":
                            aggRow[$"{field}_Min"] = group
                                .Where(r => r.ContainsKey(field))
                                .Min(r => Convert.ToDouble(r[field]));
                            break;

                        case "max":
                            aggRow[$"{field}_Max"] = group
                                .Where(r => r.ContainsKey(field))
                                .Max(r => Convert.ToDouble(r[field]));
                            break;
                    }
                }

                aggregatedData.Add(aggRow);
            }

            result.Success = true;
            result.Data = aggregatedData;
            result.RecordsProcessed = aggregatedData.Count;

            _logger.LogInformation("Aggregated {Original} records into {Aggregated} groups",
                data.Count, aggregatedData.Count);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new TransformationError
            {
                ErrorMessage = ex.Message,
                Severity = ValidationSeverity.Critical
            });
            _logger.LogError(ex, "Error aggregating data");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    // Helper Methods
    private List<Dictionary<string, object>> ApplyMapping(
        List<Dictionary<string, object>> data,
        Dictionary<string, object> parameters)
    {
        var mappedData = new List<Dictionary<string, object>>();

        foreach (var row in data)
        {
            var mappedRow = new Dictionary<string, object>();

            foreach (var param in parameters)
            {
                var sourceField = param.Key;
                var destField = param.Value.ToString() ?? sourceField;

                if (row.ContainsKey(sourceField))
                {
                    mappedRow[destField] = row[sourceField];
                }
            }

            mappedData.Add(mappedRow);
        }

        return mappedData;
    }

    private List<Dictionary<string, object>> ApplyFilter(
        List<Dictionary<string, object>> data,
        Dictionary<string, object> parameters)
    {
        return data.Where(row =>
        {
            foreach (var param in parameters)
            {
                if (!row.ContainsKey(param.Key))
                    return false;

                var value = row[param.Key]?.ToString() ?? "";
                var filterValue = param.Value?.ToString() ?? "";

                if (!value.Contains(filterValue, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return true;
        }).ToList();
    }

    private List<Dictionary<string, object>> ApplyCleansing(
        List<Dictionary<string, object>> data,
        Dictionary<string, object> parameters)
    {
        foreach (var row in data)
        {
            foreach (var key in row.Keys.ToList())
            {
                var value = row[key]?.ToString() ?? "";

                // Trim whitespace
                value = value.Trim();

                // Remove special characters if specified
                if (parameters.ContainsKey("RemoveSpecialChars") &&
                    parameters["RemoveSpecialChars"].ToString() == "true")
                {
                    value = Regex.Replace(value, @"[^a-zA-Z0-9\s]", "");
                }

                // Convert to uppercase/lowercase if specified
                if (parameters.ContainsKey("ToUpper") &&
                    parameters["ToUpper"].ToString() == "true")
                {
                    value = value.ToUpperInvariant();
                }
                else if (parameters.ContainsKey("ToLower") &&
                         parameters["ToLower"].ToString() == "true")
                {
                    value = value.ToLowerInvariant();
                }

                row[key] = value;
            }
        }

        return data;
    }

    private List<TransformationError> ApplyValidation(
        List<Dictionary<string, object>> data,
        Dictionary<string, object> parameters)
    {
        var errors = new List<TransformationError>();
        var recordIndex = 0;

        foreach (var row in data)
        {
            foreach (var param in parameters)
            {
                var fieldName = param.Key;
                var validationType = param.Value.ToString() ?? "";

                if (!row.ContainsKey(fieldName))
                {
                    errors.Add(new TransformationError
                    {
                        RecordIndex = recordIndex,
                        FieldName = fieldName,
                        ErrorMessage = $"Field '{fieldName}' is missing",
                        Severity = ValidationSeverity.Error
                    });
                    continue;
                }

                var value = row[fieldName]?.ToString() ?? "";

                switch (validationType.ToLowerInvariant())
                {
                    case "required":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            errors.Add(new TransformationError
                            {
                                RecordIndex = recordIndex,
                                FieldName = fieldName,
                                ErrorMessage = $"Field '{fieldName}' is required",
                                Severity = ValidationSeverity.Error
                            });
                        }
                        break;

                    case "email":
                        if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        {
                            errors.Add(new TransformationError
                            {
                                RecordIndex = recordIndex,
                                FieldName = fieldName,
                                ErrorMessage = $"Invalid email format",
                                Severity = ValidationSeverity.Warning
                            });
                        }
                        break;

                    case "numeric":
                        if (!double.TryParse(value, out _))
                        {
                            errors.Add(new TransformationError
                            {
                                RecordIndex = recordIndex,
                                FieldName = fieldName,
                                ErrorMessage = $"Field must be numeric",
                                Severity = ValidationSeverity.Error
                            });
                        }
                        break;
                }
            }

            recordIndex++;
        }

        return errors;
    }

    private List<Dictionary<string, object>> ApplyEnrichment(
        List<Dictionary<string, object>> data,
        Dictionary<string, object> parameters)
    {
        foreach (var row in data)
        {
            foreach (var param in parameters)
            {
                row[param.Key] = param.Value;
            }
        }

        return data;
    }

    private object ApplyTransformExpression(object value, string expression)
    {
        var strValue = value?.ToString() ?? "";

        // Simple expression evaluation
        if (expression.Contains("UPPER"))
            return strValue.ToUpperInvariant();
        if (expression.Contains("LOWER"))
            return strValue.ToLowerInvariant();
        if (expression.Contains("TRIM"))
            return strValue.Trim();

        return value;
    }
}
