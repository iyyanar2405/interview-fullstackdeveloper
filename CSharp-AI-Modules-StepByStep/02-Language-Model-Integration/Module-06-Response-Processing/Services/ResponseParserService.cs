using AI.ResponseProcessing.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Markdig;
using HtmlAgilityPack;

namespace AI.ResponseProcessing.Services;

public interface IResponseParserService
{
    Task<ParsedResponse<T>> ParseAsync<T>(ParsingRequest request);
    Task<ParsedResponse<JObject>> ParseJsonAsync(string content, ParsingOptions? options = null);
    Task<ParsedResponse<XDocument>> ParseXmlAsync(string content, ParsingOptions? options = null);
    Task<ParsedResponse<string>> ParseMarkdownAsync(string content, ParsingOptions? options = null);
    Task<List<CodeBlock>> ExtractCodeBlocksAsync(string content);
    Task<List<ExtractedLink>> ExtractLinksAsync(string content);
    Task<List<ExtractedTable>> ExtractTablesAsync(string markdown);
    Task<StructuredData> ExtractStructuredDataAsync(string content, ResponseFormat format);
}

public class ResponseParserService : IResponseParserService
{
    private readonly ILogger<ResponseParserService> _logger;
    private readonly IDeserializer _yamlDeserializer;
    private readonly MarkdownPipeline _markdownPipeline;

    public ResponseParserService(ILogger<ResponseParserService> logger)
    {
        _logger = logger;
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        _markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
    }

    public async Task<ParsedResponse<T>> ParseAsync<T>(ParsingRequest request)
    {
        try
        {
            var content = request.Options.TrimWhitespace 
                ? request.Content.Trim() 
                : request.Content;

            return request.Format switch
            {
                ResponseFormat.Json => await ParseJsonInternalAsync<T>(content, request.Options),
                ResponseFormat.Xml => await ParseXmlInternalAsync<T>(content, request.Options),
                ResponseFormat.Yaml => await ParseYamlAsync<T>(content, request.Options),
                ResponseFormat.Markdown => await ParseMarkdownInternalAsync<T>(content, request.Options),
                ResponseFormat.PlainText => await ParsePlainTextAsync<T>(content, request.Options),
                _ => throw new NotSupportedException($"Format {request.Format} is not supported")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing content in format {Format}", request.Format);
            return new ParsedResponse<T>
            {
                Success = false,
                OriginalFormat = request.Format,
                Errors = new List<ParsingError>
                {
                    new ParsingError
                    {
                        Message = ex.Message,
                        Severity = ValidationSeverity.Error,
                        Code = ex.GetType().Name
                    }
                }
            };
        }
    }

    public async Task<ParsedResponse<JObject>> ParseJsonAsync(string content, ParsingOptions? options = null)
    {
        options ??= new ParsingOptions();

        try
        {
            var cleanContent = CleanContent(content, options);
            
            // Try to extract JSON if embedded in text
            var jsonMatch = Regex.Match(cleanContent, @"\{[\s\S]*\}|\[[\s\S]*\]");
            if (jsonMatch.Success && !cleanContent.TrimStart().StartsWith("{") && !cleanContent.TrimStart().StartsWith("["))
            {
                cleanContent = jsonMatch.Value;
            }

            var jObject = JObject.Parse(cleanContent);

            return await Task.FromResult(new ParsedResponse<JObject>
            {
                Data = jObject,
                Success = true,
                OriginalFormat = ResponseFormat.Json,
                ExtractedMetadata = ExtractJsonMetadata(jObject)
            });
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "JSON parsing failed");
            return new ParsedResponse<JObject>
            {
                Success = false,
                OriginalFormat = ResponseFormat.Json,
                Errors = new List<ParsingError>
                {
                    new ParsingError
                    {
                        Message = ex.Message,
                        Location = $"Line {ex.LineNumber}, Position {ex.LinePosition}",
                        Severity = ValidationSeverity.Error,
                        Code = "JSON_PARSE_ERROR"
                    }
                }
            };
        }
    }

