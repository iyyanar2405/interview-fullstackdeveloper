using System.Collections.Concurrent;
using Module_04_Tool_Integration.Models;
using Microsoft.Extensions.Options;

namespace Module_04_Tool_Integration.Services;

public sealed class ToolCatalogService
{
    private readonly ConcurrentDictionary<string, ApiToolDefinition> _apiTools = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, DatabaseToolDefinition> _databaseTools = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, FileSystemToolDefinition> _fileSystemTools = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, WebScrapingToolDefinition> _webScrapingTools = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, CustomToolDefinition> _customTools = new(StringComparer.OrdinalIgnoreCase);

    public ToolCatalogService(IOptions<ToolIntegrationOptions> options)
    {
        foreach (var tool in options.Value.ApiTools)
        {
            _apiTools[tool.ToolId] = Clone(tool);
        }

        foreach (var tool in options.Value.DatabaseTools)
        {
            _databaseTools[tool.ToolId] = Clone(tool);
        }

        foreach (var tool in options.Value.FileSystemTools)
        {
            _fileSystemTools[tool.ToolId] = Clone(tool);
        }

        foreach (var tool in options.Value.WebScrapingTools)
        {
            _webScrapingTools[tool.ToolId] = Clone(tool);
        }

        foreach (var tool in options.Value.CustomTools)
        {
            _customTools[tool.ToolId] = Clone(tool);
        }
    }

    public IReadOnlyCollection<ToolDescriptor> GetTools()
    {
        var tools = new List<ToolDescriptor>();
        tools.AddRange(_apiTools.Values.Select(ToDescriptor));
        tools.AddRange(_databaseTools.Values.Select(ToDescriptor));
        tools.AddRange(_fileSystemTools.Values.Select(ToDescriptor));
        tools.AddRange(_webScrapingTools.Values.Select(ToDescriptor));
        tools.AddRange(_customTools.Values.Select(ToDescriptor));
        return tools;
    }

    public ToolDescriptor? GetToolDescriptor(string toolType, string toolId)
    {
        if (!ToolKinds.TryNormalize(toolType, out var normalized))
        {
            return null;
        }

        return normalized switch
        {
            ToolKinds.Api => _apiTools.TryGetValue(toolId, out var api) ? ToDescriptor(api) : null,
            ToolKinds.Database => _databaseTools.TryGetValue(toolId, out var db) ? ToDescriptor(db) : null,
            ToolKinds.FileSystem => _fileSystemTools.TryGetValue(toolId, out var fs) ? ToDescriptor(fs) : null,
            ToolKinds.WebScraping => _webScrapingTools.TryGetValue(toolId, out var ws) ? ToDescriptor(ws) : null,
            ToolKinds.Custom => _customTools.TryGetValue(toolId, out var custom) ? ToDescriptor(custom) : null,
            _ => null
        };
    }

    public ApiToolDefinition? GetApiTool(string toolId)
    {
        return _apiTools.TryGetValue(toolId, out var tool) ? Clone(tool) : null;
    }

    public DatabaseToolDefinition? GetDatabaseTool(string toolId)
    {
        return _databaseTools.TryGetValue(toolId, out var tool) ? Clone(tool) : null;
    }

    public FileSystemToolDefinition? GetFileSystemTool(string toolId)
    {
        return _fileSystemTools.TryGetValue(toolId, out var tool) ? Clone(tool) : null;
    }

    public WebScrapingToolDefinition? GetWebScrapingTool(string toolId)
    {
        return _webScrapingTools.TryGetValue(toolId, out var tool) ? Clone(tool) : null;
    }

    public CustomToolDefinition? GetCustomTool(string toolId)
    {
        return _customTools.TryGetValue(toolId, out var tool) ? Clone(tool) : null;
    }

