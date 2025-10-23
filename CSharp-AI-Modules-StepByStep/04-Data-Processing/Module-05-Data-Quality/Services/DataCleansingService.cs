using AI.DataQuality.Models;
using System.Text.RegularExpressions;

namespace AI.DataQuality.Services;

public interface IDataCleansingService
{
    Task<DataCleansingResult> CleanseDataAsync(DataCleansingRequest request);
    Task<List<Dictionary<string, object>>> RemoveDuplicatesAsync(List<Dictionary<string, object>> data);
    Task<List<Dictionary<string, object>>> FillMissingValuesAsync(List<Dictionary<string, object>> data, Dictionary<string, object> defaultValues);
    Task<List<Dictionary<string, object>>> StandardizeDataAsync(List<Dictionary<string, object>> data, List<CleansingRule> rules);
}

public class DataCleansingService : IDataCleansingService
{
    private readonly ILogger<DataCleansingService> _logger;

    public DataCleansingService(ILogger<DataCleansingService> logger)
    {
        _logger = logger;
    }

    public async Task<DataCleansingResult> CleanseDataAsync(DataCleansingRequest request)
    {
        var result = new DataCleansingResult
        {
            OriginalRecordCount = request.Data.Count,
            CleanedData = new List<Dictionary<string, object>>(request.Data)
        };

        try
        {
            // Remove duplicates if requested
            if (request.RemoveDuplicates)
            {
                var beforeCount = result.CleanedData.Count;
                result.CleanedData = await RemoveDuplicatesAsync(result.CleanedData);
                var removed = beforeCount - result.CleanedData.Count;
                
                if (removed > 0)
                {
                    result.RecordsRemoved += removed;
                    result.ActionsApplied.Add(new CleansingAction
                    {
                        RuleId = "RemoveDuplicates",
                        Operation = CleansingOperation.RemoveDuplicates,
                        RecordsAffected = removed
                    });
                }
            }

            // Fill missing values if requested
            if (request.FillMissingValues && request.DefaultValues.Count > 0)
            {
                var beforeModified = result.RecordsModified;
                result.CleanedData = await FillMissingValuesAsync(result.CleanedData, request.DefaultValues);
                result.ActionsApplied.Add(new CleansingAction
                {
                    RuleId = "FillMissing",
                    Operation = CleansingOperation.FillNull,
                    RecordsAffected = result.RecordsModified - beforeModified
                });
            }

            // Apply cleansing rules
            if (request.Rules.Count > 0)
            {
                result.CleanedData = await StandardizeDataAsync(result.CleanedData, request.Rules);
                
                foreach (var rule in request.Rules)
                {
                    result.ActionsApplied.Add(new CleansingAction
                    {
                        RuleId = rule.RuleId,
                        ColumnName = rule.ColumnName,
                        Operation = rule.Operation,
                        RecordsAffected = result.CleanedData.Count
                    });
                }
            }

            result.CleanedRecordCount = result.CleanedData.Count;

            _logger.LogInformation($"Data cleansing completed: {result.OriginalRecordCount} -> {result.CleanedRecordCount} records");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleansing data");
        }

        return result;
    }

    public async Task<List<Dictionary<string, object>>> RemoveDuplicatesAsync(List<Dictionary<string, object>> data)
    {
        try
        {
            var uniqueData = new List<Dictionary<string, object>>();
            var seenHashes = new HashSet<string>();

            foreach (var row in data)
            {
                var hash = string.Join("|", row.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key}:{kvp.Value}"));

                if (!seenHashes.Contains(hash))
                {
                    seenHashes.Add(hash);
                    uniqueData.Add(row);
                }
            }

            var duplicatesRemoved = data.Count - uniqueData.Count;
            if (duplicatesRemoved > 0)
            {
                _logger.LogInformation($"Removed {duplicatesRemoved} duplicate records");
            }

            return uniqueData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing duplicates");
            return data;
        }
    }

    public async Task<List<Dictionary<string, object>>> FillMissingValuesAsync(
        List<Dictionary<string, object>> data,
        Dictionary<string, object> defaultValues)
    {
        try
        {
            var filledData = new List<Dictionary<string, object>>();

            foreach (var row in data)
            {
                var filledRow = new Dictionary<string, object>(row);

                foreach (var defaultValue in defaultValues)
                {
                    if (!filledRow.ContainsKey(defaultValue.Key) || filledRow[defaultValue.Key] == null)
                    {
                        filledRow[defaultValue.Key] = defaultValue.Value;
                    }
                }

                filledData.Add(filledRow);
            }

            _logger.LogInformation($"Filled missing values with defaults for {defaultValues.Count} columns");

            return filledData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filling missing values");
            return data;
        }
    }

    public async Task<List<Dictionary<string, object>>> StandardizeDataAsync(
        List<Dictionary<string, object>> data,
        List<CleansingRule> rules)
    {
        try
        {
            var cleanedData = new List<Dictionary<string, object>>();

            foreach (var row in data)
            {
                var cleanedRow = new Dictionary<string, object>(row);

                foreach (var rule in rules)
                {
                    if (cleanedRow.ContainsKey(rule.ColumnName))
                    {
                        var value = cleanedRow[rule.ColumnName];
                        cleanedRow[rule.ColumnName] = ApplyCleansingOperation(value, rule.Operation, rule.Parameter);
                    }
                }

                cleanedData.Add(cleanedRow);
            }

            _logger.LogInformation($"Applied {rules.Count} cleansing rules to {data.Count} records");

            return cleanedData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error standardizing data");
            return data;
        }
    }