    public async Task<ParsedResponse<XDocument>> ParseXmlAsync(string content, ParsingOptions? options = null)
    {
        options ??= new ParsingOptions();

        try
        {
            var cleanContent = CleanContent(content, options);
            var doc = XDocument.Parse(cleanContent);

            return await Task.FromResult(new ParsedResponse<XDocument>
            {
                Data = doc,
                Success = true,
                OriginalFormat = ResponseFormat.Xml,
                ExtractedMetadata = ExtractXmlMetadata(doc)
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "XML parsing failed");
            return new ParsedResponse<XDocument>
            {
                Success = false,
                OriginalFormat = ResponseFormat.Xml,
                Errors = new List<ParsingError>
                {
                    new ParsingError
                    {
                        Message = ex.Message,
                        Severity = ValidationSeverity.Error,
                        Code = "XML_PARSE_ERROR"
                    }
                }
            };
        }
    }

    public async Task<ParsedResponse<string>> ParseMarkdownAsync(string content, ParsingOptions? options = null)
    {
        options ??= new ParsingOptions();

        try
        {
            var cleanContent = CleanContent(content, options);
            var html = Markdown.ToHtml(cleanContent, _markdownPipeline);

            var metadata = new Dictionary<string, object>();
            
            if (options.ExtractCodeBlocks)
            {
                var codeBlocks = await ExtractCodeBlocksAsync(cleanContent);
                metadata["codeBlocks"] = codeBlocks;
            }

            if (options.ExtractLinks)
            {
                var links = await ExtractLinksAsync(cleanContent);
                metadata["links"] = links;
            }

            if (options.ExtractTables)
            {
                var tables = await ExtractTablesAsync(cleanContent);
                metadata["tables"] = tables;
            }

            return new ParsedResponse<string>
            {
                Data = html,
                Success = true,
                OriginalFormat = ResponseFormat.Markdown,
                ExtractedMetadata = metadata
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Markdown parsing failed");
            return new ParsedResponse<string>
            {
                Success = false,
                OriginalFormat = ResponseFormat.Markdown,
                Errors = new List<ParsingError>
                {
                    new ParsingError
                    {
                        Message = ex.Message,
                        Severity = ValidationSeverity.Error,
                        Code = "MARKDOWN_PARSE_ERROR"
                    }
                }
            };
        }
    }

    public async Task<List<CodeBlock>> ExtractCodeBlocksAsync(string content)
    {
        var codeBlocks = new List<CodeBlock>();
        var pattern = @"```(\w+)?\n([\s\S]*?)```";
        var matches = Regex.Matches(content, pattern);

        var lineNumber = 1;
        foreach (Match match in matches)
        {
            var language = match.Groups[1].Value;
            var code = match.Groups[2].Value.Trim();

            codeBlocks.Add(new CodeBlock
            {
                Language = string.IsNullOrWhiteSpace(language) ? "text" : language,
                Code = code,
                LineNumber = lineNumber
            });

            lineNumber += code.Split('\n').Length;
        }

        return await Task.FromResult(codeBlocks);
    }

    public async Task<List<ExtractedLink>> ExtractLinksAsync(string content)
    {
        var links = new List<ExtractedLink>();
        
        // Markdown links: [text](url "title")
        var markdownPattern = @"\[([^\]]+)\]\(([^\)]+?)(?:\s+""([^""]+)"")?\)";
        var matches = Regex.Matches(content, markdownPattern);

        foreach (Match match in matches)
        {
            links.Add(new ExtractedLink
            {
                Text = match.Groups[1].Value,
                Url = match.Groups[2].Value,
                Title = match.Groups.Count > 3 ? match.Groups[3].Value : null
            });
        }

        // Plain URLs
        var urlPattern = @"https?://[^\s<>""]+";
        var urlMatches = Regex.Matches(content, urlPattern);
        
        foreach (Match match in urlMatches)
        {
            if (!links.Any(l => l.Url == match.Value))
            {
                links.Add(new ExtractedLink
                {
                    Text = match.Value,
                    Url = match.Value
                });
            }
        }

        return await Task.FromResult(links);
    }

    public async Task<List<ExtractedTable>> ExtractTablesAsync(string markdown)
    {
        var tables = new List<ExtractedTable>();
        var lines = markdown.Split('\n');

        for (int i = 0; i < lines.Length - 1; i++)
        {
            var line = lines[i].Trim();
            if (line.StartsWith("|") && lines[i + 1].Trim().Contains("---"))
            {
                var table = new ExtractedTable();
                
                // Parse headers
                table.Headers = line.Split('|')
                    .Skip(1)
                    .Select(h => h.Trim())
                    .Where(h => !string.IsNullOrWhiteSpace(h))
                    .ToList();

                // Skip separator line
                i += 2;

                // Parse rows
                while (i < lines.Length && lines[i].Trim().StartsWith("|"))
                {
                    var row = lines[i].Split('|')
                        .Skip(1)
                        .Select(c => c.Trim())
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .ToList();

                    if (row.Any())
                        table.Rows.Add(row);

                    i++;
                }

                tables.Add(table);
            }
        }

        return await Task.FromResult(tables);
    }

    public async Task<StructuredData> ExtractStructuredDataAsync(string content, ResponseFormat format)
    {
        var structuredData = new StructuredData
        {
            Type = format.ToString()
        };

        try
        {
            switch (format)
            {
                case ResponseFormat.Json:
                    var jsonResult = await ParseJsonAsync(content);
                    if (jsonResult.Success && jsonResult.Data != null)
                    {
                        structuredData.Fields = jsonResult.Data.ToObject<Dictionary<string, object>>() 
                            ?? new Dictionary<string, object>();
                    }
                    break;

                case ResponseFormat.Xml:
                    var xmlResult = await ParseXmlAsync(content);
                    if (xmlResult.Success && xmlResult.Data != null)
                    {
                        structuredData.Fields = ConvertXmlToFields(xmlResult.Data.Root);
                    }
                    break;

                case ResponseFormat.Markdown:
                    var codeBlocks = await ExtractCodeBlocksAsync(content);
                    var links = await ExtractLinksAsync(content);
                    var tables = await ExtractTablesAsync(content);

                    structuredData.Fields["codeBlocks"] = codeBlocks;
                    structuredData.Fields["links"] = links;
                    structuredData.Fields["tables"] = tables;
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting structured data from {Format}", format);
        }

        return structuredData;
    }

    #region Private Helper Methods

    private async Task<ParsedResponse<T>> ParseJsonInternalAsync<T>(string content, ParsingOptions options)
    {
        try
        {
            var cleanContent = CleanContent(content, options);
            var data = JsonConvert.DeserializeObject<T>(cleanContent);

            return await Task.FromResult(new ParsedResponse<T>
            {
                Data = data,
                Success = true,
                OriginalFormat = ResponseFormat.Json
            });
        }
        catch (Exception ex)
        {
            return new ParsedResponse<T>
            {
                Success = false,
                OriginalFormat = ResponseFormat.Json,
                Errors = new List<ParsingError>
                {
                    new ParsingError
                    {
                        Message = ex.Message,
                        Severity = ValidationSeverity.Error
                    }
                }
            };
        }
    }

    private async Task<ParsedResponse<T>> ParseXmlInternalAsync<T>(string content, ParsingOptions options)
    {
        try
        {
            var cleanContent = CleanContent(content, options);
            var doc = XDocument.Parse(cleanContent);
            
            // Convert XML to JSON then to T
            var json = JsonConvert.SerializeXNode(doc, Newtonsoft.Json.Formatting.None, true);
            var data = JsonConvert.DeserializeObject<T>(json);

            return await Task.FromResult(new ParsedResponse<T>
            {
                Data = data,
                Success = true,
                OriginalFormat = ResponseFormat.Xml
            });
        }
        catch (Exception ex)
        {
            return new ParsedResponse<T>
            {
                Success = false,
                OriginalFormat = ResponseFormat.Xml,
                Errors = new List<ParsingError>
                {
                    new ParsingError
                    {
                        Message = ex.Message,
                        Severity = ValidationSeverity.Error
                    }
                }
            };
        }
    }

    private async Task<ParsedResponse<T>> ParseYamlAsync<T>(string content, ParsingOptions options)
    {
        try
        {
            var cleanContent = CleanContent(content, options);
            var data = _yamlDeserializer.Deserialize<T>(cleanContent);

            return await Task.FromResult(new ParsedResponse<T>
            {
                Data = data,
                Success = true,
                OriginalFormat = ResponseFormat.Yaml
            });
        }
        catch (Exception ex)
        {
            return new ParsedResponse<T>
            {
                Success = false,
                OriginalFormat = ResponseFormat.Yaml,
                Errors = new List<ParsingError>
                {
                    new ParsingError
                    {
                        Message = ex.Message,
                        Severity = ValidationSeverity.Error
                    }
                }
            };
        }
    }

    private async Task<ParsedResponse<T>> ParseMarkdownInternalAsync<T>(string content, ParsingOptions options)
    {
        try
        {
            var cleanContent = CleanContent(content, options);
            var html = Markdown.ToHtml(cleanContent, _markdownPipeline);
            
            var data = (T)(object)html;

            return await Task.FromResult(new ParsedResponse<T>
            {
                Data = data,
                Success = true,
                OriginalFormat = ResponseFormat.Markdown
            });
        }
        catch (Exception ex)
        {
            return new ParsedResponse<T>
            {
                Success = false,
                OriginalFormat = ResponseFormat.Markdown,
                Errors = new List<ParsingError>
                {
                    new ParsingError
                    {
                        Message = ex.Message,
                        Severity = ValidationSeverity.Error
                    }
                }
            };
        }
    }

    private async Task<ParsedResponse<T>> ParsePlainTextAsync<T>(string content, ParsingOptions options)
    {
        try
        {
            var cleanContent = CleanContent(content, options);
            var data = (T)(object)cleanContent;

            return await Task.FromResult(new ParsedResponse<T>
            {
                Data = data,
                Success = true,
                OriginalFormat = ResponseFormat.PlainText
            });
        }
        catch (Exception ex)
        {
            return new ParsedResponse<T>
            {
                Success = false,
                OriginalFormat = ResponseFormat.PlainText,
                Errors = new List<ParsingError>
                {
                    new ParsingError
                    {
                        Message = ex.Message,
                        Severity = ValidationSeverity.Error
                    }
                }
            };
        }
    }

    private string CleanContent(string content, ParsingOptions options)
    {
        if (options.TrimWhitespace)
            content = content.Trim();

        if (options.RemoveComments)
        {
            // Remove // comments
            content = Regex.Replace(content, @"//.*$", "", RegexOptions.Multiline);
            // Remove /* */ comments
            content = Regex.Replace(content, @"/\*[\s\S]*?\*/", "");
        }

        return content;
    }

    private Dictionary<string, object> ExtractJsonMetadata(JObject jObject)
    {
        var metadata = new Dictionary<string, object>
        {
            ["propertyCount"] = jObject.Properties().Count(),
            ["depth"] = CalculateJsonDepth(jObject),
            ["hasArrays"] = jObject.Descendants().OfType<JArray>().Any()
        };

        return metadata;
    }

    private int CalculateJsonDepth(JToken token, int currentDepth = 0)
    {
        if (token is JObject obj)
        {
            return obj.Properties()
                .Select(p => CalculateJsonDepth(p.Value, currentDepth + 1))
                .DefaultIfEmpty(currentDepth)
                .Max();
        }
        else if (token is JArray array)
        {
            return array
                .Select(item => CalculateJsonDepth(item, currentDepth + 1))
                .DefaultIfEmpty(currentDepth)
                .Max();
        }

        return currentDepth;
    }

    private Dictionary<string, object> ExtractXmlMetadata(XDocument doc)
    {
        var metadata = new Dictionary<string, object>
        {
            ["rootElement"] = doc.Root?.Name.LocalName ?? "unknown",
            ["elementCount"] = doc.Descendants().Count(),
            ["attributeCount"] = doc.Descendants().Attributes().Count()
        };

        return metadata;
    }

    private Dictionary<string, object> ConvertXmlToFields(XElement? element)
    {
        var fields = new Dictionary<string, object>();

        if (element == null)
            return fields;

        foreach (var attr in element.Attributes())
        {
            fields[attr.Name.LocalName] = attr.Value;
        }

        foreach (var child in element.Elements())
        {
            fields[child.Name.LocalName] = ConvertXmlToFields(child);
        }

        if (!element.HasElements && !string.IsNullOrWhiteSpace(element.Value))
        {
            fields["_value"] = element.Value;
        }

        return fields;
    }

    #endregion
}
