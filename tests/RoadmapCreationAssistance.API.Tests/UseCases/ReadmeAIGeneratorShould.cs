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

        Func<Task> act = async () => await readmeAIGenerator.GenerateAsync(request);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<HttpRequestException>()
            .WithMessage(expectedMessage);

        #endregion
    }

    [Test]
    public async Task Propagate_InvalidOperationException_When_OpenAI_Returns_No_Output()
    {
        #region Arrange

        string expectedMessage = "No valid output found in OpenAI response.";

        openAIRepositoryMock
            .Setup(repo => repo.GetResponse(It.IsAny<string>(), request.OpenAIKey))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        #endregion

        #region Act

        Func<Task> act = async () => await readmeAIGenerator.GenerateAsync(request);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(expectedMessage);

        #endregion
    }

    [Test]
    public async Task Propagate_InvalidOperationException_When_OpenAI_Returns_No_Content()
    {
        #region Arrange

        string expectedMessage = "No content found in OpenAI output.";

        openAIRepositoryMock
            .Setup(repo => repo.GetResponse(It.IsAny<string>(), request.OpenAIKey))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        #endregion

        #region Act

        Func<Task> act = async () => await readmeAIGenerator.GenerateAsync(request);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(expectedMessage);

        #endregion
    }

    [Test]
    public async Task Propagate_InvalidOperationException_When_OpenAI_Returns_Empty_Text()
    {
        #region Arrange

        string expectedMessage = "OpenAI content text is empty.";

        openAIRepositoryMock
            .Setup(repo => repo.GetResponse(It.IsAny<string>(), request.OpenAIKey))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        #endregion

        #region Act

        Func<Task> act = async () => await readmeAIGenerator.GenerateAsync(request);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(expectedMessage);

        #endregion
    }
}