using AI.ETL.Models;
using System.Text.RegularExpressions;

namespace AI.ETL.Services;

public interface IValidationService
{
    Task<ValidationResult> ValidateDataAsync(
        List<Dictionary<string, object>> data,
        List<ValidationRule> rules);
    Task<DataQualityReport> GenerateQualityReportAsync(
        string jobId,
        List<Dictionary<string, object>> data);
}

public class ValidationService : IValidationService
{
    private readonly ILogger<ValidationService> _logger;

    public ValidationService(ILogger<ValidationService> logger)
    {
        _logger = logger;
    }

    public async Task<ValidationResult> ValidateDataAsync(
        List<Dictionary<string, object>> data,
        List<ValidationRule> rules)
    {
        await Task.CompletedTask;

        var result = new ValidationResult
        {
            TotalRecords = data.Count
        };

        var recordIndex = 0;

        foreach (var row in data)
        {
            var isRecordValid = true;

            foreach (var rule in rules)
            {
                var validationError = ValidateField(row, rule, recordIndex);

                if (validationError != null)
                {
                    isRecordValid = false;

                    if (validationError.Severity == ValidationSeverity.Error ||
                        validationError.Severity == ValidationSeverity.Critical)
                    {
                        result.Errors.Add(validationError);
                    }
                    else
                    {
                        result.Warnings.Add(validationError);
                    }
                }
            }

            if (isRecordValid)
                result.ValidRecords++;
            else
                result.InvalidRecords++;

            recordIndex++;
        }

        result.IsValid = result.Errors.Count == 0;

        _logger.LogInformation("Validation completed: {Valid} valid, {Invalid} invalid records",
            result.ValidRecords, result.InvalidRecords);

        return result;
    }

    public async Task<DataQualityReport> GenerateQualityReportAsync(
        string jobId,
        List<Dictionary<string, object>> data)
    {
        await Task.CompletedTask;

        var report = new DataQualityReport
        {
            JobId = jobId,
            TotalRecords = data.Count
        };

        if (data.Count == 0)
        {
            report.QualityScore = 0;
            return report;
        }

        // Analyze each field
        var allFields = data.SelectMany(row => row.Keys).Distinct().ToList();

        foreach (var field in allFields)
        {
            var fieldMetrics = AnalyzeField(data, field);
            report.FieldMetrics[field] = fieldMetrics;
        }

        // Calculate overall quality score
        var avgCompleteness = report.FieldMetrics.Values.Average(m => m.CompletenessPercentage);
        var avgUniqueness = report.FieldMetrics.Values.Average(m => m.UniquenessPercentage);
        report.QualityScore = (avgCompleteness + avgUniqueness) / 2;

        report.ValidRecords = (int)(data.Count * avgCompleteness / 100);
        report.InvalidRecords = data.Count - report.ValidRecords;

        _logger.LogInformation("Quality report generated: Score={Score:F2}%, Valid={Valid}, Invalid={Invalid}",
            report.QualityScore, report.ValidRecords, report.InvalidRecords);

        return report;
    }

