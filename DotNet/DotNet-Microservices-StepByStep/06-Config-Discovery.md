# 06 — Config & Discovery

Manage configuration, environments, and discover services.

## Configuration sources
- appsettings.json + environment overrides
- Environment variables for containerized deployments
- Secret stores (User Secrets, Azure Key Vault)

## Options pattern
```csharp
builder.Services.Configure<MyOptions>(builder.Configuration.GetSection("MyOptions"));
```

## Service discovery approaches
- Static config (dev): YARP LoadFromMemory
- Service registry (prod): Kubernetes services, Consul, Eureka
- DNS-based discovery in k8s (svc-name.namespace.svc.cluster.local)

## Health checks
Expose `/health` in each service; gateway can probe before routing.

Next: Resilience with HttpClientFactory + Polly.
