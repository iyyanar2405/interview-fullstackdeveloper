using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Module_03_Logging_Tracing.Services;

namespace Module_03_Logging_Tracing.Controllers;

[ApiController]
[Route("api/diagnostics")]
public sealed class DiagnosticsController : ControllerBase
{
    private readonly ILogger<DiagnosticsController> _logger;
    private readonly RequestTracingService _tracing;

    public DiagnosticsController(ILogger<DiagnosticsController> logger, RequestTracingService tracing)
    {
        _logger = logger;
        _tracing = tracing;
    }

    [HttpGet("ping")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        using var span = _tracing.StartSpan("diagnostics.ping");
        _logger.LogInformation("Ping endpoint invoked");
        return Ok(new { Message = "pong", Timestamp = DateTimeOffset.UtcNow });
    }

    [HttpGet("generate-error")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult GenerateError()
    {
        using var span = _tracing.StartSpan("diagnostics.generate_error");
        try
        {
            ThrowSampleException();
        }
        catch (Exception ex)
        {
            span.RecordException(ex);
            _logger.LogError(ex, "Explicit diagnostic error generated");
            throw;
        }

        return NoContent();
    }

    private static void ThrowSampleException()
    {
        throw new InvalidOperationException("Diagnostic error triggered for logging pipeline validation.");
    }
}