    private ValidationError? ValidateField(
        Dictionary<string, object> row,
        ValidationRule rule,
        int recordIndex)
    {
        var fieldName = rule.FieldName;

        // Check if field exists
        if (!row.ContainsKey(fieldName))
        {
            if (rule.RuleType.Equals("Required", StringComparison.OrdinalIgnoreCase))
            {
                return new ValidationError
                {
                    RecordIndex = recordIndex,
                    FieldName = fieldName,
                    RuleName = rule.Name,
                    Message = $"Required field '{fieldName}' is missing",
                    Severity = rule.Severity
                };
            }
            return null;
        }

        var value = row[fieldName];
        var stringValue = value?.ToString() ?? "";

        switch (rule.RuleType.ToLowerInvariant())
        {
            case "required":
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    return new ValidationError
                    {
                        RecordIndex = recordIndex,
                        FieldName = fieldName,
                        RuleName = rule.Name,
                        Message = $"Field '{fieldName}' cannot be empty",
                        Severity = rule.Severity,
                        ActualValue = value
                    };
                }
                break;

            case "range":
                if (rule.Parameters.ContainsKey("Min") && rule.Parameters.ContainsKey("Max"))
                {
                    if (double.TryParse(stringValue, out var numValue))
                    {
                        var min = Convert.ToDouble(rule.Parameters["Min"]);
                        var max = Convert.ToDouble(rule.Parameters["Max"]);

                        if (numValue < min || numValue > max)
                        {
                            return new ValidationError
                            {
                                RecordIndex = recordIndex,
                                FieldName = fieldName,
                                RuleName = rule.Name,
                                Message = $"Value must be between {min} and {max}",
                                Severity = rule.Severity,
                                ActualValue = numValue,
                                ExpectedValue = $"{min}-{max}"
                            };
                        }
                    }
                }
                break;

            case "pattern":
                if (rule.Parameters.ContainsKey("Regex"))
                {
                    var pattern = rule.Parameters["Regex"].ToString() ?? "";
                    if (!Regex.IsMatch(stringValue, pattern))
                    {
                        return new ValidationError
                        {
                            RecordIndex = recordIndex,
                            FieldName = fieldName,
                            RuleName = rule.Name,
                            Message = $"Value does not match required pattern",
                            Severity = rule.Severity,
                            ActualValue = value,
                            ExpectedValue = pattern
                        };
                    }
                }
                break;

            case "length":
                if (rule.Parameters.ContainsKey("MinLength"))
                {
                    var minLength = Convert.ToInt32(rule.Parameters["MinLength"]);
                    if (stringValue.Length < minLength)
                    {
                        return new ValidationError
                        {
                            RecordIndex = recordIndex,
                            FieldName = fieldName,
                            RuleName = rule.Name,
                            Message = $"Minimum length is {minLength} characters",
                            Severity = rule.Severity,
                            ActualValue = stringValue.Length,
                            ExpectedValue = minLength
                        };
                    }
                }

                if (rule.Parameters.ContainsKey("MaxLength"))
                {
                    var maxLength = Convert.ToInt32(rule.Parameters["MaxLength"]);
                    if (stringValue.Length > maxLength)
                    {
                        return new ValidationError
                        {
                            RecordIndex = recordIndex,
                            FieldName = fieldName,
                            RuleName = rule.Name,
                            Message = $"Maximum length is {maxLength} characters",
                            Severity = rule.Severity,
                            ActualValue = stringValue.Length,
                            ExpectedValue = maxLength
                        };
                    }
                }
                break;

            case "email":
                if (!Regex.IsMatch(stringValue, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return new ValidationError
                    {
                        RecordIndex = recordIndex,
                        FieldName = fieldName,
                        RuleName = rule.Name,
                        Message = "Invalid email format",
                        Severity = rule.Severity,
                        ActualValue = value
                    };
                }
                break;

            case "numeric":
                if (!double.TryParse(stringValue, out _))
                {
                    return new ValidationError
                    {
                        RecordIndex = recordIndex,
                        FieldName = fieldName,
                        RuleName = rule.Name,
                        Message = "Value must be numeric",
                        Severity = rule.Severity,
                        ActualValue = value
                    };
                }
                break;

            case "date":
                if (!DateTime.TryParse(stringValue, out _))
                {
                    return new ValidationError
                    {
                        RecordIndex = recordIndex,
                        FieldName = fieldName,
                        RuleName = rule.Name,
                        Message = "Invalid date format",
                        Severity = rule.Severity,
                        ActualValue = value
                    };
                }
                break;
        }

        return null;
    }

    private FieldQualityMetrics AnalyzeField(
        List<Dictionary<string, object>> data,
        string fieldName)
    {
        var metrics = new FieldQualityMetrics
        {
            FieldName = fieldName,
            TotalValues = data.Count
        };

        var values = data
            .Where(row => row.ContainsKey(fieldName))
            .Select(row => row[fieldName]?.ToString() ?? "")
            .ToList();

        metrics.NullValues = data.Count - values.Count(v => !string.IsNullOrWhiteSpace(v));
        metrics.UniqueValues = values.Distinct().Count();
        metrics.DuplicateValues = values.Count - metrics.UniqueValues;

        metrics.CompletenessPercentage = data.Count > 0
            ? ((double)(data.Count - metrics.NullValues) / data.Count) * 100
            : 0;

        metrics.UniquenessPercentage = values.Count > 0
            ? ((double)metrics.UniqueValues / values.Count) * 100
            : 0;

        return metrics;
    }
}
