namespace RoadmapCreationAssistance.API.Entities;

public class Milestone
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public IEnumerable<Issue> Issues { get; set; } = [];
}