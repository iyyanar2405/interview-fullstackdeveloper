# 04 — Queries, Filtering, Sorting, Pagination

Use HotChocolate.Data for filtering, sorting, projection, and pagination.

## Setup
```powershell
 dotnet add src/GraphApi package HotChocolate.Data
 dotnet add src/GraphApi package HotChocolate.Data.Filters
 dotnet add src/GraphApi package HotChocolate.Data.Sorting
```

```csharp
builder.Services
  .AddGraphQLServer()
  .AddQueryType<Query>()
  .AddFiltering()
  .AddSorting()
  .AddProjections();
```

## Filtering and sorting
```csharp
public class Query
{
    [UseFiltering]
    [UseSorting]
    public IQueryable<Book> GetBooks([Service] BooksService svc) => svc.All().AsQueryable();
}
```

Query sample:
```graphql
query {
  books(where: { title: { contains: "C#" } }, order: { title: ASC }) {
    id title author
  }
}
```

## Projection to DTO
```csharp
public record BookDto(string Title);

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Book> GetBooks([Service] BooksService svc) => svc.All().AsQueryable();
}
```

## Pagination (Cursor-based)
```csharp
builder.Services.AddGraphQLServer().SetPagingOptions(new HotChocolate.Types.Pagination.PagingOptions
{
    DefaultPageSize = 10,
    MaxPageSize = 50
});

public class Query
{
    [UsePaging(IncludeTotalCount = true)]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Book> GetBooks([Service] BooksService svc) => svc.All().AsQueryable();
}
```

Next: Mutations and input validation.
