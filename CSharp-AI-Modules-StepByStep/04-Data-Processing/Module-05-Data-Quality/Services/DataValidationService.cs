using AI.DataQuality.Models;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AI.DataQuality.Services;

public interface IDataValidationService
{
    Task<ValidationResult> ValidateDataAsync(List<Dictionary<string, object>> data, List<DataQualityRule> rules);
    Task<ValidationResult> ValidateRecordAsync(Dictionary<string, object> record, List<DataQualityRule> rules);
    Task<SchemaValidationResult> ValidateSchemaAsync(List<Dictionary<string, object>> data, SchemaDefinition schema);
    Task<List<DataQualityRule>> GetStandardRulesAsync(string datasetType);
}

public class DataValidationService : IDataValidationService
{
    private readonly ILogger<DataValidationService> _logger;

    public DataValidationService(ILogger<DataValidationService> logger)
    {
        _logger = logger;
    }

    public async Task<ValidationResult> ValidateDataAsync(
        List<Dictionary<string, object>> data,
        List<DataQualityRule> rules)
    {
        var startTime = DateTime.UtcNow;
        var result = new ValidationResult { IsValid = true };

        try
        {
            var activeRules = rules.Where(r => r.IsActive).ToList();

            for (int i = 0; i < data.Count; i++)
            {
                var record = data[i];

                foreach (var rule in activeRules)
                {
                    var isValid = await EvaluateRuleAsync(record, rule, i + 1);

                    if (!isValid)
                    {
                        result.IsValid = false;
                        result.Errors.Add(new ValidationError
                        {
                            RuleId = rule.RuleId,
                            RuleName = rule.RuleName,
                            Severity = rule.Severity,
                            Message = rule.ErrorMessage,
                            RowNumber = i + 1
                        });
                    }
                }
            }

            result.ValidationTime = DateTime.UtcNow - startTime;
            result.Metadata["TotalRecords"] = data.Count;
            result.Metadata["RulesApplied"] = activeRules.Count;

            _logger.LogInformation($"Validated {data.Count} records with {activeRules.Count} rules");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating data");
            result.IsValid = false;
            result.Errors.Add(new ValidationError
            {
                Severity = ValidationSeverity.Critical,
                Message = $"Validation error: {ex.Message}"
            });
        }

        return result;
    }

