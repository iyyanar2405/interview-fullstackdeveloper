using AI.DataFormats.Models;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AI.DataFormats.Services;

public interface IXmlService
{
    Task<DataFormatResponse<XmlParseResult>> ParseXmlAsync(string xml, XmlOptions? options = null);
    Task<DataFormatResponse<string>> SerializeToXmlAsync(object data, XmlOptions? options = null);
    Task<DataFormatResponse<XmlParseResult>> ReadXmlFileAsync(string filePath, XmlOptions? options = null);
    Task<DataFormatResponse<bool>> WriteXmlFileAsync(string filePath, object data, XmlOptions? options = null);
    Task<DataFormatResponse<T>> DeserializeAsync<T>(string xml);
}

public interface IHtmlService
{
    Task<DataFormatResponse<HtmlParseResult>> ParseHtmlAsync(string html, HtmlOptions? options = null);
    Task<DataFormatResponse<HtmlParseResult>> ReadHtmlFileAsync(string filePath, HtmlOptions? options = null);
    Task<DataFormatResponse<List<HtmlTable>>> ExtractTablesAsync(string html);
}

public class XmlService : IXmlService
{
    private readonly ILogger<XmlService> _logger;

    public XmlService(ILogger<XmlService> logger)
    {
        _logger = logger;
    }

    public async Task<DataFormatResponse<XmlParseResult>> ParseXmlAsync(string xml, XmlOptions? options = null)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<XmlParseResult>();
        var result = new XmlParseResult();

