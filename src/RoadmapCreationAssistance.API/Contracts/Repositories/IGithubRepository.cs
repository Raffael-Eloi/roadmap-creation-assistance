using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.Contracts.Repositories;

public interface IGithubRepository
{
    Task CreateMilestones(IEnumerable<Milestone> milestones, RoadmapCreationRequest request);

    Task CreateIssues(IEnumerable<Issue> issues, RoadmapCreationRequest request);

    Task CreateLabels(IEnumerable<Label> labels, RoadmapCreationRequest request);

    Task CreateProject(Project project, RoadmapCreationRequest request);

    Task LinkIssuesToProject(Project project, IEnumerable<Issue> issues, RoadmapCreationRequest request);
}