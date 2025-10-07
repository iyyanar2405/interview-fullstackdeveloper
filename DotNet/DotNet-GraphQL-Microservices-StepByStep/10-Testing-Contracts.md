# 10 — Testing & Contracts

Validate subgraph SDL, gateway composition, and end-to-end flows.

## Contract tests (SDL)
- Check that required types/fields exist in each subgraph
- Detect breaking changes (removed/renamed fields)

## Integration tests
- Spin up Accounts/Products on random ports
- Start gateway with remote schemas pointing to those ports
- Send GraphQL queries via HttpClient

## Snapshot tests
- Snapshot common queries and verify shape and performance

Next: Observability and logging.
