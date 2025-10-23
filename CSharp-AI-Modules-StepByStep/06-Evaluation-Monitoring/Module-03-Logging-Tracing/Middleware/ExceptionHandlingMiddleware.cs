using Module_03_Logging_Tracing.Services;

namespace Module_03_Logging_Tracing.Middleware;

/// <summary>
/// Captures unhandled exceptions, emits structured error logs, and returns problem details to the client.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly RequestLogFormatter _formatter;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        RequestLogFormatter formatter)
    {
        _next = next;
        _logger = logger;
        _formatter = formatter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var envelope = _formatter.BuildErrorEnvelope(context, ex);
            _logger.LogError(ex, "Unhandled exception: {Envelope}", envelope);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Title = "An unexpected error occurred",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.TraceIdentifier
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
