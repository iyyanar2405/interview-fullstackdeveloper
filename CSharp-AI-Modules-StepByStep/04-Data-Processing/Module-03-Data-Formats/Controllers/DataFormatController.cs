using AI.DataFormats.Models;
using AI.DataFormats.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.DataFormats.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataFormatController : ControllerBase
{
    private readonly IJsonService _jsonService;
    private readonly ICsvService _csvService;
    private readonly IExcelService _excelService;
    private readonly IParquetService _parquetService;
    private readonly IXmlService _xmlService;
    private readonly IHtmlService _htmlService;
    private readonly IFormatConversionService _conversionService;
    private readonly ILogger<DataFormatController> _logger;

    public DataFormatController(
        IJsonService jsonService,
        ICsvService csvService,
        IExcelService excelService,
        IParquetService parquetService,
        IXmlService xmlService,
        IHtmlService htmlService,
        IFormatConversionService conversionService,
        ILogger<DataFormatController> logger)
    {
        _jsonService = jsonService;
        _csvService = csvService;
        _excelService = excelService;
        _parquetService = parquetService;
        _xmlService = xmlService;
        _htmlService = htmlService;
        _conversionService = conversionService;
        _logger = logger;
    }

    // JSON Endpoints
    [HttpPost("json/parse")]
    public async Task<ActionResult<ApiResponseModel<object>>> ParseJson([FromBody] string json)
    {
        try
        {
            var result = await _jsonService.ParseJsonAsync(json);
            return Ok(ApiResponseModel<object>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing JSON");
            return BadRequest(ApiResponseModel<object>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("json/validate")]
    public async Task<ActionResult<ApiResponseModel<JsonValidationResult>>> ValidateJson(
        [FromBody] JsonValidationRequest request)
    {
        try
        {
            var result = await _jsonService.ValidateJsonAsync(request.Json, request.Schema);
            return Ok(ApiResponseModel<JsonValidationResult>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating JSON");
            return BadRequest(ApiResponseModel<JsonValidationResult>.ErrorResponse(ex.Message));
        }
    }

    // CSV Endpoints
    [HttpPost("csv/parse")]
    public async Task<ActionResult<ApiResponseModel<CsvParseResult>>> ParseCsv(
        [FromBody] CsvParseRequest request)
    {
        try
        {
            var result = await _csvService.ParseCsvAsync(request.Content, request.Options);
            return Ok(ApiResponseModel<CsvParseResult>.SuccessResponse(result.Data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing CSV");
            return BadRequest(ApiResponseModel<CsvParseResult>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("csv/write")]
    public async Task<ActionResult<ApiResponseModel<string>>> WriteCsv(
        [FromBody] CsvWriteRequest request)
    {
        try
        {
            var result = await _csvService.WriteCsvAsync(request.Data, request.Options);
            return Ok(ApiResponseModel<string>.SuccessResponse(result.Data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing CSV");
            return BadRequest(ApiResponseModel<string>.ErrorResponse(ex.Message));
        }
    }

    // Excel Endpoints
    [HttpPost("excel/read")]
    public async Task<ActionResult<ApiResponseModel<ExcelReadResult>>> ReadExcel(
        [FromForm] IFormFile file,
        [FromForm] string? options = null)
    {
        try
        {
            var tempPath = Path.GetTempFileName();
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var excelOptions = string.IsNullOrEmpty(options) 
                ? null 
                : System.Text.Json.JsonSerializer.Deserialize<ExcelOptions>(options);

            var result = await _excelService.ReadExcelFileAsync(tempPath, excelOptions);

            System.IO.File.Delete(tempPath);

            return Ok(ApiResponseModel<ExcelReadResult>.SuccessResponse(result.Data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading Excel file");
            return BadRequest(ApiResponseModel<ExcelReadResult>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("excel/write")]
    public async Task<IActionResult> WriteExcel([FromBody] ExcelWriteRequest request)
    {
        try
        {
            var tempPath = Path.GetTempFileName() + ".xlsx";
            var result = await _excelService.WriteExcelFileAsync(tempPath, request.Sheets, request.Options);

            if (!result.Success)
            {
                return BadRequest(ApiResponseModel<bool>.ErrorResponse(result.Error ?? "Failed to write Excel file"));
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(tempPath);
            System.IO.File.Delete(tempPath);

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "output.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing Excel file");
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    // Parquet Endpoints
    [HttpPost("parquet/read")]
    public async Task<ActionResult<ApiResponseModel<List<Dictionary<string, object>>>>> ReadParquet(
        [FromForm] IFormFile file)
    {
        try
        {
            var tempPath = Path.GetTempFileName();
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var result = await _parquetService.ReadParquetFileAsync(tempPath);
            System.IO.File.Delete(tempPath);

            return Ok(ApiResponseModel<List<Dictionary<string, object>>>.SuccessResponse(result.Data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading Parquet file");
            return BadRequest(ApiResponseModel<List<Dictionary<string, object>>>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("parquet/write")]
    public async Task<IActionResult> WriteParquet([FromBody] ParquetWriteRequest request)
    {
        try
        {
            var tempPath = Path.GetTempFileName() + ".parquet";
            var result = await _parquetService.WriteParquetFileAsync(tempPath, request.Data, request.Options);

            if (!result.Success)
            {
                return BadRequest(ApiResponseModel<bool>.ErrorResponse(result.Error ?? "Failed to write Parquet file"));
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(tempPath);
            System.IO.File.Delete(tempPath);

            return File(bytes, "application/octet-stream", "output.parquet");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing Parquet file");
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    // XML Endpoints
    [HttpPost("xml/parse")]
    public async Task<ActionResult<ApiResponseModel<XmlParseResult>>> ParseXml(
        [FromBody] XmlParseRequest request)
    {
        try
        {
            var result = await _xmlService.ParseXmlAsync(request.Content, request.Options);
            return Ok(ApiResponseModel<XmlParseResult>.SuccessResponse(result.Data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing XML");
            return BadRequest(ApiResponseModel<XmlParseResult>.ErrorResponse(ex.Message));
        }
    }

    // HTML Endpoints
    [HttpPost("html/parse")]
    public async Task<ActionResult<ApiResponseModel<HtmlParseResult>>> ParseHtml(
        [FromBody] HtmlParseRequest request)
    {
        try
        {
            var result = await _htmlService.ParseHtmlAsync(request.Content, request.Options);
            return Ok(ApiResponseModel<HtmlParseResult>.SuccessResponse(result.Data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing HTML");
            return BadRequest(ApiResponseModel<HtmlParseResult>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("html/extract-tables")]
    public async Task<ActionResult<ApiResponseModel<List<HtmlTable>>>> ExtractHtmlTables(
        [FromBody] string html)
    {
        try
        {
            var result = await _htmlService.ExtractTablesAsync(html);
            return Ok(ApiResponseModel<List<HtmlTable>>.SuccessResponse(result.Data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting HTML tables");
            return BadRequest(ApiResponseModel<List<HtmlTable>>.ErrorResponse(ex.Message));
        }
    }

    // Conversion Endpoints
    [HttpPost("convert")]
    public async Task<ActionResult<ApiResponseModel<FormatConversionResult>>> Convert(
        [FromBody] FormatConversionRequest request)
    {
        try
        {
            var result = await _conversionService.ConvertAsync(request);
            return Ok(ApiResponseModel<FormatConversionResult>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting format");
            return BadRequest(ApiResponseModel<FormatConversionResult>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("detect-schema")]
    public async Task<ActionResult<ApiResponseModel<SchemaDetectionResult>>> DetectSchema(
        [FromForm] IFormFile file,
        [FromForm] DataFormat format)
    {
        try
        {
            var tempPath = Path.GetTempFileName();
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var result = await _conversionService.DetectSchemaAsync(tempPath, format);
            System.IO.File.Delete(tempPath);

            return Ok(ApiResponseModel<SchemaDetectionResult>.SuccessResponse(result.Data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting schema");
            return BadRequest(ApiResponseModel<SchemaDetectionResult>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "Data Format Service"
        });
    }
}

// Request Models
public class JsonValidationRequest
{
    public string Json { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
}

public class CsvParseRequest
{
    public string Content { get; set; } = string.Empty;
    public CsvOptions? Options { get; set; }
}

public class CsvWriteRequest
{
    public List<Dictionary<string, object>> Data { get; set; } = new();
    public CsvOptions? Options { get; set; }
}

public class ExcelWriteRequest
{
    public Dictionary<string, List<Dictionary<string, object>>> Sheets { get; set; } = new();
    public ExcelWriteOptions? Options { get; set; }
}

public class ParquetWriteRequest
{
    public List<Dictionary<string, object>> Data { get; set; } = new();
    public ParquetOptions? Options { get; set; }
}

public class XmlParseRequest
{
    public string Content { get; set; } = string.Empty;
    public XmlOptions? Options { get; set; }
}

public class HtmlParseRequest
{
    public string Content { get; set; } = string.Empty;
    public HtmlOptions? Options { get; set; }
}
