using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;

namespace RoadmapCreationAssistance.API.UseCases;

public class RoadmapCreator(IMilestonesAIGenerator milestonesAIGenerator, IGithubRepository githubRepository) : IRoadmapCreator
{
    public async Task CreateAsync()
    {
        IEnumerable<Milestone> milestones = await milestonesAIGenerator.GenerateWithIssues();

        await githubRepository.CreateMilestones(milestones);
    }
}