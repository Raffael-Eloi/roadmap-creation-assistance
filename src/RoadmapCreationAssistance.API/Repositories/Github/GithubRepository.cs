using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.Extensions;
using RoadmapCreationAssistance.API.Models;
using RoadmapCreationAssistance.API.Repositories.Github.Models;
using System.Text.Json;

namespace RoadmapCreationAssistance.API.Repositories.Github;

public class GithubRepository(IConfiguration configuration) : IGithubRepository
{
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
        HttpClient httpClient = new();
        string baseUrl = GetBaseUrl();
        httpClient.BaseAddress = new Uri(baseUrl);
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.GitHubToken);
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
            GithubMilestone milestoneResponse = await response.DeserializeAsync<GithubMilestone>();
            milestone.Id = milestoneResponse.Number!.Value;
        }
    }

    private string GetBaseUrl()
    {
        return configuration["GitHubApiBaseUrl"]!;
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
        HttpClient httpClient = CreateHttpClient(request);

        // First, get the owner ID (user or organization)
        string ownerQuery = $$"""
            {
                "query": "query { user(login: \"{{request.GitHubOwner}}\") { id } }"
            }
            """;

        HttpContent ownerContent = ownerQuery.ToJsonContent();
        HttpResponseMessage ownerResponse = await httpClient.PostAsync("/graphql", ownerContent);
        string ownerResponseJson = await ownerResponse.Content.ReadAsStringAsync();

        using JsonDocument ownerDoc = JsonDocument.Parse(ownerResponseJson);
        string? ownerId = ownerDoc.RootElement
            .GetProperty("data")
            .GetProperty("user")
            .GetProperty("id")
            .GetString();

        if (string.IsNullOrEmpty(ownerId))
            throw new InvalidOperationException("Could not retrieve owner ID from GitHub");

        // Create the project
        string createProjectMutation = $$"""
            {
                "query": "mutation { createProjectV2(input: { ownerId: \"{{ownerId}}\", title: \"{{project.Title}}\" }) { projectV2 { id } } }"
            }
            """;

        HttpContent projectContent = createProjectMutation.ToJsonContent();
        HttpResponseMessage projectResponse = await httpClient.PostAsync("/graphql", projectContent);
        string projectResponseJson = await projectResponse.Content.ReadAsStringAsync();

        using JsonDocument projectDoc = JsonDocument.Parse(projectResponseJson);
        string? projectId = projectDoc.RootElement
            .GetProperty("data")
            .GetProperty("createProjectV2")
            .GetProperty("projectV2")
            .GetProperty("id")
            .GetString();

        if (string.IsNullOrEmpty(projectId))
            throw new InvalidOperationException("Could not create project on GitHub");

        project.Id = projectId;

        // Get the default view ID and Status field ID
        string getViewAndFieldQuery = $$"""
            {
                "query": "query { node(id: \"{{projectId}}\") { ... on ProjectV2 { views(first: 1) { nodes { id } } fields(first: 20) { nodes { ... on ProjectV2SingleSelectField { id name options { id name } } } } } } }"
            }
            """;

        HttpContent viewQueryContent = getViewAndFieldQuery.ToJsonContent();
        HttpResponseMessage viewQueryResponse = await httpClient.PostAsync("/graphql", viewQueryContent);
        string viewQueryResponseJson = await viewQueryResponse.Content.ReadAsStringAsync();

        using JsonDocument viewDoc = JsonDocument.Parse(viewQueryResponseJson);
        string? viewId = viewDoc.RootElement
            .GetProperty("data")
            .GetProperty("node")
            .GetProperty("views")
            .GetProperty("nodes")[0]
            .GetProperty("id")
            .GetString();

        // Find the Status field ID
        string? statusFieldId = null;
        JsonElement fields = viewDoc.RootElement
            .GetProperty("data")
            .GetProperty("node")
            .GetProperty("fields")
            .GetProperty("nodes");

        foreach (JsonElement field in fields.EnumerateArray())
        {
            if (field.TryGetProperty("name", out JsonElement nameElement) &&
                nameElement.GetString() == "Status")
            {
                statusFieldId = field.GetProperty("id").GetString();
                break;
            }
        }

        // Link the repository to the project
        string repoQuery = $$"""
            {
                "query": "query { repository(owner: \"{{request.GitHubOwner}}\", name: \"{{request.GitHubRepositoryName}}\") { id } }"
            }
            """;

        HttpContent repoContent = repoQuery.ToJsonContent();
        HttpResponseMessage repoResponse = await httpClient.PostAsync("/graphql", repoContent);
        string repoResponseJson = await repoResponse.Content.ReadAsStringAsync();

        using JsonDocument repoDoc = JsonDocument.Parse(repoResponseJson);
        string? repoId = repoDoc.RootElement
            .GetProperty("data")
            .GetProperty("repository")
            .GetProperty("id")
            .GetString();

        if (string.IsNullOrEmpty(repoId))
            throw new InvalidOperationException("Could not retrieve repository ID from GitHub");

        string linkRepoMutation = $$"""
            {
                "query": "mutation { linkProjectV2ToRepository(input: { projectId: \"{{projectId}}\", repositoryId: \"{{repoId}}\" }) { repository { id } } }"
            }
            """;

        HttpContent linkContent = linkRepoMutation.ToJsonContent();
        await httpClient.PostAsync("/graphql", linkContent);
    }

    public async Task LinkIssuesToProject(Project project, IEnumerable<Issue> issues, RoadmapCreationRequest request)
    {
        HttpClient httpClient = CreateHttpClient(request);

        foreach (Issue issue in issues)
        {
            // Get the issue's node ID
            string issueQuery = $$"""
                {
                    "query": "query { repository(owner: \"{{request.GitHubOwner}}\", name: \"{{request.GitHubRepositoryName}}\") { issue(number: {{issue.Number}}) { id } } }"
                }
                """;

            HttpContent issueContent = issueQuery.ToJsonContent();
            HttpResponseMessage issueResponse = await httpClient.PostAsync("/graphql", issueContent);
            string issueResponseJson = await issueResponse.Content.ReadAsStringAsync();

            using JsonDocument issueDoc = JsonDocument.Parse(issueResponseJson);
            string? issueNodeId = issueDoc.RootElement
                .GetProperty("data")
                .GetProperty("repository")
                .GetProperty("issue")
                .GetProperty("id")
                .GetString();

            if (string.IsNullOrEmpty(issueNodeId))
                continue;

            // Add the issue to the project
            string addItemMutation = $$"""
                {
                    "query": "mutation { addProjectV2ItemById(input: { projectId: \"{{project.Id}}\", contentId: \"{{issueNodeId}}\" }) { item { id } } }"
                }
                """;

            HttpContent addItemContent = addItemMutation.ToJsonContent();
            await httpClient.PostAsync("/graphql", addItemContent);
        }
    }
}