# 06 — Subscriptions

Real-time updates using WebSockets or SSE with Hot Chocolate.

## Setup
```powershell
 dotnet add src/GraphApi package HotChocolate.Subscriptions
```

```csharp
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddInMemorySubscriptions();

var app = builder.Build();
app.MapGraphQL();
app.UseWebSockets();
app.Run();

public class Subscription
{
    [Subscribe]
    [Topic]
    public Book OnBookAdded([EventMessage] Book book) => book;
}

public class Mutation
{
    public async Task<AddBookPayload> AddBook([Service] BooksService svc, [Service] ITopicEventSender sender, AddBookInput input)
    {
        var book = svc.Add(input.Title, input.Author);
        await sender.SendAsync(nameof(Subscription.OnBookAdded), book);
        return new AddBookPayload(book);
    }
}
```

Client subscription:
```graphql
subscription {
  onBookAdded { id title author }
}
```

Next: EF Core integration and DataLoaders to avoid N+1.
