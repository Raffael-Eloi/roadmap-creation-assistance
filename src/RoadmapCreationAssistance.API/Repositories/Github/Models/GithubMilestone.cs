namespace RoadmapCreationAssistance.API.Repositories.Github.Models;

internal class GithubMilestone
{
    public int? Number { get; set; }

    public required string Title { get; set; }

    public string Description { get; set; } = string.Empty;
}