# Module 03: Logging & Tracing

Reference implementation for structured logging and distributed tracing in ASP.NET Core.

## Highlights

- **Correlation IDs**: Incoming requests receive an `X-Correlation-Id` header that flows across logs and traces.
- **Structured Logging**: Serilog enrichers capture machine, process, thread, and activity identifiers.
- **Exception Pipeline**: Centralized middleware emits structured envelopes and returns RFC 7807 problem details.
- **Request Diagnostics**: Per-request structured log payloads with method, route, status code, duration, and headers.
- **OpenTelemetry Tracing**: ASP.NET Core, `HttpClient`, and SQL instrumentation exporting to Zipkin and console.
- **Zipkin-Compatible**: Configuration defaults to `http://localhost:9411/api/v2/spans` (override via `Tracing:ZipkinEndpoint`).

## Project Layout

```
Module-03-Logging-Tracing/
├── Controllers/
│   └── DiagnosticsController.cs
├── Infrastructure/
│   └── LoggingEnricher.cs
├── Middleware/
│   ├── CorrelationIdMiddleware.cs
│   ├── ExceptionHandlingMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Services/
│   ├── RequestLogFormatter.cs
│   └── RequestTracingService.cs
├── Module-03-Logging-Tracing.csproj
├── Program.cs
├── appsettings.json
└── README.md
```

## Endpoints

| Method | Route | Description |
| ------ | ----- | ----------- |
| GET | `/api/diagnostics/ping` | Emits a simple span/log pair for connectivity checks. |
| GET | `/api/diagnostics/generate-error` | Throws an exception to validate error logging. |
| GET | `/health` | Liveness probe. |
| GET | `/swagger` | Swagger UI and OpenAPI spec. |

## Running Locally

```powershell
cd Module-03-Logging-Tracing
 dotnet restore
 dotnet run
```

Optional: launch Zipkin locally (Docker example):

```powershell
docker run -d -p 9411:9411 openzipkin/zipkin
```

## Configuration Notes

- Update `appsettings.json` `Serilog.WriteTo` section to forward logs to Seq, Elastic, or other sinks.
- Replace Zipkin exporter with OTLP or Jaeger by swapping OpenTelemetry exporters in `Program.cs`.
- Middleware order is critical: correlation → request logging → exception handling ensures consistent envelopes.

Use this module alongside Modules 01 and 02 to obtain correlated performance, quality, and trace visibility.
