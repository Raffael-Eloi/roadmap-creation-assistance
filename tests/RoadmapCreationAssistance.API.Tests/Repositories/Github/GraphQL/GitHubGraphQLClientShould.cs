using FluentAssertions;
using GraphQL;
using Moq;
using Moq.Protected;
using RoadmapCreationAssistance.API.Repositories.Github.GraphQL;
using System.Net;
using System.Text.Json;

namespace RoadmapCreationAssistance.API.Tests.Repositories.Github.GraphQL;

public class GitHubGraphQLClientShould
{
    private Mock<IHttpClientFactory> httpClientFactoryMock;
    private Mock<HttpMessageHandler> httpMessageHandlerMock;
    private GitHubGraphQLClient gitHubGraphQLClient;
    private const string Token = "test-token";
    private const string BaseUrl = "https://api.github.com";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [SetUp]
    public void Setup()
    {
        httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        HttpClient httpClient = new(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(BaseUrl)
        };

        httpClientFactoryMock
            .Setup(factory => factory.CreateClient(GitHubGraphQLClient.HttpClientName))
            .Returns(httpClient);

        gitHubGraphQLClient = new GitHubGraphQLClient(httpClientFactoryMock.Object);
    }

    #region GetUserIdAsync

    [Test]
    public async Task Throw_InvalidOperationException_When_User_Is_Null()
    {
        #region Arrange

        SetupGraphQLResponse(new { data = new { user = (object?)null } });

        #endregion

        #region Act

        Func<Task> act = async () => await gitHubGraphQLClient.GetUserIdAsync("nonexistent-user", Token);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not retrieve user ID from GitHub");

        #endregion
    }

    [Test]
    public async Task Throw_InvalidOperationException_When_User_Id_Is_Null()
    {
        #region Arrange

        SetupGraphQLResponse(new { data = new { user = new { id = (string?)null } } });

        #endregion

        #region Act

        Func<Task> act = async () => await gitHubGraphQLClient.GetUserIdAsync("user-with-null-id", Token);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not retrieve user ID from GitHub");

        #endregion
    }

    #endregion

    #region CreateProjectAsync

    [Test]
    public async Task Throw_InvalidOperationException_When_Project_Creation_Returns_Null()
    {
        #region Arrange

        SetupGraphQLResponse(new { data = new { createProjectV2 = (object?)null } });

        #endregion

        #region Act

        Func<Task> act = async () => await gitHubGraphQLClient.CreateProjectAsync("owner-id", "Project Title", Token);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not create project on GitHub");

        #endregion
    }

    [Test]
    public async Task Throw_InvalidOperationException_When_Project_Id_Is_Null()
    {
        #region Arrange

        SetupGraphQLResponse(new { data = new { createProjectV2 = new { projectV2 = new { id = (string?)null } } } });

        #endregion

        #region Act

        Func<Task> act = async () => await gitHubGraphQLClient.CreateProjectAsync("owner-id", "Project Title", Token);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not create project on GitHub");

        #endregion
    }

    #endregion

    #region GetRepositoryIdAsync

    [Test]
    public async Task Throw_InvalidOperationException_When_Repository_Is_Null()
    {
        #region Arrange

        SetupGraphQLResponse(new { data = new { repository = (object?)null } });

        #endregion

        #region Act

        Func<Task> act = async () => await gitHubGraphQLClient.GetRepositoryIdAsync("owner", "repo", Token);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not retrieve repository ID from GitHub");

        #endregion
    }

    [Test]
    public async Task Throw_InvalidOperationException_When_Repository_Id_Is_Null()
    {
        #region Arrange

        SetupGraphQLResponse(new { data = new { repository = new { id = (string?)null } } });

        #endregion

        #region Act

        Func<Task> act = async () => await gitHubGraphQLClient.GetRepositoryIdAsync("owner", "repo", Token);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not retrieve repository ID from GitHub");

        #endregion
    }

    #endregion

    #region GetIssueNodeIdAsync

    [Test]
    public async Task Throw_InvalidOperationException_When_Issue_Is_Null()
    {
        #region Arrange

        SetupGraphQLResponse(new { data = new { repository = new { issue = (object?)null } } });

        #endregion

        #region Act

        int issueNumber = 42;
        Func<Task> act = async () => await gitHubGraphQLClient.GetIssueNodeIdAsync("owner", "repo", issueNumber, Token);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage($"Could not retrieve issue #{issueNumber} node ID from GitHub");

        #endregion
    }

    [Test]
    public async Task Throw_InvalidOperationException_When_Issue_Id_Is_Null()
    {
        #region Arrange

        SetupGraphQLResponse(new { data = new { repository = new { issue = new { id = (string?)null } } } });

        #endregion

        #region Act

        int issueNumber = 123;
        Func<Task> act = async () => await gitHubGraphQLClient.GetIssueNodeIdAsync("owner", "repo", issueNumber, Token);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage($"Could not retrieve issue #{issueNumber} node ID from GitHub");

        #endregion
    }

    #endregion

    #region GraphQL API Errors

    [Test]
    public async Task Throw_InvalidOperationException_When_GraphQL_Returns_Errors()
    {
        #region Arrange

        string errorMessage = "Field 'user' not found";
        SetupGraphQLResponseWithErrors(errorMessage);

        #endregion

        #region Act

        Func<Task> act = async () => await gitHubGraphQLClient.GetUserIdAsync("any-user", Token);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage($"GitHub GraphQL API error: {errorMessage}");

        #endregion
    }

    [Test]
    public async Task Throw_InvalidOperationException_With_Multiple_Error_Messages()
    {
        #region Arrange

        string[] errorMessages = ["First error", "Second error"];
        SetupGraphQLResponseWithErrors(errorMessages);

        #endregion

        #region Act

        Func<Task> act = async () => await gitHubGraphQLClient.GetUserIdAsync("any-user", Token);

        #endregion

        #region Assert

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("GitHub GraphQL API error: First error; Second error");

        #endregion
    }

    #endregion

    #region Helper Methods

    private void SetupGraphQLResponse(object responseData)
    {
        string jsonResponse = JsonSerializer.Serialize(responseData, JsonOptions);

        HttpResponseMessage httpResponse = new(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
        };

        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);
    }

    private void SetupGraphQLResponseWithErrors(params string[] errorMessages)
    {
        object[] errors = [.. errorMessages.Select(msg => new { message = msg })];
        object responseData = new { data = (object?)null, errors };

        string jsonResponse = JsonSerializer.Serialize(responseData, JsonOptions);

        HttpResponseMessage httpResponse = new(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
        };

        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);
    }

    #endregion
}
