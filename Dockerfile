# Base stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY src/RoadmapCreationAssistance.API/RoadmapCreationAssistance.API.csproj RoadmapCreationAssistance.API/
RUN dotnet restore RoadmapCreationAssistance.API/RoadmapCreationAssistance.API.csproj

# Copy source code and build
COPY src/RoadmapCreationAssistance.API/ RoadmapCreationAssistance.API/
WORKDIR /src/RoadmapCreationAssistance.API
RUN dotnet build -c Release --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install Datadog serverless-init and .NET tracer
COPY --from=datadog/serverless-init:1-alpine /datadog-init /app/datadog-init
COPY --from=datadog/dd-lib-dotnet-init /datadog-init/monitoring-home/ /dd_tracer/dotnet/
ENV DD_SERVICE=roadmap-creation-assistance

# Health check (wget is available in Alpine by default, curl is not)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --spider --quiet http://localhost:8080/health || exit 1

ENTRYPOINT ["/app/datadog-init"]
CMD ["dotnet", "RoadmapCreationAssistance.API.dll"]