        try
        {
            options ??= new XmlOptions();

            var doc = XDocument.Parse(xml);
            result.RootElement = doc.Root?.Name.LocalName ?? string.Empty;

            // Convert to dictionary
            result.Data = XElementToDictionary(doc.Root);

            // Extract items if they exist
            var itemElements = doc.Descendants(options.ItemElement);
            foreach (var item in itemElements)
            {
                var itemDict = XElementToDictionary(item);
                result.Items.Add(itemDict);
            }

            result.ItemCount = result.Items.Count;

            response.Success = true;
            response.Data = result;
            response.RecordCount = result.ItemCount;
            response.SizeBytes = System.Text.Encoding.UTF8.GetByteCount(xml);

            _logger.LogInformation("Parsed XML: {Items} items", result.ItemCount);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error parsing XML");
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<string>> SerializeToXmlAsync(object data, XmlOptions? options = null)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<string>();

        try
        {
            options ??= new XmlOptions();

            var serializer = new XmlSerializer(data.GetType());
            var settings = new XmlWriterSettings
            {
                Indent = options.Pretty,
                OmitXmlDeclaration = options.OmitXmlDeclaration,
                Encoding = System.Text.Encoding.GetEncoding(options.Encoding)
            };

            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            serializer.Serialize(xmlWriter, data);
            var xml = stringWriter.ToString();

            response.Success = true;
            response.Data = xml;
            response.SizeBytes = System.Text.Encoding.UTF8.GetByteCount(xml);

            _logger.LogInformation("Serialized to XML: {Size} bytes", response.SizeBytes);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error serializing to XML");
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<XmlParseResult>> ReadXmlFileAsync(string filePath, XmlOptions? options = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<XmlParseResult>();

        try
        {
            var xml = await File.ReadAllTextAsync(filePath);
            var parseResult = await ParseXmlAsync(xml, options);

            response.Success = parseResult.Success;
            response.Data = parseResult.Data;
            response.Error = parseResult.Error;
            response.RecordCount = parseResult.RecordCount;
            response.SizeBytes = parseResult.SizeBytes;

            _logger.LogInformation("Read XML file: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error reading XML file: {FilePath}", filePath);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<bool>> WriteXmlFileAsync(
        string filePath,
        object data,
        XmlOptions? options = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<bool>();

        try
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var serializeResult = await SerializeToXmlAsync(data, options);

            if (!serializeResult.Success)
            {
                response.Success = false;
                response.Error = serializeResult.Error;
                return response;
            }

            await File.WriteAllTextAsync(filePath, serializeResult.Data);

            response.Success = true;
            response.Data = true;
            response.SizeBytes = serializeResult.SizeBytes;

            _logger.LogInformation("Wrote XML file: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error writing XML file: {FilePath}", filePath);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<T>> DeserializeAsync<T>(string xml)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<T>();

        try
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            var data = (T?)serializer.Deserialize(reader);

            response.Success = true;
            response.Data = data;

            _logger.LogInformation("Deserialized XML to {Type}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error deserializing XML to {Type}", typeof(T).Name);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    private Dictionary<string, object> XElementToDictionary(XElement? element)
    {
        var dict = new Dictionary<string, object>();

        if (element == null)
            return dict;

        // Add attributes
        foreach (var attr in element.Attributes())
        {
            dict[$"@{attr.Name.LocalName}"] = attr.Value;
        }

        // Add elements
        var groups = element.Elements().GroupBy(e => e.Name.LocalName);
        foreach (var group in groups)
        {
            var items = group.ToList();

            if (items.Count == 1)
            {
                var item = items[0];
                if (item.HasElements)
                {
                    dict[item.Name.LocalName] = XElementToDictionary(item);
                }
                else
                {
                    dict[item.Name.LocalName] = item.Value;
                }
            }
            else
            {
                dict[group.Key] = items.Select(XElementToDictionary).ToList();
            }
        }

        // Add text value if no child elements
        if (!element.HasElements && !string.IsNullOrWhiteSpace(element.Value))
        {
            dict["_text"] = element.Value;
        }

        return dict;
    }
}

public class HtmlService : IHtmlService
{
    private readonly ILogger<HtmlService> _logger;

    public HtmlService(ILogger<HtmlService> logger)
    {
        _logger = logger;
    }

    public async Task<DataFormatResponse<HtmlParseResult>> ParseHtmlAsync(string html, HtmlOptions? options = null)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<HtmlParseResult>();
        var result = new HtmlParseResult();

        try
        {
            options ??= new HtmlOptions();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Extract content based on selector
            if (!string.IsNullOrEmpty(options.Selector))
            {
                var nodes = doc.DocumentNode.SelectNodes(options.Selector);
                if (nodes != null)
                {
                    result.Content = string.Join("\n", nodes.Select(n => n.InnerText));
                }
            }
            else
            {
                result.Content = doc.DocumentNode.InnerText;
            }

            // Extract tables
            if (options.ExtractTables)
            {
                result.Tables = ExtractTables(doc);
            }

            // Extract links
            if (options.ExtractLinks)
            {
                result.Links = ExtractLinks(doc);
            }

            // Extract images
            if (options.ExtractImages)
            {
                result.Images = ExtractImages(doc);
            }

            response.Success = true;
            response.Data = result;
            response.SizeBytes = System.Text.Encoding.UTF8.GetByteCount(html);

            _logger.LogInformation("Parsed HTML: {Tables} tables, {Links} links, {Images} images",
                result.Tables.Count, result.Links.Count, result.Images.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error parsing HTML");
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<HtmlParseResult>> ReadHtmlFileAsync(string filePath, HtmlOptions? options = null)
    {
        var html = await File.ReadAllTextAsync(filePath);
        return await ParseHtmlAsync(html, options);
    }

    public async Task<DataFormatResponse<List<HtmlTable>>> ExtractTablesAsync(string html)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<List<HtmlTable>>();

        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var tables = ExtractTables(doc);

            response.Success = true;
            response.Data = tables;
            response.RecordCount = tables.Count;

            _logger.LogInformation("Extracted {Count} HTML tables", tables.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error extracting HTML tables");
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    private List<HtmlTable> ExtractTables(HtmlDocument doc)
    {
        var tables = new List<HtmlTable>();
        var tableNodes = doc.DocumentNode.SelectNodes("//table");

        if (tableNodes == null)
            return tables;

        foreach (var tableNode in tableNodes)
        {
            var table = new HtmlTable();

            // Extract headers
            var headerNodes = tableNode.SelectNodes(".//th");
            if (headerNodes != null)
            {
                table.Headers = headerNodes.Select(h => h.InnerText.Trim()).ToList();
            }

            // Extract rows
            var rowNodes = tableNode.SelectNodes(".//tr");
            if (rowNodes != null)
            {
                foreach (var rowNode in rowNodes)
                {
                    var cellNodes = rowNode.SelectNodes(".//td");
                    if (cellNodes != null)
                    {
                        var row = cellNodes.Select(c => c.InnerText.Trim()).ToList();
                        table.Rows.Add(row);
                    }
                }
            }

            table.RowCount = table.Rows.Count;
            table.ColumnCount = table.Headers.Count > 0 ? table.Headers.Count : 
                                table.Rows.FirstOrDefault()?.Count ?? 0;

            tables.Add(table);
        }

        return tables;
    }

    private List<HtmlLink> ExtractLinks(HtmlDocument doc)
    {
        var links = new List<HtmlLink>();
        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes == null)
            return links;

        foreach (var linkNode in linkNodes)
        {
            links.Add(new HtmlLink
            {
                Href = linkNode.GetAttributeValue("href", string.Empty),
                Text = linkNode.InnerText.Trim(),
                Title = linkNode.GetAttributeValue("title", null)
            });
        }

        return links;
    }

    private List<HtmlImage> ExtractImages(HtmlDocument doc)
    {
        var images = new List<HtmlImage>();
        var imgNodes = doc.DocumentNode.SelectNodes("//img[@src]");

        if (imgNodes == null)
            return images;

        foreach (var imgNode in imgNodes)
        {
            images.Add(new HtmlImage
            {
                Src = imgNode.GetAttributeValue("src", string.Empty),
                Alt = imgNode.GetAttributeValue("alt", null),
                Title = imgNode.GetAttributeValue("title", null)
            });
        }

        return images;
    }
}
