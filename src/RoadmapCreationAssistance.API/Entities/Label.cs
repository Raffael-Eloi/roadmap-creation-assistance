namespace RoadmapCreationAssistance.API.Entities;

public class Label
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public string? Color { get; set; }
}