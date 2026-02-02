using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.UseCases;

public class ReadmeAIGenerator() : IReadmeAIGenerator
{
    public Task<string> GenerateAsync(RoadmapCreationRequest creationRequest)
    {
        throw new NotImplementedException();
    }
}