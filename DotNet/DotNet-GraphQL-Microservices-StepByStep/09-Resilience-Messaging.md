# 09 — Resilience & Messaging

Improve reliability with policies and integrate events.

## Resilience with Polly
```powershell
 dotnet add src/* package Polly.Extensions.Http
```

```csharp
builder.Services.AddHttpClient("products")
    .AddPolicyHandler(Polly.Extensions.Http.HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(r => (int)r.StatusCode == 429)
        .WaitAndRetryAsync(3, retry => TimeSpan.FromMilliseconds(200 * retry)));
```

## Messaging events
- Emit domain events (e.g., ProductCreated)
- Use Kafka/RabbitMQ for inter-service communication
- Apply outbox pattern to ensure reliable publish

Next: Testing contracts and e2e via the gateway.
