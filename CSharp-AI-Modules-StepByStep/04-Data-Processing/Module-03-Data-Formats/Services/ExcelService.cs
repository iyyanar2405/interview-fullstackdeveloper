using AI.DataFormats.Models;
using ClosedXML.Excel;
using ExcelDataReader;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace AI.DataFormats.Services;

public interface IExcelService
{
    Task<DataFormatResponse<ExcelReadResult>> ReadExcelFileAsync(string filePath, ExcelOptions? options = null);
    Task<DataFormatResponse<bool>> WriteExcelFileAsync(string filePath, Dictionary<string, List<Dictionary<string, object>>> sheets, ExcelWriteOptions? options = null);
    Task<DataFormatResponse<List<Dictionary<string, object>>>> ReadSheetAsync(string filePath, string sheetName, ExcelOptions? options = null);
    Task<DataFormatResponse<bool>> WriteSheetAsync(string filePath, string sheetName, List<Dictionary<string, object>> data, ExcelWriteOptions? options = null);
}

public class ExcelService : IExcelService
{
    private readonly ILogger<ExcelService> _logger;

    static ExcelService()
    {
        // Required for ExcelDataReader to work with code pages
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public ExcelService(ILogger<ExcelService> logger)
    {
        _logger = logger;
    }

    public async Task<DataFormatResponse<ExcelReadResult>> ReadExcelFileAsync(string filePath, ExcelOptions? options = null)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<ExcelReadResult>();
        var result = new ExcelReadResult();

        try
        {
            options ??= new ExcelOptions();

            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = options.HasHeader
                }
            });

            result.TotalSheets = dataSet.Tables.Count;

            foreach (DataTable table in dataSet.Tables)
            {
                result.SheetNames.Add(table.TableName);

                if (options.ReadAllSheets || 
                    (options.SheetName != null && table.TableName == options.SheetName) ||
                    (options.SheetName == null && dataSet.Tables.IndexOf(table) == options.SheetIndex))
                {
                    var sheetData = ConvertTableToDictionary(table, options);
                    result.Sheets[table.TableName] = sheetData;
                    result.TotalRows += sheetData.Count;

                    if (!options.ReadAllSheets)
                        break;
                }
            }

            result.Metadata["FilePath"] = filePath;
            result.Metadata["FileName"] = Path.GetFileName(filePath);

            response.Success = true;
            response.Data = result;
            response.RecordCount = result.TotalRows;
            response.SizeBytes = new FileInfo(filePath).Length;

            _logger.LogInformation("Read Excel file: {FilePath}, {Sheets} sheets, {Rows} rows",
                filePath, result.TotalSheets, result.TotalRows);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error reading Excel file: {FilePath}", filePath);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<bool>> WriteExcelFileAsync(
        string filePath,
        Dictionary<string, List<Dictionary<string, object>>> sheets,
        ExcelWriteOptions? options = null)
    {
        await Task.CompletedTask;
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<bool>();

        try
        {
            options ??= new ExcelWriteOptions();

            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var workbook = new XLWorkbook();

            foreach (var sheetEntry in sheets)
            {
                var sheetName = sheetEntry.Key;
                var data = sheetEntry.Value;

                if (data.Count == 0)
                    continue;

                var worksheet = workbook.Worksheets.Add(sheetName);

                // Write headers
                var headers = data.First().Keys.ToList();
                for (int col = 0; col < headers.Count; col++)
                {
                    worksheet.Cell(1, col + 1).Value = headers[col];

                    if (options.ApplyStyles)
                    {
                        worksheet.Cell(1, col + 1).Style.Font.Bold = true;
                        worksheet.Cell(1, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                }

                // Write data rows
                for (int row = 0; row < data.Count; row++)
                {
                    var record = data[row];
                    for (int col = 0; col < headers.Count; col++)
                    {
                        var value = record.ContainsKey(headers[col]) ? record[headers[col]] : string.Empty;
                        worksheet.Cell(row + 2, col + 1).Value = value?.ToString() ?? string.Empty;
                    }
                }

                // Auto-fit columns
                if (options.AutoFitColumns)
                {
                    worksheet.Columns().AdjustToContents();
                }
            }

            workbook.SaveAs(filePath);

            response.Success = true;
            response.Data = true;
            response.RecordCount = sheets.Values.Sum(s => s.Count);
            response.SizeBytes = new FileInfo(filePath).Length;

            _logger.LogInformation("Wrote Excel file: {FilePath}, {Sheets} sheets",
                filePath, sheets.Count);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error writing Excel file: {FilePath}", filePath);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<List<Dictionary<string, object>>>> ReadSheetAsync(
        string filePath,
        string sheetName,
        ExcelOptions? options = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DataFormatResponse<List<Dictionary<string, object>>>();

        try
        {
            options ??= new ExcelOptions { SheetName = sheetName };
            options.SheetName = sheetName;

            var readResult = await ReadExcelFileAsync(filePath, options);

            if (!readResult.Success || readResult.Data == null)
            {
                response.Success = false;
                response.Error = readResult.Error ?? "Failed to read Excel file";
                return response;
            }

            if (readResult.Data.Sheets.TryGetValue(sheetName, out var sheetData))
            {
                response.Success = true;
                response.Data = sheetData;
                response.RecordCount = sheetData.Count;
            }
            else
            {
                response.Success = false;
                response.Error = $"Sheet '{sheetName}' not found";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            _logger.LogError(ex, "Error reading Excel sheet: {SheetName}", sheetName);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTime = stopwatch.Elapsed;
        }

        return response;
    }

    public async Task<DataFormatResponse<bool>> WriteSheetAsync(
        string filePath,
        string sheetName,
        List<Dictionary<string, object>> data,
        ExcelWriteOptions? options = null)
    {
        var sheets = new Dictionary<string, List<Dictionary<string, object>>>
        {
            [sheetName] = data
        };

        return await WriteExcelFileAsync(filePath, sheets, options);
    }

    // Helper methods
    private List<Dictionary<string, object>> ConvertTableToDictionary(DataTable table, ExcelOptions options)
    {
        var result = new List<Dictionary<string, object>>();
        var startRow = options.HasHeader ? 1 : 0;
        var endRow = options.EndRow ?? table.Rows.Count;

        for (int i = startRow; i < endRow && i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];

            // Skip empty rows if configured
            if (options.SkipEmptyRows && IsEmptyRow(row))
                continue;

            var record = new Dictionary<string, object>();

            for (int j = 0; j < table.Columns.Count; j++)
            {
                var columnName = table.Columns[j].ColumnName;
                var value = row[j];

                // Convert DBNull to empty string
                if (value == DBNull.Value)
                    value = string.Empty;

                record[columnName] = value;
            }

            result.Add(record);
        }

        return result;
    }

    private bool IsEmptyRow(DataRow row)
    {
        return row.ItemArray.All(field => 
            field == null || 
            field == DBNull.Value || 
            string.IsNullOrWhiteSpace(field.ToString()));
    }
}
