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

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "RoadmapCreationAssistance.API.dll"]