# Base stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
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

# Install Datadog .NET tracer for APM auto-instrumentation
ARG DD_TRACER_VERSION=3.37.0
ADD https://github.com/DataDog/dd-trace-dotnet/releases/download/v${DD_TRACER_VERSION}/datadog-dotnet-apm-${DD_TRACER_VERSION}.tar.gz /opt/datadog/
RUN cd /opt/datadog && tar -xzf datadog-dotnet-apm-${DD_TRACER_VERSION}.tar.gz && rm datadog-dotnet-apm-${DD_TRACER_VERSION}.tar.gz

ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_DOTNET_TRACER_HOME=/opt/datadog
ENV DD_SERVICE=roadmap-creation-assistance
ENV DD_AGENT_HOST=localhost

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "RoadmapCreationAssistance.API.dll"]