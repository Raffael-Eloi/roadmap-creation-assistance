namespace RoadmapCreationAssistance.API.Entities;

public class Issue
{
    public required string Title { get; init; }

    public int? Number { get; set; }

    public string Body { get; init; } = string.Empty;

    public int Milestone { get; set; }

    public List<string> Labels { get; init; } = [];
}