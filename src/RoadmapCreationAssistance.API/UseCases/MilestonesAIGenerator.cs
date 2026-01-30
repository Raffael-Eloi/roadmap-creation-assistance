using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.Models;
using System.Text.Json;

namespace RoadmapCreationAssistance.API.UseCases;

public class MilestonesAIGenerator(IOpenAIRepository openAIRepository) : IMilestonesAIGenerator
{
    public async Task<IEnumerable<Milestone>> GenerateWithIssues(RoadmapCreationRequest request)
    {
        string prompt = await GetRoadmapPrompt();

        string response = await openAIRepository.GetResponse(prompt, request.OpenAIKey);

        IEnumerable<Milestone>? milestones = JsonSerializer.Deserialize<IEnumerable<Milestone>>(response);

        if (milestones is null)
            return [];

        return milestones;
    }

    private static async Task<string> GetRoadmapPrompt()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string promptPath = Path.Combine(currentDirectory, "PromptBase", "roadmap_base.md");
        return await File.ReadAllTextAsync(promptPath);
    }
}