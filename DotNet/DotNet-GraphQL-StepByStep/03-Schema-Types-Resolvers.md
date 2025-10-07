# 03 — Schema, Types, Resolvers

Model your schema with object types, inputs, and resolvers.

## Object types and resolvers
```csharp
public record Book(int Id, string Title, string Author);

public class Query
{
    public IEnumerable<Book> GetBooks() => new[]
    {
        new Book(1, "DDD", "Evans"),
        new Book(2, "CLR via C#", "Richter")
    };
}

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddType<BookType>();

var app = builder.Build();
app.MapGraphQL();
app.Run();

public class BookType : ObjectType<Book>
{
    protected override void Configure(IObjectTypeDescriptor<Book> d)
    {
        d.Field(f => f.Id).Type<NonNullType<IdType>>();
        d.Field(f => f.Title).Type<NonNullType<StringType>>();
        d.Field(f => f.Author).Type<NonNullType<StringType>>();
    }
}
```

## Input types and mutations
```csharp
public record AddBookInput(string Title, string Author);
public record AddBookPayload(Book Book);

public class Mutation
{
    private static readonly List<Book> Books = new();

    public AddBookPayload AddBook(AddBookInput input)
    {
        var book = new Book(Books.Count + 1, input.Title, input.Author);
        Books.Add(book);
        return new AddBookPayload(book);
    }
}

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();
```

Mutation example:
```graphql
mutation($input: AddBookInput!) {
  addBook(input: $input) {
    book { id title author }
  }
}
```

Variables:
```json
{ "input": { "title": "Clean Code", "author": "Martin" } }
```

## Field resolvers and dependencies
```csharp
public class BooksService
{
    private readonly List<Book> _books = new()
    {
        new(1, "DDD", "Evans"),
        new(2, "CLR via C#", "Richter")
    };
    public IEnumerable<Book> All() => _books;
    public Book Add(string title, string author)
    {
        var b = new Book(_books.Count + 1, title, author);
        _books.Add(b);
        return b;
    }
}

builder.Services.AddSingleton<BooksService>();

public class Query
{
    public IEnumerable<Book> GetBooks([Service] BooksService svc) => svc.All();
}

public class Mutation
{
    public AddBookPayload AddBook([Service] BooksService svc, AddBookInput input)
        => new(svc.Add(input.Title, input.Author));
}
```

Next: filtering, sorting, projection, and pagination.
