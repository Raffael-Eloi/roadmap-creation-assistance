using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.UseCases;

public class ReadmeAIGenerator(IOpenAIRepository openAIRepository) : IReadmeAIGenerator
{
    public async Task<string> GenerateAsync(RoadmapCreationRequest request)
    {
        string prompt = await GetRoadmapPrompt(request);

        return await openAIRepository.GetResponse(prompt, request.OpenAIKey);
    }

    private static async Task<string> GetRoadmapPrompt(RoadmapCreationRequest request)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string promptPath = Path.Combine(currentDirectory, "PromptBase", "roadmap_base.md");
        string prompt = await File.ReadAllTextAsync(promptPath);
        prompt += $"***Given this prompt, I want you to generate a readme in MarkDown***";
        prompt += $"All documentation, milestones, issues and code must be written **in {request.Language}**";
        return prompt;
    }
}