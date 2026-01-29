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
        
        PopulateMilestoneIdOnIssues(milestones);

        IEnumerable<Issue> issues = [.. milestones.SelectMany(milestone => milestone.Issues)];

        await githubRepository.CreateIssues(issues);
    }

    private static void PopulateMilestoneIdOnIssues(IEnumerable<Milestone> milestones)
    {
        foreach (Milestone milestone in milestones)
        {
            foreach (Issue issue in milestone.Issues)
            {
                issue.Milestone = milestone.Id;
            }
        }
    }
}