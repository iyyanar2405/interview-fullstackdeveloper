using AI.Foundation.API.Models;
using System.Net;
using System.Text.Json;

namespace AI.Foundation.API.Middleware;

/// <summary>
/// Global error handling middleware
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        };

        switch (exception)
        {
            case ArgumentException argEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Error = "Invalid argument";
                errorResponse.ErrorCode = ErrorCodes.InvalidRequest;
                errorResponse.Details = argEx.Message;
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Error = "Unauthorized access";
                errorResponse.ErrorCode = ErrorCodes.Unauthorized;
                break;

            case TimeoutException:
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse.Error = "Request timeout";
                errorResponse.ErrorCode = ErrorCodes.TimeoutError;
                break;

            case TaskCanceledException:
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse.Error = "Request was cancelled";
                errorResponse.ErrorCode = ErrorCodes.TimeoutError;
                break;

            case HttpRequestException httpEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                errorResponse.Error = "External service error";
                errorResponse.ErrorCode = ErrorCodes.ServiceUnavailable;
                errorResponse.Details = httpEx.Message;
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Error = "An internal server error occurred";
                errorResponse.ErrorCode = ErrorCodes.InternalServerError;
                
                // Only include detailed error information in development
                if (_environment.IsDevelopment())
                {
                    errorResponse.Details = exception.Message;
                    errorResponse.Metadata = new Dictionary<string, object>
                    {
                        ["exception"] = new ExceptionDetails
                        {
                            Type = exception.GetType().Name,
                            Message = exception.Message,
                            StackTrace = exception.StackTrace,
                            InnerException = exception.InnerException != null
                                ? new ExceptionDetails
                                {
                                    Type = exception.InnerException.GetType().Name,
                                    Message = exception.InnerException.Message
                                }
                                : null
                        }
                    };
                }
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}