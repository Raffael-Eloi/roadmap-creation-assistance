using FluentAssertions;
using Moq;
using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.UseCases;
using System.Text.Json;

namespace RoadmapCreationAssistance.API.Tests.UseCases;

internal class MilestonesAIGeneratorShould
{
    private IMilestonesAIGenerator milestonesAIGenerator;
    private Mock<IOpenAIRepository> openAIRepositoryMock;

    [SetUp]
    public void Setup()
    {
        milestonesAIGenerator = new MilestonesAIGenerator();

        openAIRepositoryMock = new Mock<IOpenAIRepository>();
    }

    [Test]
    public async Task Generate_Milestones()
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

        List<Milestone> milistonesResponse = [milestone1, milestone2];

        string jsonResponse = JsonSerializer.Serialize(milistonesResponse);

        openAIRepositoryMock
            .Setup(repo => repo.GetResponse(""))
            .ReturnsAsync(jsonResponse);

        #endregion

        #region Act

        IEnumerable<Milestone> milestones = await milestonesAIGenerator.GenerateWithIssues();

        #endregion

        #region Assert

        milestones.Should().NotBeEmpty();

        milestones.Should().Contain(milestone1);
        milestones.Should().Contain(milestone2);

        #endregion
    }
}