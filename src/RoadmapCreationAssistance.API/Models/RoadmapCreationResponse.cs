namespace RoadmapCreationAssistance.API.Models;

public sealed record RoadmapCreationResponse
{
    public required string ProjectId { get; set; }

    public required int MilestonesCreatedCount { get; set; }

    public required int IssuesCreatedCount { get; set; }

    public required bool ReadmeCreated { get; set; }
}