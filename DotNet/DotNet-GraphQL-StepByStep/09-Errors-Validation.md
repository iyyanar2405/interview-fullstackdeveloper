# 09 — Errors & Validation

Model errors, add codes/extensions, and validate inputs.

## Throw GraphQLException with details
```csharp
throw new GraphQLException(
    ErrorBuilder.New()
        .SetMessage("Title must be unique")
        .SetCode("DUPLICATE_TITLE")
        .SetExtension("field", "title")
        .Build());
```

## Map exceptions globally
```csharp
builder.Services
    .AddGraphQLServer()
    .AddErrorFilter(sp => new MyErrorFilter());

public class MyErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
        => error.WithMessage("An unexpected error occurred");
}
```

## Validation options
- Manual guards in mutations
- DataAnnotations on input records
- FluentValidation integration via community packages

```csharp
public record AddBookInput(
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.MinLength(2)]
    string Title,
    string Author);
```

Next: Testing GraphQL endpoints and snapshots.
