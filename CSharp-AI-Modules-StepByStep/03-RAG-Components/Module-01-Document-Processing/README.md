# Module 01: Document Processing

## Overview
This module provides comprehensive document processing capabilities for RAG (Retrieval-Augmented Generation) systems, including text extraction from multiple formats, intelligent chunking strategies, OCR, preprocessing, and metadata extraction.

## Features

### 📄 Document Extraction
- **PDF**: iText7 and PdfPig support with page-range extraction
- **Word**: DOCX/DOC support using OpenXML and NPOI
- **Excel**: XLSX/XLS with table extraction
- **HTML**: Full HTML parsing with table extraction
- **Markdown**: Markdown to text conversion
- **Images**: OCR-based text extraction
- **Metadata**: Automatic metadata extraction from all formats

### ✂️ Chunking Strategies
1. **Fixed Size**: Equal-sized chunks with configurable overlap
2. **Sentence-Based**: Semantic sentence boundary splitting
3. **Paragraph-Based**: Natural paragraph boundaries
4. **Sliding Window**: Overlapping chunks for context preservation
5. **Semantic**: Similarity-based chunking (advanced)
6. **Recursive**: Hierarchical splitting with multiple separators

### 🔍 OCR (Optical Character Recognition)
- Tesseract OCR integration
- Multi-language support
- Word and line-level confidence scores
- Bounding box detection
- Image preprocessing options

### 🧹 Text Preprocessing
- Whitespace normalization
- Special character removal
- URL/Email removal
- Lowercase conversion
- Stop word removal
- Number removal

### 📊 Metadata Extraction
- File metadata (size, type, dates)
- Document properties (title, author, keywords)
- Text statistics (word count, sentence count, etc.)
- Language detection
- Keyword extraction

## Learning Objectives

- Extract text from various document formats (PDF, DOCX, Excel, HTML, Markdown)
- Implement 6 intelligent document chunking strategies
- Extract and preserve document metadata with enrichment
- Handle OCR for scanned documents and images
- Implement comprehensive preprocessing pipelines
- Build complete RAG document processing pipeline

## Project Structure

```
Module-01-Document-Processing/
├── Controllers/
│   └── DocumentProcessingController.cs    # REST API endpoints
├── Models/
│   └── DocumentProcessingModels.cs        # All data models
├── Services/
│   ├── DocumentExtractionService.cs       # PDF, Word, Excel, HTML extraction
│   ├── ChunkingService.cs                 # Chunking strategies + OCR
│   └── ProcessingServices.cs              # Preprocessing, Metadata, Batch
├── Extensions/
│   └── ServiceCollectionExtensions.cs     # Dependency injection
├── AI.DocumentProcessing.csproj           # Project configuration
├── Program.cs                             # Application entry point
├── appsettings.json                       # Configuration
├── appsettings.Development.json           # Dev configuration
└── README.md                              # This file
```

## Dependencies
- **iText7** (8.0.2) - PDF text extraction
- **PdfPig** (0.1.8) - Alternative PDF processing
- **NPOI** (2.6.2) - Excel/Word processing
- **DocumentFormat.OpenXml** (3.0.0) - Office document processing
- **HtmlAgilityPack** (1.11.54) - HTML parsing
- **Markdig** (0.33.0) - Markdown processing
- **Tesseract** (5.2.0) - OCR engine
- **Serilog** - Logging
- **Swashbuckle** - API documentation

## Installation

### Prerequisites
```powershell
# Install .NET 8.0 SDK
dotnet --version  # Should be 8.0 or higher
```

### Restore Dependencies
```powershell
cd Module-01-Document-Processing
dotnet restore
```

### Tesseract OCR Setup
1. Download Tesseract trained data files from: https://github.com/tesseract-ocr/tessdata
2. Create a `tessdata` folder in the project root
3. Place language data files (e.g., `eng.traineddata`) in the `tessdata` folder

## Configuration

