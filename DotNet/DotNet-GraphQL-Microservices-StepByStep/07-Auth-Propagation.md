# 07 — Auth Propagation

Forward caller identity through the gateway to downstream services.

## JWT at the gateway
```csharp
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", o =>
{
    o.Authority = builder.Configuration["Jwt:Authority"];
    o.Audience = builder.Configuration["Jwt:Audience"];
    o.RequireHttpsMetadata = false; // dev
});

builder.Services.AddAuthorization();
```

## Forward headers to subgraphs
```csharp
builder.Services.AddHttpClient("accounts").AddHeaderPropagation();

builder.Services.AddHeaderPropagation(o => o.Headers.Add("Authorization"));

app.UseHeaderPropagation();
```

In Fusion, configure HTTP clients used for each remote schema to include Authorization from the incoming request.

Next: DataLoader per service and avoiding N+1.
