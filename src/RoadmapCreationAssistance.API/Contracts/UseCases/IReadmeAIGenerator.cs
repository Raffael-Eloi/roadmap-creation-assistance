using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.Contracts.UseCases;

public interface IReadmeAIGenerator
{
    Task<string> GenerateAsync(RoadmapCreationRequest request);
}