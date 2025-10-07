# 11 — Containers & Kubernetes

Dockerize services and deploy to k8s.

## Dockerfile (service)
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src/CatalogService/*.csproj ./src/CatalogService/
RUN dotnet restore ./src/CatalogService/CatalogService.csproj
COPY . .
RUN dotnet publish ./src/CatalogService/CatalogService.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CatalogService.dll"]
```

## Compose
- Define services for Catalog, Orders, and Gateway

## Kubernetes
- Deployment + Service per microservice
- Ingress for Gateway
- ConfigMaps/Secrets for configuration

Next: CI/CD pipeline basics.
