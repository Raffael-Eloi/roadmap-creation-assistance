using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.UseCases;

public class RoadmapCreator(IMilestonesAIGenerator milestonesAIGenerator, IGithubRepository githubRepository) : IRoadmapCreator
{
    public async Task CreateAsync(RoadmapCreationRequest request)
    {
        await CreateLabels(request);

        IEnumerable<Milestone> milestones = await CreateMilestones(request);

        IEnumerable<Issue> issues = await CreateIssues(request, milestones);
        
        Project project = await CreateProject(request);

        await githubRepository.LinkIssuesToProject(project, issues, request);
    }

    private async Task CreateLabels(RoadmapCreationRequest request)
    {
        IEnumerable<Label> labels = Label.GenerateDefaultLabels();

        await githubRepository.CreateLabels(labels, request);
    }

    private async Task<IEnumerable<Milestone>> CreateMilestones(RoadmapCreationRequest request)
    {
        IEnumerable<Milestone> milestones = await milestonesAIGenerator.GenerateWithIssues(request);

        await githubRepository.CreateMilestones(milestones, request);

        PopulateMilestoneIdOnIssues(milestones);

        return milestones;
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

    private async Task<IEnumerable<Issue>> CreateIssues(RoadmapCreationRequest request, IEnumerable<Milestone> milestones)
    {
        IEnumerable<Issue> issues = [.. milestones.SelectMany(milestone => milestone.Issues)];

        await githubRepository.CreateIssues(issues, request);
        return issues;
    }

    private async Task<Project> CreateProject(RoadmapCreationRequest request)
    {
        Project project = new()
        {
            Title = "Roadmap - Software Engineer"
        };

        await githubRepository.CreateProject(project, request);
        return project;
    }
}