### appsettings.json
```json
{
  "DocumentProcessing": {
    "TessDataPath": "tessdata",
    "DefaultExtractionOptions": {
      "ExtractImages": false,
      "ExtractTables": false,
      "ExtractMetadata": true,
      "UseOCR": false
    },
    "DefaultChunkingOptions": {
      "ChunkSize": 1000,
      "ChunkOverlap": 200,
      "MinChunkSize": 100,
      "MaxChunkSize": 2000
    },
    "MaxFileSizeMB": 100,
    "EnableCaching": true
  }
}
```

## Usage Examples

### 1. Document Extraction

#### Extract from PDF
```csharp
var extractionService = serviceProvider.GetRequiredService<IDocumentExtractionService>();

var pdfBytes = await File.ReadAllBytesAsync("document.pdf");
var request = new ExtractionRequest
{
    Content = pdfBytes,
    Type = DocumentType.PDF,
    Options = new ExtractionOptions
    {
        ExtractMetadata = true,
        ExtractTables = true,
        PageStart = 1,
        PageEnd = 10
    }
};

var result = await extractionService.ExtractAsync(request);
Console.WriteLine($"Extracted text: {result.Text}");
Console.WriteLine($"Pages: {result.Pages.Count}");
Console.WriteLine($"Tables: {result.Tables.Count}");
```

#### Extract from Word Document
```csharp
var wordBytes = await File.ReadAllBytesAsync("document.docx");
var request = new ExtractionRequest
{
    Content = wordBytes,
    Type = DocumentType.Word,
    Options = new ExtractionOptions { ExtractMetadata = true }
};

var result = await extractionService.ExtractAsync(request);
Console.WriteLine($"Title: {result.Metadata.Title}");
Console.WriteLine($"Author: {result.Metadata.Author}");
Console.WriteLine($"Word Count: {result.Metadata.WordCount}");
```

### 2. Text Chunking

#### Fixed-Size Chunking
```csharp
var chunkingService = serviceProvider.GetRequiredService<IChunkingService>();

var options = new ChunkingOptions
{
    ChunkSize = 1000,
    ChunkOverlap = 200,
    IncludeMetadata = true
};

var result = await chunkingService.ChunkByFixedSizeAsync(text, options);
Console.WriteLine($"Total chunks: {result.TotalChunks}");

foreach (var chunk in result.Chunks)
{
    Console.WriteLine($"Chunk {chunk.ChunkIndex}: {chunk.CharacterCount} characters");
}
```

### 3. OCR Processing

#### Extract Text from Image
```csharp
var ocrService = serviceProvider.GetRequiredService<IOCRService>();

var imageBytes = await File.ReadAllBytesAsync("scanned.png");
var request = new OCRRequest
{
    ImageData = imageBytes,
    Options = new OCROptions
    {
        Language = "eng",
        DPI = 300,
        AutoRotate = true
    }
};

var result = await ocrService.ExtractTextAsync(request);
Console.WriteLine($"Text: {result.Text}");
Console.WriteLine($"Confidence: {result.Confidence:P}");
Console.WriteLine($"Words detected: {result.Words.Count}");
```

## API Endpoints

### Extraction Endpoints
- `POST /api/documentprocessing/extract` - Extract text from document
- `POST /api/documentprocessing/extract/upload` - Extract from uploaded file
- `POST /api/documentprocessing/extract/pdf` - Extract from PDF with page range

### Chunking Endpoints
- `POST /api/documentprocessing/chunk` - Chunk text with strategy
- `POST /api/documentprocessing/chunk/fixed-size` - Fixed-size chunking
- `POST /api/documentprocessing/chunk/sentence` - Sentence-based chunking
- `POST /api/documentprocessing/chunk/paragraph` - Paragraph-based chunking
- `POST /api/documentprocessing/chunk/sliding-window` - Sliding window chunking

### Complete Pipeline
- `POST /api/documentprocessing/pipeline` - Complete extraction → preprocessing → chunking pipeline

## Running the Application

### Development Mode
```powershell
dotnet run
```

### Production Mode
```powershell
dotnet run --configuration Release
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000` (root URL)