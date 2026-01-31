namespace RoadmapCreationAssistance.API.Repositories.OpenAI.Models;

internal sealed class OpenAIResponse
{
    public string Status { get; set; } = string.Empty;

    public List<OpenAIOutput> Output { get; set; } = [];
}