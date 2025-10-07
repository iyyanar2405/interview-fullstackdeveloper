# 12 — Docker & Deployment

Containerize the GraphQL API and deploy.

## Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src/GraphApi/*.csproj ./src/GraphApi/
RUN dotnet restore ./src/GraphApi/GraphApi.csproj
COPY . .
RUN dotnet publish ./src/GraphApi/GraphApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "GraphApi.dll"]
```

Build & run:
```powershell
 docker build -t graphapi:latest .
 docker run -it --rm -p 8080:8080 graphapi:latest
```

## Hosting tips
- Prefer console logging for containers
- Externalize config via environment variables
- Expose `/graphql` and any tooling endpoints (e.g., `/voyager`)