    private object ApplyCleansingOperation(object? value, CleansingOperation operation, object? parameter)
    {
        if (value == null)
            return parameter ?? value!;

        var stringValue = value.ToString() ?? "";

        try
        {
            return operation switch
            {
                CleansingOperation.Trim => stringValue.Trim(),
                CleansingOperation.UpperCase => stringValue.ToUpperInvariant(),
                CleansingOperation.LowerCase => stringValue.ToLowerInvariant(),
                CleansingOperation.RemoveSpecialCharacters => Regex.Replace(stringValue, @"[^a-zA-Z0-9\s]", ""),
                CleansingOperation.ReplaceValue => parameter ?? value,
                CleansingOperation.FillNull => value ?? parameter ?? value,
                CleansingOperation.FormatDate => FormatDate(value),
                CleansingOperation.Round => RoundNumeric(value, parameter),
                CleansingOperation.Standardize => StandardizeString(stringValue),
                _ => value
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error applying operation {operation} to value {value}");
            return value;
        }
    }

    private object FormatDate(object value)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        if (DateTime.TryParse(value.ToString(), out var parsedDate))
        {
            return parsedDate.ToString("yyyy-MM-dd");
        }

        return value;
    }

    private object RoundNumeric(object value, object? parameter)
    {
        var decimals = parameter != null ? Convert.ToInt32(parameter) : 2;

        if (value is double doubleValue)
        {
            return Math.Round(doubleValue, decimals);
        }

        if (value is decimal decimalValue)
        {
            return Math.Round(decimalValue, decimals);
        }

        if (double.TryParse(value.ToString(), out var parsed))
        {
            return Math.Round(parsed, decimals);
        }

        return value;
    }

    private string StandardizeString(string value)
    {
        // Remove extra whitespace
        value = Regex.Replace(value, @"\s+", " ");
        
        // Trim
        value = value.Trim();
        
        // Title case for common words
        if (value.Length > 0)
        {
            value = char.ToUpper(value[0]) + value.Substring(1).ToLower();
        }

        return value;
    }
}

public interface IDataGovernanceService
{
    Task<ComplianceReport> CheckComplianceAsync(List<Dictionary<string, object>> data, DataGovernancePolicy policy);
    Task<DataGovernancePolicy> CreatePolicyAsync(string policyName, string description, List<DataQualityRule> rules);
    Task<List<DataGovernancePolicy>> GetActivePoliciesAsync();
}

public class DataGovernanceService : IDataGovernanceService
{
    private readonly ILogger<DataGovernanceService> _logger;
    private readonly IDataValidationService _validationService;
    private readonly List<DataGovernancePolicy> _policies = new();

    public DataGovernanceService(
        ILogger<DataGovernanceService> logger,
        IDataValidationService validationService)
    {
        _logger = logger;
        _validationService = validationService;
    }

    public async Task<ComplianceReport> CheckComplianceAsync(
        List<Dictionary<string, object>> data,
        DataGovernancePolicy policy)
    {
        var report = new ComplianceReport
        {
            DatasetName = "Dataset",
            PolicyId = policy.PolicyId
        };

        try
        {
            // Validate data against policy rules
            var validationResult = await _validationService.ValidateDataAsync(data, policy.Rules);

            // Calculate compliance score
            var totalRules = policy.Rules.Count(r => r.IsActive);
            var violatedRules = validationResult.Errors.Select(e => e.RuleId).Distinct().Count();
            
            report.ComplianceScore = totalRules > 0 
                ? (double)(totalRules - violatedRules) / totalRules * 100 
                : 100;

            report.IsCompliant = report.ComplianceScore >= 90; // 90% threshold

            // Create violations
            foreach (var error in validationResult.Errors)
            {
                var existingViolation = report.Violations.FirstOrDefault(v => v.RuleId == error.RuleId);

                if (existingViolation != null)
                {
                    existingViolation.AffectedRecords++;
                }
                else
                {
                    report.Violations.Add(new ComplianceViolation
                    {
                        RuleId = error.RuleId,
                        RuleName = error.RuleName,
                        Severity = error.Severity,
                        Description = error.Message,
                        AffectedRecords = 1
                    });
                }
            }

            report.Metadata["TotalRecords"] = data.Count;
            report.Metadata["ValidationErrors"] = validationResult.Errors.Count;

            _logger.LogInformation($"Compliance check completed: {report.ComplianceScore:F2}% compliant");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking compliance");
        }

        return report;
    }

    public async Task<DataGovernancePolicy> CreatePolicyAsync(
        string policyName,
        string description,
        List<DataQualityRule> rules)
    {
        var policy = new DataGovernancePolicy
        {
            PolicyName = policyName,
            Description = description,
            Rules = rules,
            Owner = "System"
        };

        _policies.Add(policy);
        _logger.LogInformation($"Created policy '{policyName}' with {rules.Count} rules");

        return await Task.FromResult(policy);
    }

    public async Task<List<DataGovernancePolicy>> GetActivePoliciesAsync()
    {
        return await Task.FromResult(_policies.Where(p => p.IsActive).ToList());
    }
}
