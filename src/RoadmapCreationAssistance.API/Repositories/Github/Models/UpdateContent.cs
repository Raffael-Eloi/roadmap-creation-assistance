namespace RoadmapCreationAssistance.API.Repositories.Github.Models;

public sealed class UpdateContent
{
    public required string Message { get; set; }

    public required string Content { get; set; }
}