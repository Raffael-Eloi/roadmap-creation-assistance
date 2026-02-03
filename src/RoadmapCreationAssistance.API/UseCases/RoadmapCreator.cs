using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.UseCases;

public class RoadmapCreator(IMilestonesAIGenerator milestonesAIGenerator, IReadmeAIGenerator readmeAIGenerator, IGithubRepository githubRepository) : IRoadmapCreator
{
    public async Task<RoadmapCreationResponse> CreateAsync(RoadmapCreationRequest request)
    {
        await CreateLabels(request);

        IEnumerable<Milestone> milestones = await CreateMilestones(request);

        IEnumerable<Issue> issues = await CreateIssues(request, milestones);

        Project project = await CreateProject(request);

        await githubRepository.LinkIssuesToProject(project, issues, request);

        await CreateReadme(request);
        
        return SuccessfulResponse(milestones, issues, project);
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

        PopulateMilestoneIdAndAssigneeOnIssues(milestones, request);

        return milestones;
    }

    private static void PopulateMilestoneIdAndAssigneeOnIssues(IEnumerable<Milestone> milestones, RoadmapCreationRequest request)
    {
        foreach (Milestone milestone in milestones)
        {
            foreach (Issue issue in milestone.Issues)
            {
                issue.Milestone = milestone.Id;
                issue.Assignee = request.GitHubOwner;
            }
        }
    }

    private async Task<IEnumerable<Issue>> CreateIssues(RoadmapCreationRequest request, IEnumerable<Milestone> milestones)
    {
        List<Issue> issues = [.. milestones.SelectMany(milestone => milestone.Issues)];
        if (issues.Count == 0)
        {
            return issues;
        }

        await githubRepository.CreateIssues(issues, request);
        return issues;
    }

    private async Task<Project> CreateProject(RoadmapCreationRequest request)
    {
        Project project = new()
        {
            Title = Project.DefaultTitle
        };

        await githubRepository.CreateProject(project, request);
        return project;
    }

    private async Task CreateReadme(RoadmapCreationRequest request)
    {
        string readme = await readmeAIGenerator.GenerateAsync(request);

        await githubRepository.CreateReadme(readme, request);
    }

    private static RoadmapCreationResponse SuccessfulResponse(IEnumerable<Milestone> milestones, IEnumerable<Issue> issues, Project project)
    {
        return new RoadmapCreationResponse
        {
            ProjectId = project.Id!,
            MilestonesCreatedCount = milestones.Count(),
            IssuesCreatedCount = issues.Count(),
            ReadmeCreated = true
        };
    }
}