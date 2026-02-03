using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.Extensions;
using RoadmapCreationAssistance.API.Models;
using RoadmapCreationAssistance.API.Repositories.Github.Models;
using System.Net.Http.Headers;

namespace RoadmapCreationAssistance.API.Repositories.Github;

public class GithubRepository(IHttpClientFactory httpClientFactory, IGitHubGraphQLClient graphQLClient) : IGithubRepository
{
    public const string HttpClientName = "GitHub";

    public async Task CreateLabels(IEnumerable<Label> labels, RoadmapCreationRequest request)
    {
        HttpClient httpClient = CreateHttpClient(request);

        foreach (Label label in labels)
        {
            HttpContent labelContent = label.ToJsonContent();
            await httpClient.PostAsync($"/repos/{request.GitHubOwner}/{request.GitHubRepositoryName}/labels", labelContent);
        }
    }

    private HttpClient CreateHttpClient(RoadmapCreationRequest request)
    {
        HttpClient httpClient = httpClientFactory.CreateClient(HttpClientName);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.GitHubToken);
        httpClient.DefaultRequestHeaders.Add("User-Agent", request.GitHubOwner);
        return httpClient;
    }

    public async Task CreateMilestones(IEnumerable<Milestone> milestones, RoadmapCreationRequest request)
    {
        HttpClient httpClient = CreateHttpClient(request);

        foreach (Milestone milestone in milestones)
        {
            GithubMilestone githubMilestone = new()
            {
                Title = milestone.Title,
                Description = milestone.Description
            };

            HttpContent content = githubMilestone.ToJsonContent();
            HttpResponseMessage response = await httpClient.PostAsync($"/repos/{request.GitHubOwner}/{request.GitHubRepositoryName}/milestones", content);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Failed to create milestone '{milestone.Title}'. Status code: {response.StatusCode}");

            GithubMilestone milestoneResponse = await response.DeserializeAsync<GithubMilestone>();
            milestone.Id = milestoneResponse.Number!.Value;
        }
    }


    public async Task CreateIssues(IEnumerable<Issue> issues, RoadmapCreationRequest request)
    {
        HttpClient httpClient = CreateHttpClient(request);

        foreach (Issue issue in issues)
        {
            HttpContent content = issue.ToJsonContent();
            HttpResponseMessage response = await httpClient.PostAsync($"/repos/{request.GitHubOwner}/{request.GitHubRepositoryName}/issues", content);
            IssueResponse issueCreated = await response.DeserializeAsync<IssueResponse>();
            issue.Number = issueCreated.Number;
        }
    }

    public async Task CreateProject(Project project, RoadmapCreationRequest request)
    {
        string ownerId = await graphQLClient.GetUserIdAsync(request.GitHubOwner, request.GitHubToken);

        string projectId = await graphQLClient.CreateProjectAsync(ownerId, project.Title, request.GitHubToken);
        project.Id = projectId;

        string repositoryId = await graphQLClient.GetRepositoryIdAsync(
            request.GitHubOwner,
            request.GitHubRepositoryName,
            request.GitHubToken);

        await graphQLClient.LinkRepositoryToProjectAsync(projectId, repositoryId, request.GitHubToken);
    }

    public async Task LinkIssuesToProject(Project project, IEnumerable<Issue> issues, RoadmapCreationRequest request)
    {
        foreach (Issue issue in issues)
        {
            if (!issue.Number.HasValue)
                continue;

            try
            {
                string issueNodeId = await graphQLClient.GetIssueNodeIdAsync(
                    request.GitHubOwner,
                    request.GitHubRepositoryName,
                    issue.Number.Value,
                    request.GitHubToken);

                await graphQLClient.AddItemToProjectAsync(project.Id!, issueNodeId, request.GitHubToken);
            }
            catch (InvalidOperationException)
            {
                // Skip issues that cannot be linked
                continue;
            }
        }
    }

    public async Task CreateReadme(string readme, RoadmapCreationRequest request)
    {
        HttpClient httpClient = CreateHttpClient(request);

        UpdateContent content = new()
        {
            Message = "Add README.md",
            Content = readme.ToBase64()
        };

        await httpClient.PutAsync($"/repos/{request.GitHubOwner}/{request.GitHubRepositoryName}/contents/README.md", content.ToJsonContent());
    }
}