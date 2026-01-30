namespace RoadmapCreationAssistance.API.Contracts.Repositories;

public interface IOpenAIRepository
{
    Task<string> GetResponse(string input, string openAiToken);
}