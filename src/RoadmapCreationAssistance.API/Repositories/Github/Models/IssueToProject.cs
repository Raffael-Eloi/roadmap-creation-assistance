namespace RoadmapCreationAssistance.API.Repositories.Github.Models;

internal class IssueToProject
{
    public string Type => "Issue";

    public required string Repo { get; set; }

    public required int Number { get; set; }
}