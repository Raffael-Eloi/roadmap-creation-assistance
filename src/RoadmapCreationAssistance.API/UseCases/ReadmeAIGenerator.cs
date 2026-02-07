using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.UseCases;

public class ReadmeAIGenerator(IPromptProvider promptProvider, IOpenAIRepository openAIRepository) : IReadmeAIGenerator
{
    public async Task<string> GenerateAsync(RoadmapCreationRequest request)
    {
        string prompt = await GetRoadmapPrompt(request.Language, request.ApiDomainDefinition);

        return await openAIRepository.GetResponse(prompt, request.OpenAIKey);
    }

    private async Task<string> GetRoadmapPrompt(string language, string apiDomainDefinition)
    {
        string prompt = await promptProvider.GetRoadmapBaseAsync(language, apiDomainDefinition);
        prompt += $"***Given this prompt, I want you to generate a readme in MarkDown***";
        return prompt;
    }
}