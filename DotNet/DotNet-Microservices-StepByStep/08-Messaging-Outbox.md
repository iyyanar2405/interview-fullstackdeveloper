# 08 — Messaging & Outbox

Integrate async messaging and ensure reliable publishing.

## Broker options
- RabbitMQ: simple queues, topics
- Apache Kafka: high-throughput event streaming

## Outbox pattern
- Write domain event to Outbox table in the same transaction as state change
- Background worker reads Outbox and publishes to broker
- Mark event as dispatched (idempotent handling)

## Idempotency keys
- Use request IDs to prevent duplicate processing on retries

Next: Observability across services.
