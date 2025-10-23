using System.Text.Json;
using System.Text.Json.Serialization;

namespace Module_03_Logging_Tracing.Services;

/// <summary>
/// Creates structured logging envelopes for requests/responses/errors that can be shipped to log storage.
/// </summary>
public sealed class RequestLogFormatter
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public string BuildRequestEnvelope(HttpContext context, TimeSpan elapsed)
    {
        var envelope = new
        {
            context.TraceIdentifier,
            context.Request.Method,
            Path = context.Request.Path.ToString(),
            context.Response.StatusCode,
            ElapsedMs = Math.Round(elapsed.TotalMilliseconds, 2),
            Host = context.Request.Host.ToString(),
            Query = context.Request.Query.ToDictionary(kvp => kvp.Key, kvp => string.Join(',', kvp.Value)),
            Headers = ExtractHeaders(context.Request.Headers)
        };

        return JsonSerializer.Serialize(envelope, SerializerOptions);
    }

    public string BuildErrorEnvelope(HttpContext context, Exception exception)
    {
        var envelope = new
        {
            context.TraceIdentifier,
            context.Request.Method,
            Path = context.Request.Path.ToString(),
            Error = exception.Message,
            StackTrace = exception.StackTrace,
            Source = exception.Source,
            context.Response.StatusCode,
            Headers = ExtractHeaders(context.Request.Headers)
        };

        return JsonSerializer.Serialize(envelope, SerializerOptions);
    }

    private static Dictionary<string, string> ExtractHeaders(IHeaderDictionary headers)
    {
        return headers
            .Where(kvp => kvp.Key.StartsWith("X-", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(kvp => kvp.Key, kvp => string.Join(',', kvp.Value));
    }
}
