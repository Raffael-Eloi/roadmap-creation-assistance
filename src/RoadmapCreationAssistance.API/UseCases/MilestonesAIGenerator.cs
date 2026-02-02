using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.JsonSerialization;
using RoadmapCreationAssistance.API.Models;
using System.Text.Json;

namespace RoadmapCreationAssistance.API.UseCases;

public class MilestonesAIGenerator(IOpenAIRepository openAIRepository) : IMilestonesAIGenerator
{
    public async Task<IEnumerable<Milestone>> GenerateWithIssues(RoadmapCreationRequest request)
    {
        string prompt = await GetRoadmapPrompt(request);

        string response = await openAIRepository.GetResponse(prompt, request.OpenAIKey);

        IEnumerable<Milestone>? milestones = JsonSerializer.Deserialize<IEnumerable<Milestone>>(response, JsonSerializationOptions.Default);

        if (milestones is null)
            throw new InvalidOperationException("Failed to deserialize milestones from AI response.");

        return milestones;
    }

    private static async Task<string> GetRoadmapPrompt(RoadmapCreationRequest request)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string promptPath = Path.Combine(currentDirectory, "PromptBase", "roadmap_base.md");
        string prompt = await File.ReadAllTextAsync(promptPath);

        string milestonesGenerationPromptPath = Path.Combine(currentDirectory, "PromptBase", "milestones_generation_instruction.md");
        string milestonesGenerationPrompt = await File.ReadAllTextAsync(milestonesGenerationPromptPath);

        prompt += Environment.NewLine + milestonesGenerationPrompt + Environment.NewLine;
        prompt += $"All documentation, milestones, issues and code must be written **in {request.Language}**";
        return prompt;
    }
}