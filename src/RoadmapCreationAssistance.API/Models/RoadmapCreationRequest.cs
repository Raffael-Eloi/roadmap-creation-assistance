using System.ComponentModel.DataAnnotations;

namespace RoadmapCreationAssistance.API.Models;

public sealed class RoadmapCreationRequest
{
    public required string GitHubOwner { get; set; }

    public required string GitHubRepositoryName { get; set; }

    public required string GitHubToken { get; set; }

    public required string OpenAIKey { get; set; }

    [AllowedValues(["English-US", "Portuguese-BR"])]
    public string Language { get; set; } = "English-US";
}