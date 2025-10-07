# 05 — Mutations & Inputs

Create mutations with input types and simple validation.

## Input and payload patterns
```csharp
public record AddBookInput(string Title, string Author);
public record AddBookPayload(Book Book);

public class Mutation
{
    public AddBookPayload AddBook([Service] BooksService svc, AddBookInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
            throw new GraphQLException(ErrorBuilder.New().SetMessage("Title is required").SetCode("VALIDATION_ERROR").Build());

        var book = svc.Add(input.Title, input.Author);
        return new AddBookPayload(book);
    }
}

builder.Services.AddGraphQLServer().AddMutationType<Mutation>();
```

## File uploads (optional)
```powershell
 dotnet add src/GraphApi package HotChocolate.AspNetCore
```

```csharp
public class Mutation
{
    public async Task<string> UploadAsync(IFile file, CancellationToken ct)
    {
        using var stream = file.OpenReadStream();
        // save somewhere...
        await Task.CompletedTask;
        return file.Name;
    }
}
```

Next: Subscriptions with in-memory pub/sub.
