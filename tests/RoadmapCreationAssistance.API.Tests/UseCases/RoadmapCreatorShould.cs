using Moq;
using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.Models;
using RoadmapCreationAssistance.API.UseCases;

namespace RoadmapCreationAssistance.API.Tests.UseCases;

public class RoadmapCreatorShould
{
    private Mock<IMilestonesAIGenerator> milestonesAiGeneratorMock;
    private Mock<IReadmeAIGenerator> readmeAiGeneratorMock;
    private Mock<IGithubRepository> githubRepositoryMock;
    private IRoadmapCreator roadmapCreator;
    private RoadmapCreationRequest request;

    [SetUp]
    public void Setup()
    {
        milestonesAiGeneratorMock = new Mock<IMilestonesAIGenerator>();
        readmeAiGeneratorMock = new Mock<IReadmeAIGenerator>();
        githubRepositoryMock = new Mock<IGithubRepository>();
        roadmapCreator = new RoadmapCreator(milestonesAiGeneratorMock.Object, readmeAiGeneratorMock.Object, githubRepositoryMock.Object);

        request = new RoadmapCreationRequest
        {
            GitHubOwner = "John",
            GitHubRepositoryName = "My repo",
            GitHubToken = "MYTOKEN",
            OpenAIKey = "MYOPENAIKEY"
        };
    }

    [Test]
    public async Task Create_Labels_On_Github()
    {
        #region Arrange

        #endregion

        #region Act

        await roadmapCreator.CreateAsync(request);

        #endregion

        #region Assert

        Label expectedTechLabel = new()
        {
            Name = "TECH",
            Description = "Technical implementation",
            Color = "416BB8"
        };

        Label expectedMeLabel = new()
        {
            Name = "ME",
            Description = "Mindset Evolution — Reflection and reasoning",
            Color = "33D631"
        };

        Label expectedHandsOnLabel = new()
        {
            Name = "HO",
            Description = "Hands-On — Small practical challenges",
            Color = "FFE638"
        };

        githubRepositoryMock
            .Verify(githubRepo =>
                githubRepo.CreateLabels(It.Is<IEnumerable<Label>>(labelsToBeCreated =>
                    labelsToBeCreated.Any(label => label.Name == expectedTechLabel.Name && label.Description == expectedTechLabel.Description && label.Color == expectedTechLabel.Color) &&
                    labelsToBeCreated.Any(label => label.Name == expectedMeLabel.Name && label.Description == expectedMeLabel.Description && label.Color == expectedMeLabel.Color) &&
                    labelsToBeCreated.Any(label => label.Name == expectedHandsOnLabel.Name && label.Description == expectedHandsOnLabel.Description && label.Color == expectedHandsOnLabel.Color)), request),
            Times.Once);

        #endregion
    }

    [Test]
    public async Task Create_Milestones_On_Github()
    {
        #region Arrange

        Milestone milestone1 = new()
        {
            Title = "My title 1",
            Description = "My description 1",
            Issues =
            [
                new Issue()
                {
                    Title = "My issue 1 from milestone 1"
                },
                new Issue()
                {
                    Title = "My issue 2 from milestone 1"
                }
            ]
        };

        Milestone milestone2 = new()
        {
            Title = "My title 2",
            Description = "My description 2",
            Issues =
            [
                new Issue()
                {
                    Title = "My issue 1 from milestone 2"
                }
            ]
        };

        List<Milestone> milestones = [milestone1, milestone2];

        milestonesAiGeneratorMock
            .Setup(milestonesGenerator => milestonesGenerator.GenerateWithIssues(request))
            .ReturnsAsync([milestone1, milestone2]);

        #endregion

        #region Act

        await roadmapCreator.CreateAsync(request);

        #endregion

        #region Assert

        githubRepositoryMock
            .Verify(githubRepo => githubRepo.CreateMilestones(milestones, request),
            Times.Once);

        #endregion
    }

    [Test]
    public async Task Create_Issues_On_Github()
    {
        #region Arrange

        Milestone milestone1 = new()
        {
            Title = "My title 1",
            Description = "My description 1",
            Issues =
            [
                new Issue()
                {
                    Title = "My issue 1 from milestone 1"
                },
                new Issue()
                {
                    Title = "My issue 2 from milestone 1"
                }
            ]
        };

        Milestone milestone2 = new()
        {
            Title = "My title 2",
            Description = "My description 2",
            Issues =
            [
                new Issue()
                {
                    Title = "My issue 1 from milestone 2"
                }
            ]
        };

        List<Milestone> milestones = [milestone1, milestone2];

        int milestone1Id = 1010;

        int milestone2Id = 2020;

        milestonesAiGeneratorMock
            .Setup(milestonesGenerator => milestonesGenerator.GenerateWithIssues(request))
            .Callback(() =>
            {
                milestone1.Id = milestone1Id;
                milestone2.Id = milestone2Id;
            })
            .ReturnsAsync([milestone1, milestone2]);

        #endregion

        #region Act

        await roadmapCreator.CreateAsync(request);

        #endregion

        #region Assert

        githubRepositoryMock
            .Verify(githubRepo => 
                githubRepo.CreateIssues(It.Is<IEnumerable<Issue>>(issuesToBeCreated => 
                    issuesToBeCreated.Any(issue => issue.Title == milestone1.Issues.First().Title && issue.Milestone == milestone1Id && issue.Assignee == request.GitHubOwner) &&
                    issuesToBeCreated.Any(issue => issue.Title == milestone1.Issues.Last().Title && issue.Milestone == milestone1Id && issue.Assignee == request.GitHubOwner) &&
                    issuesToBeCreated.Any(issue => issue.Title == milestone2.Issues.First().Title && issue.Milestone == milestone2Id && issue.Assignee == request.GitHubOwner)), request),
            Times.Once);

        #endregion
    }

