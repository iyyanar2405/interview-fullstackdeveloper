# 01 — Architecture Overview

Microservices: small, independently deployable services around business capabilities.

## Core principles
- Strong bounded contexts and clear ownership
- Smart endpoints, dumb pipes
- Async first (events), sync when necessary
- Automation: infra as code, CI/CD, observability

## Communication styles
- Synchronous: REST/gRPC for request/response
- Asynchronous: events with Kafka/RabbitMQ for decoupling

## Data ownership
- Each service owns its database (no shared DB)
- Cross-service queries via composition (API gateway, GraphQL) or materialized views
- Consistency: eventual via events + outbox

## API gateway patterns
- Routing/aggregation via YARP (reverse proxy) or GraphQL BFF
- Central auth, rate limiting, observability hooks

## Reliability & security
- Timeouts, retries, circuit breakers (Polly)
- JWT per service; propagate identity and correlation IDs

Next: Setup & tooling.
