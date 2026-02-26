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

COPY --from=datadog/serverless-init:1-alpine /datadog-init /app/datadog-init
COPY --from=datadog/dd-lib-dotnet-init /datadog-init/monitoring-home/ /dd_tracer/dotnet/
ENV DD_SERVICE=roadmap-creation-assistance

# Health check (wget is available in Alpine by default, curl is not)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --spider --quiet http://localhost:8080/health || exit 1

# Debug: capture datadog-init output before it crashes
RUN printf '#!/bin/sh\necho "=== DEBUG: Starting datadog-init ==="\necho "=== ENV ==="\nenv | grep -E "^DD_|^LD_|^CORECLR" | sort\necho "=== datadog-init file info ==="\nfile /app/datadog-init 2>/dev/null || echo "file command not available"\nls -la /app/datadog-init\necho "=== Attempting to run datadog-init ==="\n/app/datadog-init "$@" 2>&1\nEXIT_CODE=$?\necho "=== datadog-init exited with code: $EXIT_CODE ==="\nexit $EXIT_CODE\n' > /app/debug-entrypoint.sh && chmod +x /app/debug-entrypoint.sh

ENTRYPOINT ["/app/debug-entrypoint.sh"]
CMD ["dotnet", "RoadmapCreationAssistance.API.dll"]