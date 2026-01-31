namespace RoadmapCreationAssistance.API.Repositories.OpenAI.Models;

internal sealed class OpenAIInputRequest(string input)
{
    public string Model => "gpt-5-nano-2025-08-07";

    public string Input { get; set; } = input;
}