    [Test]
    public async Task Do_Not_Create_Issues_Without_Milestones_On_Github()
    {
        #region Arrange

        Milestone milestone1 = new()
        {
            Title = "My title 1",
            Description = "My description 1",
            Issues =
            [
                new Issue()
                {
                    Title = "My issue 1 from milestone 1"
                },
                new Issue()
                {
                    Title = "My issue 2 from milestone 1"
                }
            ]
        };

        Milestone milestone2 = new()
        {
            Title = "My title 2",
            Description = "My description 2",
            Issues =
            [
                new Issue()
                {
                    Title = "My issue 1 from milestone 2"
                }
            ]
        };

        List<Milestone> milestones = [milestone1, milestone2];

        int? unexistingMilestone1Id = null;

        int? unexistingMilestone2Id = null;

        milestonesAiGeneratorMock
            .Setup(milestonesGenerator => milestonesGenerator.GenerateWithIssues(request))
            .Callback(() =>
            {
                milestone1.Id = unexistingMilestone1Id;
                milestone2.Id = unexistingMilestone2Id;
            })
            .ReturnsAsync([milestone1, milestone2]);

        #endregion

        #region Act

        await roadmapCreator.CreateAsync(request);

        #endregion

        #region Assert

        githubRepositoryMock
            .Verify(githubRepo => 
                githubRepo.CreateIssues(It.IsAny<IEnumerable<Issue>>(), request),
            Times.Never);

        #endregion
    }

    [Test]
    public async Task Create_A_Project_On_Github()
    {
        #region Arrange

        #endregion

        #region Act

        await roadmapCreator.CreateAsync(request);

        #endregion

        #region Assert

        string expectedTitle = "Roadmap - Software Engineer";

        githubRepositoryMock
            .Verify(githubRepo => 
                githubRepo.CreateProject(It.Is<Project>(project => project.Title == expectedTitle), request),
            Times.Once);

        #endregion
    }

    [Test]
    public async Task Link_Issues_To_The_Project_On_Github()
    {
        #region Arrange

        Milestone milestone = new()
        {
            Title = "My title 1",
            Description = "My description 1",
            Issues =
            [
                new Issue()
                {
                    Title = "My issue 1 from milestone 1"
                },
                new Issue()
                {
                    Title = "My issue 2 from milestone 1"
                }
            ]
        };

        milestonesAiGeneratorMock
            .Setup(milestonesGenerator => milestonesGenerator.GenerateWithIssues(request))
            .ReturnsAsync([milestone]);

        milestonesAiGeneratorMock
            .Setup(milestonesGenerator => milestonesGenerator.GenerateWithIssues(request))
            .Callback(() =>
            {
                milestone.Id = 1010;
            })
            .ReturnsAsync([milestone]);

        #endregion

        #region Act

        await roadmapCreator.CreateAsync(request);

        #endregion

        #region Assert

        string expectedTitle = "Roadmap - Software Engineer";

        githubRepositoryMock
            .Verify(githubRepo => 
                githubRepo.LinkIssuesToProject(
                    It.Is<Project>(project => project.Title == expectedTitle),
                    It.Is<IEnumerable<Issue>>(issues => 
                        issues.Any(issue => issue.Title == milestone.Issues.First().Title) && 
                        issues.Any(issue => issue.Title == milestone.Issues.Last().Title)),
                    request),
            Times.Once);

        #endregion
    }

    [Test]
    public async Task Create_Readme_On_Github()
    {
        #region Arrange

        string readme = "# Roadmap\n\nThis is a sample roadmap.";

        readmeAiGeneratorMock
            .Setup(readmeGenerator => readmeGenerator.GenerateAsync(request))
            .ReturnsAsync(readme);

        #endregion

        #region Act

        await roadmapCreator.CreateAsync(request);

        #endregion

        #region Assert

        githubRepositoryMock
            .Verify(githubRepo => githubRepo.CreateReadme(readme, request),
            Times.Once);

        #endregion
    }
}