    private static ToolDescriptor ToDescriptor(ApiToolDefinition tool)
    {
        return new ToolDescriptor
        {
            ToolId = tool.ToolId,
            ToolType = ToolKinds.Api,
            Name = tool.Name,
            Summary = tool.Description,
            Capabilities = tool.SupportedOperations,
            Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["method"] = tool.Method,
                ["baseUrl"] = tool.BaseUrl
            }
        };
    }

    private static ToolDescriptor ToDescriptor(DatabaseToolDefinition tool)
    {
        return new ToolDescriptor
        {
            ToolId = tool.ToolId,
            ToolType = ToolKinds.Database,
            Name = tool.Name,
            Summary = tool.Description,
            Capabilities = tool.SupportedOperations,
            Metadata = new Dictionary<string, string>
            {
                ["engine"] = tool.Engine,
                ["tables"] = tool.Tables.Count.ToString()
            }
        };
    }

    private static ToolDescriptor ToDescriptor(FileSystemToolDefinition tool)
    {
        return new ToolDescriptor
        {
            ToolId = tool.ToolId,
            ToolType = ToolKinds.FileSystem,
            Name = tool.Name,
            Summary = tool.Description,
            Capabilities = tool.Capabilities,
            Metadata = new Dictionary<string, string>
            {
                ["rootPath"] = tool.RootPath,
                ["seedFiles"] = tool.SeedFiles.Count.ToString()
            }
        };
    }

    private static ToolDescriptor ToDescriptor(WebScrapingToolDefinition tool)
    {
        return new ToolDescriptor
        {
            ToolId = tool.ToolId,
            ToolType = ToolKinds.WebScraping,
            Name = tool.Name,
            Summary = tool.Description,
            Capabilities = tool.Selectors.Keys.ToArray(),
            Metadata = new Dictionary<string, string>
            {
                ["targetUrl"] = tool.TargetUrl,
                ["samples"] = tool.Samples.Count.ToString()
            }
        };
    }

    private static ToolDescriptor ToDescriptor(CustomToolDefinition tool)
    {
        return new ToolDescriptor
        {
            ToolId = tool.ToolId,
            ToolType = ToolKinds.Custom,
            Name = tool.Name,
            Summary = tool.Description,
            Capabilities = tool.Parameters,
            Metadata = new Dictionary<string, string>
            {
                ["behaviors"] = tool.Behaviors.Count.ToString()
            }
        };
    }

    private static ApiToolDefinition Clone(ApiToolDefinition tool)
    {
        return new ApiToolDefinition
        {
            ToolId = tool.ToolId,
            Name = tool.Name,
            Description = tool.Description,
            BaseUrl = tool.BaseUrl,
            Method = tool.Method,
            SupportedOperations = tool.SupportedOperations.ToList(),
            DefaultHeaders = new Dictionary<string, string>(tool.DefaultHeaders, StringComparer.OrdinalIgnoreCase),
            SampleParameters = new Dictionary<string, string>(tool.SampleParameters, StringComparer.OrdinalIgnoreCase),
            ResponseTemplates = tool.ResponseTemplates.Select(template => new ApiResponseTemplate
            {
                Operation = template.Operation,
                Narrative = template.Narrative,
                Outputs = new Dictionary<string, string>(template.Outputs, StringComparer.OrdinalIgnoreCase)
            }).ToList()
        };
    }

    private static DatabaseToolDefinition Clone(DatabaseToolDefinition tool)
    {
        return new DatabaseToolDefinition
        {
            ToolId = tool.ToolId,
            Name = tool.Name,
            Description = tool.Description,
            Engine = tool.Engine,
            SupportedOperations = tool.SupportedOperations.ToList(),
            Tables = tool.Tables.Select(table => new DatabaseTableDefinition
            {
                TableName = table.TableName,
                Rows = table.Rows.Select(row => new Dictionary<string, string>(row, StringComparer.OrdinalIgnoreCase)).ToList()
            }).ToList()
        };
    }

    private static FileSystemToolDefinition Clone(FileSystemToolDefinition tool)
    {
        return new FileSystemToolDefinition
        {
            ToolId = tool.ToolId,
            Name = tool.Name,
            Description = tool.Description,
            RootPath = tool.RootPath,
            Capabilities = tool.Capabilities.ToList(),
            SeedFiles = tool.SeedFiles.Select(seed => new FileSeedDefinition
            {
                Path = seed.Path,
                Content = seed.Content
            }).ToList()
        };
    }

    private static WebScrapingToolDefinition Clone(WebScrapingToolDefinition tool)
    {
        return new WebScrapingToolDefinition
        {
            ToolId = tool.ToolId,
            Name = tool.Name,
            Description = tool.Description,
            TargetUrl = tool.TargetUrl,
            Selectors = new Dictionary<string, string>(tool.Selectors, StringComparer.OrdinalIgnoreCase),
            Samples = tool.Samples.Select(sample => new ScrapedSampleRecord
            {
                Selector = sample.Selector,
                Content = sample.Content,
                CapturedAt = sample.CapturedAt
            }).ToList()
        };
    }

    private static CustomToolDefinition Clone(CustomToolDefinition tool)
    {
        return new CustomToolDefinition
        {
            ToolId = tool.ToolId,
            Name = tool.Name,
            Description = tool.Description,
            Parameters = tool.Parameters.ToList(),
            Behaviors = new Dictionary<string, string>(tool.Behaviors, StringComparer.OrdinalIgnoreCase),
            DefaultOutputs = new Dictionary<string, string>(tool.DefaultOutputs, StringComparer.OrdinalIgnoreCase)
        };
    }
}
