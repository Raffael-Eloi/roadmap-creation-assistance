namespace RoadmapCreationAssistance.API.Entities;

public sealed class Project
{
    public const string DefaultTitle = "Roadmap - Software Engineer";

    public string? Id { get; set; }

    public required string Title { get; set; }
}