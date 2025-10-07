# 12 — Docker & Kubernetes

Containerize services and the gateway; run with Compose or Kubernetes.

## Dockerfiles
Create a Dockerfile per service similar to:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src/AccountsApi/*.csproj ./src/AccountsApi/
RUN dotnet restore ./src/AccountsApi/AccountsApi.csproj
COPY . .
RUN dotnet publish ./src/AccountsApi/AccountsApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "AccountsApi.dll"]
```

## Docker Compose
- Define services for Accounts, Products, Gateway
- Link networks and environment variables

## Kubernetes (outline)
- Deployment and Service per component
- Ingress for gateway
- ConfigMaps/Secrets for configuration
