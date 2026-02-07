using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.JsonSerialization;
using RoadmapCreationAssistance.API.Models;
using System.Text.Json;

namespace RoadmapCreationAssistance.API.UseCases;

public class MilestonesAIGenerator(IPromptProvider promptProvider, IOpenAIRepository openAIRepository) : IMilestonesAIGenerator
{
    public async Task<IEnumerable<Milestone>> GenerateWithIssues(RoadmapCreationRequest request)
    {
        string prompt = await promptProvider.GetMilestoneInstructionAsync(request.Language, request.ApiDomainDefinition);

        string response = await openAIRepository.GetResponse(prompt, request.OpenAIKey);

        IEnumerable<Milestone>? milestones = JsonSerializer.Deserialize<IEnumerable<Milestone>>(response, JsonSerializationOptions.Default);

        if (milestones is null)
            throw new InvalidOperationException("Failed to deserialize milestones from AI response.");

        return milestones;
    }
}