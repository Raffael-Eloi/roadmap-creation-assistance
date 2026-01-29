using RoadmapCreationAssistance.API.Entities;

namespace RoadmapCreationAssistance.API.Contracts.Repositories;

public interface IGithubRepository
{
    Task CreateMilestones(IEnumerable<Milestone> milestones);

    Task CreateIssues(IEnumerable<Issue> issues);
}