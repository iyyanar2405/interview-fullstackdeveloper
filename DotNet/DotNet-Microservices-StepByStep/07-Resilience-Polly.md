# 07 — Resilience (Polly)

Use HttpClientFactory with Polly for retries, timeouts, and circuit breakers.

## Configure resilient client
```csharp
builder.Services.AddHttpClient("catalog")
    .AddPolicyHandler(Polly.Extensions.Http.HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(r => (int)r.StatusCode == 429)
        .WaitAndRetryAsync(3, retry => TimeSpan.FromMilliseconds(200 * retry)))
    .AddPolicyHandler(Polly.Policy.TimeoutAsync<HttpResponseMessage>(3))
    .AddPolicyHandler(Polly.CircuitBreakerAsync<HttpResponseMessage>(5, TimeSpan.FromSeconds(30)));
```

## Usage
```csharp
var clientFactory = app.Services.GetRequiredService<IHttpClientFactory>();
var client = clientFactory.CreateClient("catalog");
var res = await client.GetAsync("http://localhost:5001/api/v1/items");
```

Next: Messaging and outbox pattern.
