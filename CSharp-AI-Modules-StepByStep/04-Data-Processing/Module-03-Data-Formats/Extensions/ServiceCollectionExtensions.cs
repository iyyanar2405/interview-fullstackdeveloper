using AI.DataFormats.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AI.DataFormats.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataFormatServices(this IServiceCollection services)
    {
        // Register format-specific services
        services.AddScoped<IJsonService, JsonService>();
        services.AddScoped<ICsvService, CsvService>();
        services.AddScoped<IExcelService, ExcelService>();
        services.AddScoped<IParquetService, ParquetService>();
        services.AddScoped<IXmlService, XmlService>();
        services.AddScoped<IHtmlService, HtmlService>();
        
        // Register conversion service
        services.AddScoped<IFormatConversionService, FormatConversionService>();

        // Add HTTP client for potential external validations
        services.AddHttpClient();

        return services;
    }
}
