using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.Contracts.UseCases;

public interface IMilestonesAIGenerator
{
    Task<IEnumerable<Milestone>> GenerateWithIssues(RoadmapCreationRequest request);
}