    public async Task<ValidationResult> ValidateRecordAsync(
        Dictionary<string, object> record,
        List<DataQualityRule> rules)
    {
        var result = new ValidationResult { IsValid = true };

        try
        {
            var activeRules = rules.Where(r => r.IsActive).ToList();

            foreach (var rule in activeRules)
            {
                var isValid = await EvaluateRuleAsync(record, rule);

                if (!isValid)
                {
                    result.IsValid = false;
                    result.Errors.Add(new ValidationError
                    {
                        RuleId = rule.RuleId,
                        RuleName = rule.RuleName,
                        Severity = rule.Severity,
                        Message = rule.ErrorMessage
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating record");
            result.IsValid = false;
        }

        return result;
    }

    public async Task<SchemaValidationResult> ValidateSchemaAsync(
        List<Dictionary<string, object>> data,
        SchemaDefinition schema)
    {
        var result = new SchemaValidationResult { IsValid = true };

        try
        {
            for (int i = 0; i < data.Count; i++)
            {
                var record = data[i];

                // Validate required fields
                foreach (var requiredField in schema.RequiredFields)
                {
                    if (!record.ContainsKey(requiredField) || record[requiredField] == null)
                    {
                        result.IsValid = false;
                        result.Violations.Add(new SchemaViolation
                        {
                            FieldName = requiredField,
                            ViolationType = "RequiredFieldMissing",
                            Message = $"Required field '{requiredField}' is missing or null",
                            RowNumber = i + 1
                        });
                    }
                }

                // Validate field definitions
                foreach (var field in schema.Fields)
                {
                    if (record.ContainsKey(field.FieldName))
                    {
                        var value = record[field.FieldName];

                        // Type validation
                        if (!ValidateDataType(value, field.DataType))
                        {
                            result.IsValid = false;
                            result.Violations.Add(new SchemaViolation
                            {
                                FieldName = field.FieldName,
                                ViolationType = "DataTypeMismatch",
                                Message = $"Expected {field.DataType}, got {value?.GetType().Name}",
                                RowNumber = i + 1,
                                ActualValue = value
                            });
                        }

                        // Range validation
                        if (field.MinValue != null || field.MaxValue != null)
                        {
                            if (!ValidateRange(value, field.MinValue, field.MaxValue))
                            {
                                result.IsValid = false;
                                result.Violations.Add(new SchemaViolation
                                {
                                    FieldName = field.FieldName,
                                    ViolationType = "RangeViolation",
                                    Message = $"Value outside allowed range",
                                    RowNumber = i + 1,
                                    ActualValue = value
                                });
                            }
                        }

                        // Length validation for strings
                        if (value is string strValue)
                        {
                            if (field.MinLength.HasValue && strValue.Length < field.MinLength.Value)
                            {
                                result.IsValid = false;
                                result.Violations.Add(new SchemaViolation
                                {
                                    FieldName = field.FieldName,
                                    ViolationType = "LengthViolation",
                                    Message = $"String length {strValue.Length} is less than minimum {field.MinLength.Value}",
                                    RowNumber = i + 1
                                });
                            }

                            if (field.MaxLength.HasValue && strValue.Length > field.MaxLength.Value)
                            {
                                result.IsValid = false;
                                result.Violations.Add(new SchemaViolation
                                {
                                    FieldName = field.FieldName,
                                    ViolationType = "LengthViolation",
                                    Message = $"String length {strValue.Length} exceeds maximum {field.MaxLength.Value}",
                                    RowNumber = i + 1
                                });
                            }
                        }

                        // Pattern validation
                        if (!string.IsNullOrEmpty(field.Pattern) && value is string patternValue)
                        {
                            if (!Regex.IsMatch(patternValue, field.Pattern))
                            {
                                result.IsValid = false;
                                result.Violations.Add(new SchemaViolation
                                {
                                    FieldName = field.FieldName,
                                    ViolationType = "PatternViolation",
                                    Message = $"Value does not match pattern: {field.Pattern}",
                                    RowNumber = i + 1
                                });
                            }
                        }

                        // Allowed values validation
                        if (field.AllowedValues != null && field.AllowedValues.Count > 0)
                        {
                            if (!field.AllowedValues.Contains(value))
                            {
                                result.IsValid = false;
                                result.Violations.Add(new SchemaViolation
                                {
                                    FieldName = field.FieldName,
                                    ViolationType = "AllowedValuesViolation",
                                    Message = $"Value not in allowed list",
                                    RowNumber = i + 1,
                                    ActualValue = value
                                });
                            }
                        }
                    }
                }
            }

            result.Statistics["TotalRecords"] = data.Count;
            result.Statistics["ViolationCount"] = result.Violations.Count;

            _logger.LogInformation($"Schema validation completed: {result.Violations.Count} violations found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating schema");
            result.IsValid = false;
        }

        return await Task.FromResult(result);
    }

    public async Task<List<DataQualityRule>> GetStandardRulesAsync(string datasetType)
    {
        var rules = new List<DataQualityRule>();

        // Standard completeness rules
        rules.Add(new DataQualityRule
        {
            RuleName = "NoNullValues",
            Description = "Check for null values",
            Dimension = DataQualityDimension.Completeness,
            Severity = ValidationSeverity.Error,
            Expression = "value != null",
            ErrorMessage = "Value cannot be null"
        });

        // Standard accuracy rules
        rules.Add(new DataQualityRule
        {
            RuleName = "ValidEmail",
            Description = "Validate email format",
            Dimension = DataQualityDimension.Accuracy,
            Severity = ValidationSeverity.Error,
            Expression = "email_pattern",
            ErrorMessage = "Invalid email format"
        });

        // Standard consistency rules
        rules.Add(new DataQualityRule
        {
            RuleName = "DateConsistency",
            Description = "End date must be after start date",
            Dimension = DataQualityDimension.Consistency,
            Severity = ValidationSeverity.Error,
            Expression = "end_date > start_date",
            ErrorMessage = "End date must be after start date"
        });

        return await Task.FromResult(rules);
    }

    private async Task<bool> EvaluateRuleAsync(Dictionary<string, object> record, DataQualityRule rule, int? rowNumber = null)
    {
        try
        {
            // Simple expression evaluation
            if (rule.Expression.Contains("email_pattern"))
            {
                var emailField = record.FirstOrDefault(kvp => kvp.Key.ToLower().Contains("email"));
                if (emailField.Value != null)
                {
                    var email = emailField.Value.ToString();
                    return Regex.IsMatch(email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                }
            }

            if (rule.Expression == "value != null")
            {
                return record.Values.All(v => v != null);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error evaluating rule {rule.RuleName}");
            return false;
        }
    }

    private bool ValidateDataType(object? value, DataType expectedType)
    {
        if (value == null) return true;

        return expectedType switch
        {
            DataType.String => value is string,
            DataType.Integer => value is int or long,
            DataType.Decimal => value is decimal or double or float,
            DataType.Boolean => value is bool,
            DataType.DateTime => value is DateTime or DateTimeOffset,
            _ => true
        };
    }

    private bool ValidateRange(object? value, object? minValue, object? maxValue)
    {
        if (value == null) return true;

        try
        {
            if (value is IComparable comparable)
            {
                if (minValue != null && comparable.CompareTo(minValue) < 0)
                    return false;

                if (maxValue != null && comparable.CompareTo(maxValue) > 0)
                    return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
