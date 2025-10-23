using AI.DocumentProcessing.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using UglyToad.PdfPig;
using NPOI.XWPF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;
using Markdig;
using System.Text;

namespace AI.DocumentProcessing.Services;

public interface IDocumentExtractionService
{
    Task<ExtractionResult> ExtractAsync(ExtractionRequest request);
    Task<ExtractionResult> ExtractFromPathAsync(string filePath, ExtractionOptions? options = null);
    DocumentType DetectDocumentType(string fileName);
}

public class DocumentExtractionService : IDocumentExtractionService
{
    private readonly IOCRService _ocrService;

    public DocumentExtractionService(IOCRService ocrService)
    {
        _ocrService = ocrService;
    }

    public async Task<ExtractionResult> ExtractAsync(ExtractionRequest request)
    {
        var result = new ExtractionResult();

        try
        {
            result = request.Type switch
            {
                DocumentType.PDF => await ExtractFromPdfAsync(request),
                DocumentType.Word => await ExtractFromWordAsync(request),
                DocumentType.Excel => await ExtractFromExcelAsync(request),
                DocumentType.Text => await ExtractFromTextAsync(request),
                DocumentType.HTML => await ExtractFromHtmlAsync(request),
                DocumentType.Markdown => await ExtractFromMarkdownAsync(request),
                DocumentType.Image => await ExtractFromImageAsync(request),
                _ => throw new NotSupportedException($"Document type {request.Type} is not supported")
            };

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Extraction failed: {ex.Message}");
        }

        return result;
    }

    public async Task<ExtractionResult> ExtractFromPathAsync(string filePath, ExtractionOptions? options = null)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var content = await File.ReadAllBytesAsync(filePath);
        var type = DetectDocumentType(filePath);

        var request = new ExtractionRequest
        {
            Content = content,
            Type = type,
            FilePath = filePath,
            Options = options ?? new ExtractionOptions()
        };

