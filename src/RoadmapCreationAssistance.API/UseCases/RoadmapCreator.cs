using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.UseCases;

public class RoadmapCreator(IMilestonesAIGenerator milestonesAIGenerator, IGithubRepository githubRepository) : IRoadmapCreator
{
    public async Task CreateAsync(RoadmapCreationRequest request)
    {
        IEnumerable<Label> labels = GenerateDefaultLabels();

        await githubRepository.CreateLabels(labels, request);

        IEnumerable<Milestone> milestones = await milestonesAIGenerator.GenerateWithIssues(request);

        await githubRepository.CreateMilestones(milestones, request);
        
        PopulateMilestoneIdOnIssues(milestones);

        IEnumerable<Issue> issues = [.. milestones.SelectMany(milestone => milestone.Issues)];

        await githubRepository.CreateIssues(issues, request);

        Project project = new()
        {
            Title = "Roadmap - Software Engineer"
        };

        await githubRepository.CreateProject(project, request);

        await githubRepository.LinkIssuesToProject(project, issues, request);
    }

    private static IEnumerable<Label> GenerateDefaultLabels()
    {
        return
        [
            new Label()
            {
                Name = "TECH",
                Description = "Technical implementation",
                Color = "416BB8"
            },
            new Label()
            {
                Name = "ME",
                Description = "Mindset Evolution — Reflection and reasoning",
                Color = "33D631"
            },
            new Label()
            {
                Name = "HO",
                Description = "Larger practical challenges",
                Color = "FFE638"
            },
        ];
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