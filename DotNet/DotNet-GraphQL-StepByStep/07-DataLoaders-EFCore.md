# 07 — DataLoaders & EF Core

Batch and cache related entity loads and connect to EF Core.

## EF Core context
```csharp
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
}

public record Author(int Id, string Name);
public record Book(int Id, string Title, int AuthorId);
```

Register EF and GraphQL:
```csharp
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("Data Source=app.db"));

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();
```

## DataLoader example
```csharp
public class AuthorByIdDataLoader : BatchDataLoader<int, Author>
{
    private readonly IDbContextFactory<AppDbContext> _dbf;
    public AuthorByIdDataLoader(IBatchScheduler scheduler, IDbContextFactory<AppDbContext> dbf) : base(scheduler)
    {
        _dbf = dbf;
    }
    protected override async Task<IReadOnlyDictionary<int, Author>> LoadBatchAsync(IReadOnlyList<int> keys, CancellationToken ct)
    {
        await using var db = await _dbf.CreateDbContextAsync(ct);
        var authors = await db.Authors.Where(a => keys.Contains(a.Id)).ToListAsync(ct);
        return authors.ToDictionary(a => a.Id);
    }
}

builder.Services.AddPooledDbContextFactory<AppDbContext>(o => o.UseSqlite("Data Source=app.db"));
builder.Services.AddDataLoader<AuthorByIdDataLoader>();
```

Resolve nested fields with DataLoader:
```csharp
public class BookType : ObjectType<Book>
{
    protected override void Configure(IObjectTypeDescriptor<Book> d)
    {
        d.Field("author")
            .Type<NonNullType<ObjectType<Author>>>()
            .Resolve(async (ctx) =>
            {
                var dl = ctx.DataLoader<AuthorByIdDataLoader>();
                var book = ctx.Parent<Book>();
                return await dl.LoadAsync(book.AuthorId, ctx.RequestAborted);
            });
    }
}
```

Next: Authorization at field/object level with JWT.
