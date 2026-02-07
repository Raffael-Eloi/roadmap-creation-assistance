using FluentAssertions;
using Moq;
using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.Models;
using RoadmapCreationAssistance.API.UseCases;
using System.Text.Json;

namespace RoadmapCreationAssistance.API.Tests.UseCases;

internal class MilestonesAIGeneratorShould
{
    private IMilestonesAIGenerator milestonesAIGenerator;
    private RoadmapCreationRequest request;
    private Mock<IPromptProvider> promptProviderMock;
    private Mock<IOpenAIRepository> openAIRepositoryMock;

    [SetUp]
    public void Setup()
    {
        promptProviderMock = new Mock<IPromptProvider>();
        openAIRepositoryMock = new Mock<IOpenAIRepository>();

        milestonesAIGenerator = new MilestonesAIGenerator(promptProviderMock.Object, openAIRepositoryMock.Object);

        request = new RoadmapCreationRequest
        {
            GitHubOwner = "John",
            GitHubRepositoryName = "My repo",
            GitHubToken = "MYTOKEN",
            OpenAIKey = "MYOPENAIKEY",
            ApiDomainDefinition = "MY API SPECS"
        };
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

        List<Milestone> milestonesResponse = [milestone1, milestone2];

        string jsonResponse = JsonSerializer.Serialize(milestonesResponse);

        openAIRepositoryMock
            .Setup(repo => repo.GetResponse(It.IsAny<string>(), request.OpenAIKey))
            .ReturnsAsync(jsonResponse);

        string prompt = "Prompt: Software Engineering Confidence Roadmap";

        promptProviderMock
            .Setup(provider => provider.GetMilestoneInstructionAsync(request.Language, request.ApiDomainDefinition))
            .ReturnsAsync(prompt);

        #endregion

        #region Act

        IEnumerable<Milestone> milestones = await milestonesAIGenerator.GenerateWithIssues(request);

        #endregion

        #region Assert

        milestones.Should().NotBeEmpty();
        milestones.Should().BeEquivalentTo([milestone1, milestone2]);

        // Verify the prompt was sent to the repository
        openAIRepositoryMock.Verify(
            repo => repo.GetResponse(prompt, request.OpenAIKey),
            Times.Once()
        );

        #endregion
    }

    [Test]
    public async Task Throw_InvalidOperationException_When_Deserialization_Returns_Null()
    {
        #region Arrange

        string invalidJsonResponse = "null";

        openAIRepositoryMock
            .Setup(repo => repo.GetResponse(It.IsAny<string>(), request.OpenAIKey))
            .ReturnsAsync(invalidJsonResponse);

        #endregion

        #region Act

        Func<Task> act = async () => await milestonesAIGenerator.GenerateWithIssues(request);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to deserialize milestones from AI response.");

        #endregion
    }

    [Test]
    public async Task Throw_JsonException_When_Response_Is_Invalid_Json()
    {
        #region Arrange

        string invalidJsonResponse = "this is not valid json";

        openAIRepositoryMock
            .Setup(repo => repo.GetResponse(It.IsAny<string>(), request.OpenAIKey))
            .ReturnsAsync(invalidJsonResponse);

        #endregion

        #region Act

        Func<Task> act = async () => await milestonesAIGenerator.GenerateWithIssues(request);

        #endregion

        #region Assert

        await act.Should().ThrowAsync<JsonException>();

        #endregion
    }

    [Test]
    public async Task Propagate_HttpRequestException_When_OpenAI_Request_Fails()
    {
        #region Arrange

        string expectedMessage = "Error occurred while sending request to OpenAI API.";

        openAIRepositoryMock
            .Setup(repo => repo.GetResponse(It.IsAny<string>(), request.OpenAIKey))
            .ThrowsAsync(new HttpRequestException(expectedMessage));

        #endregion

        #region Act

        Func<Task> act = async () => await milestonesAIGenerator.GenerateWithIssues(request);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<HttpRequestException>()
            .WithMessage(expectedMessage);

        #endregion
    }
}