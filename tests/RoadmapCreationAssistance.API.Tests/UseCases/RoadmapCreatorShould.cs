using Moq;
using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.UseCases;

namespace RoadmapCreationAssistance.API.Tests.UseCases;

public class RoadmapCreatorShould
{
    [Test]
    public async Task Create_Milestones_On_Github()
    {
        #region Arrange

        var milestonesAiGeneratorMock = new Mock<IMilestonesAIGenerator>();

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

        var githubRepositoryMock = new Mock<IGithubRepository>();

        IRoadmapCreator roadmapCreator = new RoadmapCreator(milestonesAiGeneratorMock.Object, githubRepositoryMock.Object);

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
}