using System.ComponentModel.DataAnnotations;

namespace RoadmapCreationAssistance.API.Models;

public sealed class RoadmapCreationRequest
{
    [Required(AllowEmptyStrings = false)]
    [MinLength(5)]
    [MaxLength(100)]
    public required string GitHubOwner { get; set; }

    [Required(AllowEmptyStrings = false)]
    [MinLength(5)]
    [MaxLength(150)]
    public required string GitHubRepositoryName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [MinLength(5)]
    [MaxLength(60)]
    public required string GitHubToken { get; set; }

    [Required(AllowEmptyStrings = false)]
    [MinLength(5)]
    [MaxLength(250)]
    public required string OpenAIKey { get; set; }

    [AllowedValues(["English-US", "Portuguese-BR"])]
    public string Language { get; set; } = "English-US";

    [Required(AllowEmptyStrings = false)]
    [MinLength(10)]
    [MaxLength(250)]
    public required string ApiDomainDefinition { get; set; }
}