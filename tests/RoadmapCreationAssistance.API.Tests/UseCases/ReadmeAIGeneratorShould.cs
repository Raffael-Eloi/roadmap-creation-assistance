using FluentAssertions;
using Moq;
using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Models;
using RoadmapCreationAssistance.API.UseCases;

namespace RoadmapCreationAssistance.API.Tests.UseCases;

internal class ReadmeAIGeneratorShould
{
    private IReadmeAIGenerator readmeAIGenerator;
    private RoadmapCreationRequest request;
    private Mock<IOpenAIRepository> openAIRepositoryMock;

    [SetUp]
    public void Setup()
    {
        openAIRepositoryMock = new Mock<IOpenAIRepository>();

        readmeAIGenerator = new ReadmeAIGenerator(openAIRepositoryMock.Object);

        request = new RoadmapCreationRequest
        {
            GitHubOwner = "John",
            GitHubRepositoryName = "My repo",
            GitHubToken = "MYTOKEN",
            OpenAIKey = "MYOPENAIKEY"
        };
    }

    [Test]
    public async Task Generate_Readme()
    {
        #region Arrange

        string openAiResponse = "# This is the readme generated";

        openAIRepositoryMock
            .Setup(repo => repo.GetResponse(It.IsAny<string>(), request.OpenAIKey))
            .ReturnsAsync(openAiResponse);

        #endregion

        #region Act

        string response = await readmeAIGenerator.GenerateAsync(request);

        #endregion

        #region Assert

        // Verify the prompt was sent to the repository
        openAIRepositoryMock.Verify(
            repo => repo.GetResponse(It.Is<string>(prompt =>
                prompt.Contains("📌 Prompt: Software Engineering Confidence Roadmap") &&
                prompt.Contains("***Given this prompt, I want you to generate a readme in MarkDown***") &&
                prompt.Contains($"All documentation, milestones, issues and code must be written **in {request.Language}**")
            ), request.OpenAIKey),
            Times.Once()
        );

        response.Should().Be(openAiResponse);

        #endregion
    }
}