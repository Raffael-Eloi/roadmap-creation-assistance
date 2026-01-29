using Moq;
using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.UseCases;

namespace RoadmapCreationAssistance.API.Tests.UseCases;

public class RoadmapCreatorShould
{
    private Mock<IMilestonesAIGenerator> milestonesAiGeneratorMock;
    private Mock<IGithubRepository> githubRepositoryMock;
    private IRoadmapCreator roadmapCreator;

    [SetUp]
    public void Setup()
    {
        milestonesAiGeneratorMock = new Mock<IMilestonesAIGenerator>();
        githubRepositoryMock = new Mock<IGithubRepository>();
        roadmapCreator = new RoadmapCreator(milestonesAiGeneratorMock.Object, githubRepositoryMock.Object);
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
            .Setup(milestonesGenerator => milestonesGenerator.GenerateWithIssues())
            .ReturnsAsync([milestone1, milestone2]);

        #endregion

        #region Act

        await roadmapCreator.CreateAsync();

        #endregion

        #region Assert

        githubRepositoryMock
            .Verify(githubRepo => githubRepo.CreateMilestones(milestones),
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
            .Setup(milestonesGenerator => milestonesGenerator.GenerateWithIssues())
            .Callback(() =>
            {
                milestone1.Id = milestone1Id;
                milestone2.Id = milestone2Id;
            })
            .ReturnsAsync([milestone1, milestone2]);

        #endregion

        #region Act

        await roadmapCreator.CreateAsync();

        #endregion

        #region Assert

        githubRepositoryMock
            .Verify(githubRepo => 
                githubRepo.CreateIssues(It.Is<IEnumerable<Issue>>(issuesToBeCreated => 
                    issuesToBeCreated.Any(issue => issue.Title == milestone1.Issues.First().Title && issue.Milestone == milestone1Id) &&
                    issuesToBeCreated.Any(issue => issue.Title == milestone1.Issues.Last().Title && issue.Milestone == milestone1Id) &&
                    issuesToBeCreated.Any(issue => issue.Title == milestone2.Issues.First().Title && issue.Milestone == milestone2Id))),
            Times.Once);

        #endregion
    }
}