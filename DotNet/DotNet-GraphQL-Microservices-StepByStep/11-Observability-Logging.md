# 11 — Observability & Logging

Centralized logs, tracing, and metrics across services.

## Structured logging
- Serilog to console with correlation IDs
- Forward headers like `x-correlation-id`

## Tracing
- OpenTelemetry exporter
- Correlate traces across gateway and subgraphs

## Metrics
- ASP.NET Core meters and Prometheus exporters

Next: Containerize services and the gateway.
