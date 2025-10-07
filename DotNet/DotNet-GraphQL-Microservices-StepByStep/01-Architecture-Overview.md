# 01 — Architecture Overview

How GraphQL fits microservices, and composition patterns.

## Why GraphQL in microservices?
- Single entry point for clients
- Aggregates data across services
- Evolvable schema with strong typing

## Composition options
- Federation (Apollo Federation, Hot Chocolate Fusion)
  - Pros: declarative ownership per subgraph, entity references, distributed schema
  - Cons: operational complexity, subgraph discipline
- Schema stitching (older): resolver-based composition
- Single “Backend for Frontend” (BFF): simpler but centralizes logic

## Key concepts
- Subgraph: independently owned GraphQL service
- Gateway: composes executable supergraph
- Entity: type with a unique key across subgraphs
- Extension: add fields to an entity in another subgraph

## Evolution & contracts
- Use SDL as the contract per subgraph
- Backward-compatible field additions; avoid breaking changes
- Versioning via deprecation, not hard forks

## Operational pillars
- Security: JWT across gateway + services
- Resilience: timeouts/retries, circuit breakers (Polly)
- Observability: correlation IDs, logs, traces, metrics
- Performance: DataLoader, caching, pagination

Next: Setup and solution structure.
