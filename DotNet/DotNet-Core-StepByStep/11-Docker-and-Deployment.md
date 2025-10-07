# 11 — Docker and Deployment

Containerize your API and deploy to common targets.

## Dockerfile (SDK -> runtime)
```dockerfile
# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src/MyApi/*.csproj ./src/MyApi/
RUN dotnet restore ./src/MyApi/MyApi.csproj
COPY . .
RUN dotnet publish ./src/MyApi/MyApi.csproj -c Release -o /app/publish /p:UseAppHost=false

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "MyApi.dll"]
```

Build and run:
```powershell
 docker build -t myapi:latest .
 docker run -it --rm -p 8080:8080 myapi:latest
```

## IIS (Windows) hosting
- Publish: `dotnet publish -c Release`
- Use IIS ASP.NET Core Module (ANCM) with the Hosting Bundle
- Point site to published folder; configure app pool (No Managed Code)

## Linux hosting (systemd + Nginx)
- Publish self-contained or framework-dependent
- Reverse proxy via Nginx to Kestrel

## Azure App Service
- Create Web App (Linux or Windows)
- Deploy via GitHub Actions or `az webapp up`
- Configure `ASPNETCORE_ENVIRONMENT`, connection strings in App Settings

## Tips
- Use health checks for readiness/liveness
- Log to console for containerized apps
- Externalize configuration via env vars
