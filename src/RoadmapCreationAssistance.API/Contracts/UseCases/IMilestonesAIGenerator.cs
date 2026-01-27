using RoadmapCreationAssistance.API.Entities;

namespace RoadmapCreationAssistance.API.Contracts.UseCases;

public interface IMilestonesAIGenerator
{
    Task<IEnumerable<Milestone>> GenerateWithIssues();
}