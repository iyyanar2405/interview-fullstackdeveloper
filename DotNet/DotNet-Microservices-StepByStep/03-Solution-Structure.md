# 03 — Solution Structure

Organize source, shared code, and tests for clarity and autonomy.

## Suggested structure
```
src/
  CatalogService/
    Controllers/
    Endpoints/
    Application/
    Domain/
    Infrastructure/
  OrdersService/
  ApiGateway/
shared/
  BuildingBlocks/
    BuildingBlocks.csproj
    Abstractions/
    Messaging/
    Observability/
 tests/
  Catalog.Tests/
  Orders.Tests/
  E2E.Tests/
```

- Keep shared minimal; prefer package references over shared projects when possible
- Each service owns its own data and migrations
- Align namespaces with folders

Next: build the first REST microservice.
