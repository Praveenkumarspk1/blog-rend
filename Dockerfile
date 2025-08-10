# Multi-stage build for Blazor WebAssembly application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage for the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["BlogSpace/BlogSpace.sln", "BlogSpace/"]
COPY ["BlogSpace/Server/BlogSpace.Server.csproj", "BlogSpace/Server/"]
COPY ["BlogSpace/Client/BlogSpace.Client.csproj", "BlogSpace/Client/"]
COPY ["BlogSpace/Shared/BlogSpace.Shared.csproj", "BlogSpace/Shared/"]

# Restore dependencies
RUN dotnet restore "BlogSpace/BlogSpace.sln"

# Copy source code
COPY . .
WORKDIR "/src/BlogSpace"

# Build the application
RUN dotnet build "BlogSpace.sln" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BlogSpace.sln" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80;https://+:443

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "BlogSpace.Server.dll"]
