namespace RoadmapCreationAssistance.API.Entities;

public class Milestone
{
    public int Id { get; set; }

    public required string Title { get; init; }

    public string Description { get; init; } = string.Empty;

    public IEnumerable<Issue> Issues { get; init; } = [];
}