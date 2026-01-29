using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.Contracts.UseCases;

public interface IRoadmapCreator
{
    Task CreateAsync(RoadmapCreationRequest request);
}