        return await ExtractAsync(request);
    }

    public DocumentType DetectDocumentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".pdf" => DocumentType.PDF,
            ".doc" or ".docx" => DocumentType.Word,
            ".xls" or ".xlsx" => DocumentType.Excel,
            ".ppt" or ".pptx" => DocumentType.PowerPoint,
            ".txt" => DocumentType.Text,
            ".html" or ".htm" => DocumentType.HTML,
            ".md" => DocumentType.Markdown,
            ".png" or ".jpg" or ".jpeg" or ".bmp" or ".tiff" => DocumentType.Image,
            ".csv" => DocumentType.CSV,
            ".json" => DocumentType.JSON,
            ".xml" => DocumentType.XML,
            _ => DocumentType.Unknown
        };
    }

    #region PDF Extraction

    private async Task<ExtractionResult> ExtractFromPdfAsync(ExtractionRequest request)
    {
        var result = new ExtractionResult();
        var textBuilder = new StringBuilder();

        try
        {
            // Method 1: Using iText7
            using var memoryStream = new MemoryStream(request.Content);
            using var pdfReader = new PdfReader(memoryStream);
            using var pdfDocument = new PdfDocument(pdfReader);

            var totalPages = pdfDocument.GetNumberOfPages();
            var startPage = request.Options.PageStart ?? 1;
            var endPage = request.Options.PageEnd ?? totalPages;

            // Extract metadata
            if (request.Options.ExtractMetadata)
            {
                var docInfo = pdfDocument.GetDocumentInfo();
                result.Metadata = new DocumentMetadata
                {
                    Title = docInfo.GetTitle(),
                    Author = docInfo.GetAuthor(),
                    Subject = docInfo.GetSubject(),
                    Keywords = docInfo.GetKeywords(),
                    PageCount = totalPages
                };
            }

            // Extract text from each page
            for (int pageNum = startPage; pageNum <= endPage; pageNum++)
            {
                var page = pdfDocument.GetPage(pageNum);
                var strategy = new LocationTextExtractionStrategy();
                var pageText = PdfTextExtractor.GetTextFromPage(page, strategy);

                var extractedPage = new ExtractedPage
                {
                    PageNumber = pageNum,
                    Text = pageText,
                    CharacterCount = pageText.Length
                };

                textBuilder.AppendLine(pageText);
                result.Pages.Add(extractedPage);

                // Extract images if requested
                if (request.Options.ExtractImages)
                {
                    var images = await ExtractImagesFromPdfPageAsync(page, pageNum, request.Options.UseOCR);
                    extractedPage.Images.AddRange(images);
                    result.Images.AddRange(images);
                }
            }

            result.Text = textBuilder.ToString();

            // If OCR is enabled and text extraction yielded little content, try OCR on entire document
            if (request.Options.UseOCR && result.Text.Length < 100)
            {
                result.Text = await ExtractTextWithOCRAsync(request.Content);
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"PDF extraction failed: {ex.Message}");
            
            // Fallback to PdfPig if iText7 fails
            try
            {
                result = await ExtractFromPdfUsingPdfPigAsync(request);
            }
            catch (Exception fallbackEx)
            {
                result.Errors.Add($"PdfPig fallback failed: {fallbackEx.Message}");
            }
        }

        return result;
    }

    private async Task<ExtractionResult> ExtractFromPdfUsingPdfPigAsync(ExtractionRequest request)
    {
        var result = new ExtractionResult();
        var textBuilder = new StringBuilder();

        using var memoryStream = new MemoryStream(request.Content);
        using var pdfDocument = PdfDocument.Open(memoryStream);

        foreach (var page in pdfDocument.GetPages())
        {
            var pageText = page.Text;
            textBuilder.AppendLine(pageText);

            result.Pages.Add(new ExtractedPage
            {
                PageNumber = page.Number,
                Text = pageText,
                CharacterCount = pageText.Length
            });
        }

        result.Text = textBuilder.ToString();
        result.Metadata.PageCount = pdfDocument.NumberOfPages;

        return result;
    }

    private async Task<List<ExtractedImage>> ExtractImagesFromPdfPageAsync(iText.Kernel.Pdf.PdfPage page, int pageNumber, bool useOCR)
    {
        var images = new List<ExtractedImage>();
        // Image extraction implementation would go here
        // This is a placeholder for the actual implementation
        return await Task.FromResult(images);
    }

    private async Task<string> ExtractTextWithOCRAsync(byte[] pdfContent)
    {
        // OCR implementation for entire PDF
        // This would convert PDF pages to images and run OCR
        return await Task.FromResult(string.Empty);
    }

    #endregion

    #region Word Extraction

    private async Task<ExtractionResult> ExtractFromWordAsync(ExtractionRequest request)
    {
        var result = new ExtractionResult();
        var textBuilder = new StringBuilder();

        try
        {
            using var memoryStream = new MemoryStream(request.Content);

            // Check if it's DOCX (OpenXML format)
            if (IsOpenXmlFormat(request.Content))
            {
                using var wordDocument = WordprocessingDocument.Open(memoryStream, false);
                var body = wordDocument.MainDocumentPart?.Document.Body;

                if (body != null)
                {
                    // Extract metadata
                    if (request.Options.ExtractMetadata)
                    {
                        var coreProps = wordDocument.PackageProperties;
                        result.Metadata = new DocumentMetadata
                        {
                            Title = coreProps.Title,
                            Author = coreProps.Creator,
                            Subject = coreProps.Subject,
                            Keywords = coreProps.Keywords,
                            CreationDate = coreProps.Created,
                            ModificationDate = coreProps.Modified
                        };
                    }

                    // Extract text
                    foreach (var paragraph in body.Descendants<Paragraph>())
                    {
                        textBuilder.AppendLine(paragraph.InnerText);
                    }
                }
            }
            else
            {
                // Use NPOI for older DOC format
                var doc = new XWPFDocument(memoryStream);

                foreach (var paragraph in doc.Paragraphs)
                {
                    textBuilder.AppendLine(paragraph.ParagraphText);
                }
            }

            result.Text = textBuilder.ToString();
            result.Metadata.WordCount = result.Text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Word extraction failed: {ex.Message}");
        }

        return await Task.FromResult(result);
    }

    private bool IsOpenXmlFormat(byte[] content)
    {
        // Check for ZIP signature (OpenXML files are ZIP archives)
        return content.Length > 4 && 
               content[0] == 0x50 && 
               content[1] == 0x4B && 
               content[2] == 0x03 && 
               content[3] == 0x04;
    }

    #endregion

    #region Excel Extraction

    private async Task<ExtractionResult> ExtractFromExcelAsync(ExtractionRequest request)
    {
        var result = new ExtractionResult();
        var textBuilder = new StringBuilder();

        try
        {
            using var memoryStream = new MemoryStream(request.Content);

            // Check if it's XLSX (OpenXML format)
            if (IsOpenXmlFormat(request.Content))
            {
                var workbook = new XSSFWorkbook(memoryStream);

                for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                {
                    var sheet = workbook.GetSheetAt(sheetIndex);
                    textBuilder.AppendLine($"Sheet: {sheet.SheetName}");

                    if (request.Options.ExtractTables)
                    {
                        var table = ExtractTableFromSheet(sheet);
                        result.Tables.Add(table);
                    }

                    foreach (var row in sheet)
                    {
                        var rowText = new StringBuilder();
                        foreach (var cell in row.Cells)
                        {
                            rowText.Append(GetCellValue(cell) + "\t");
                        }
                        textBuilder.AppendLine(rowText.ToString());
                    }
                }
            }
            else
            {
                // Use NPOI for older XLS format
                var workbook = new HSSFWorkbook(memoryStream);

                for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                {
                    var sheet = workbook.GetSheetAt(sheetIndex);
                    textBuilder.AppendLine($"Sheet: {sheet.SheetName}");

                    foreach (var row in sheet)
                    {
                        var rowText = new StringBuilder();
                        foreach (var cell in row.Cells)
                        {
                            rowText.Append(GetCellValue(cell) + "\t");
                        }
                        textBuilder.AppendLine(rowText.ToString());
                    }
                }
            }

            result.Text = textBuilder.ToString();
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Excel extraction failed: {ex.Message}");
        }

        return await Task.FromResult(result);
    }

    private ExtractedTable ExtractTableFromSheet(dynamic sheet)
    {
        var table = new ExtractedTable { PageNumber = 1 };
        
        var firstRow = sheet.GetRow(0);
        if (firstRow != null)
        {
            foreach (var cell in firstRow.Cells)
            {
                table.Headers.Add(GetCellValue(cell));
            }
        }

        for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
        {
            var row = sheet.GetRow(rowIndex);
            if (row != null)
            {
                var rowData = new List<string>();
                foreach (var cell in row.Cells)
                {
                    rowData.Add(GetCellValue(cell));
                }
                table.Rows.Add(rowData);
            }
        }

        return table;
    }

    private string GetCellValue(dynamic cell)
    {
        if (cell == null) return string.Empty;

        return cell.CellType switch
        {
            NPOI.SS.UserModel.CellType.Numeric => cell.NumericCellValue.ToString(),
            NPOI.SS.UserModel.CellType.String => cell.StringCellValue,
            NPOI.SS.UserModel.CellType.Boolean => cell.BooleanCellValue.ToString(),
            NPOI.SS.UserModel.CellType.Formula => cell.CellFormula,
            _ => string.Empty
        };
    }

    #endregion

    #region HTML Extraction

    private async Task<ExtractionResult> ExtractFromHtmlAsync(ExtractionRequest request)
    {
        var result = new ExtractionResult();

        try
        {
            var html = Encoding.UTF8.GetString(request.Content);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Extract metadata
            if (request.Options.ExtractMetadata)
            {
                result.Metadata.Title = htmlDoc.DocumentNode.SelectSingleNode("//title")?.InnerText;
                result.Metadata.Author = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='author']")?.GetAttributeValue("content", "");
                result.Metadata.Keywords = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='keywords']")?.GetAttributeValue("content", "");
            }

            // Extract text (remove scripts and styles)
            var nodesToRemove = htmlDoc.DocumentNode.SelectNodes("//script|//style");
            if (nodesToRemove != null)
            {
                foreach (var node in nodesToRemove)
                {
                    node.Remove();
                }
            }

            result.Text = htmlDoc.DocumentNode.InnerText.Trim();

            // Extract tables if requested
            if (request.Options.ExtractTables)
            {
                var tables = htmlDoc.DocumentNode.SelectNodes("//table");
                if (tables != null)
                {
                    foreach (var tableNode in tables)
                    {
                        result.Tables.Add(ExtractHtmlTable(tableNode));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"HTML extraction failed: {ex.Message}");
        }

        return await Task.FromResult(result);
    }

    private ExtractedTable ExtractHtmlTable(HtmlNode tableNode)
    {
        var table = new ExtractedTable();

        // Extract headers
        var headerRow = tableNode.SelectSingleNode(".//thead//tr") ?? tableNode.SelectSingleNode(".//tr");
        if (headerRow != null)
        {
            var headerCells = headerRow.SelectNodes(".//th|.//td");
            if (headerCells != null)
            {
                table.Headers.AddRange(headerCells.Select(c => c.InnerText.Trim()));
            }
        }

        // Extract rows
        var rows = tableNode.SelectNodes(".//tbody//tr") ?? tableNode.SelectNodes(".//tr[position()>1]");
        if (rows != null)
        {
            foreach (var row in rows)
            {
                var cells = row.SelectNodes(".//td");
                if (cells != null)
                {
                    table.Rows.Add(cells.Select(c => c.InnerText.Trim()).ToList());
                }
            }
        }

        return table;
    }

    #endregion

    #region Text and Markdown Extraction

    private async Task<ExtractionResult> ExtractFromTextAsync(ExtractionRequest request)
    {
        var result = new ExtractionResult
        {
            Text = Encoding.UTF8.GetString(request.Content)
        };

        return await Task.FromResult(result);
    }

    private async Task<ExtractionResult> ExtractFromMarkdownAsync(ExtractionRequest request)
    {
        var result = new ExtractionResult();

        try
        {
            var markdown = Encoding.UTF8.GetString(request.Content);
            
            // Convert to HTML first, then extract text
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var html = Markdown.ToHtml(markdown, pipeline);

            // Extract plain text
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            result.Text = htmlDoc.DocumentNode.InnerText;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Markdown extraction failed: {ex.Message}");
        }

        return await Task.FromResult(result);
    }

    #endregion

    #region Image Extraction

    private async Task<ExtractionResult> ExtractFromImageAsync(ExtractionRequest request)
    {
        var result = new ExtractionResult();

        try
        {
            if (request.Options.UseOCR)
            {
                var ocrRequest = new OCRRequest
                {
                    ImageData = request.Content,
                    Options = new OCROptions
                    {
                        Language = "eng",
                        DPI = 300
                    }
                };

                var ocrResult = await _ocrService.ExtractTextAsync(ocrRequest);
                result.Text = ocrResult.Text;
            }
            else
            {
                result.Errors.Add("OCR is disabled. Enable OCR to extract text from images.");
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Image extraction failed: {ex.Message}");
        }

        return result;
    }

    #endregion
}
