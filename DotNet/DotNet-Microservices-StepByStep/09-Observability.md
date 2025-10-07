# 09 — Observability

Centralized logging, tracing and metrics across services.

## Logging
- Serilog structured logs to console
- Enrich with correlation/trace IDs

## Tracing & metrics
- OpenTelemetry tracing and metrics
- Exporters: Console, OTLP (Jaeger/Tempo), Prometheus

## Correlation
- Propagate `traceparent` and `x-correlation-id` via gateway and services

Next: Testing strategies.
