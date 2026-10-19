# Builds the API image. Build from the repository root, because the API also needs BookKeeper.Core and BookKeeper.Infrastructure:
# docker build -t bookkeeper-api -f BookKeeper.Api/Dockerfile .

# Build stage: the SDK image compiles and publishes the API.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish BookKeeper.Api/BookKeeper.Api.csproj -c Release -o /app/publish

# Runtime stage: a smaller image that only runs the published API on port 8080.
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
# curl is used by the health check in docker-compose.yaml. Installed before the app is copied, so this layer is reused.
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookKeeper.Api.dll"]
