namespace RoadmapCreationAssistance.API.Entities;

public class Issue
{
    public required string Title { get; set; }

    public string Body { get; set; } = string.Empty;
}