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
COPY --from=datadog/dd-lib-dotnet-init:latest-musl /datadog-init/monitoring-home/ /dd_tracer/dotnet/

# Datadog .NET APM configuration
ENV DD_SERVICE=roadmap-creation-assistance
ENV DD_TRACE_ENABLED=true
ENV DD_APM_ENABLED=true
ENV DD_APM_NON_LOCAL_TRAFFIC=true
ENV DD_DOTNET_TRACER_HOME=/dd_tracer/dotnet
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/dd_tracer/dotnet/linux-musl-x64/Datadog.Trace.ClrProfiler.Native.so
ENV DD_TRACE_LOG_DIRECTORY=/var/log/datadog/dotnet

# Health check (wget is available in Alpine by default, curl is not)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --spider --quiet http://localhost:8080/health || exit 1

ENTRYPOINT ["/app/datadog-init"]
CMD ["dotnet", "RoadmapCreationAssistance.API.dll"]