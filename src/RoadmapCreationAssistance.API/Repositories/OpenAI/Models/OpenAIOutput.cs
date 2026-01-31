
namespace RoadmapCreationAssistance.API.Repositories.OpenAI.Models;

internal sealed class OpenAIOutput
{
    public string Type { get; set; } = string.Empty;

    public List<OpenAIContent> Content { get; set; } = [];
}