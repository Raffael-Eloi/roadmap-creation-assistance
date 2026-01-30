using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.Extensions;
using RoadmapCreationAssistance.API.Models;
using System.Text;
using System.Text.Json;

namespace RoadmapCreationAssistance.API.Repositories;

public class GithubRepository(HttpClient httpClient, IConfiguration configuration) : IGithubRepository
{
    public async Task CreateLabels(IEnumerable<Label> labels, RoadmapCreationRequest request)
    {
        string baseUrl = GetBaseUrl();

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.GitHubToken);
        httpClient.DefaultRequestHeaders.Add("User-Agent", request.GitHubOwner);

        foreach (Label label in labels)
        {
            HttpContent content = label.ToJsonContent();
            await httpClient.PostAsync($"{baseUrl}/repos/{request.GitHubOwner}/{request.GitHubRepositoryName}/labels", content);
        }
    }

    public async Task CreateMilestones(IEnumerable<Milestone> milestones, RoadmapCreationRequest request)
    {
        string baseUrl = GetBaseUrl();

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.GitHubToken);
        httpClient.DefaultRequestHeaders.Add("User-Agent", request.GitHubOwner);

        foreach (Milestone milestone in milestones)
        {
            GithubMilestone githubMilestone = new()
            {
                Title = milestone.Title,
                Description = milestone.Description
            };

            HttpContent content = githubMilestone.ToJsonContent();
            HttpResponseMessage response = await httpClient.PostAsync($"{baseUrl}/repos/{request.GitHubOwner}/{request.GitHubRepositoryName}/milestones", content);
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
        string baseUrl = GetBaseUrl();

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.GitHubToken);
        httpClient.DefaultRequestHeaders.Add("User-Agent", request.GitHubOwner);

        foreach (Issue issue in issues)
        {
            HttpContent content = issue.ToJsonContent();
            HttpResponseMessage response = await httpClient.PostAsync($"{baseUrl}/repos/{request.GitHubOwner}/{request.GitHubRepositoryName}/issues", content);
            IssueResponse issueCreated = await response.DeserializeAsync<IssueResponse>();
            issue.Number = issueCreated.Number;
        }
    }

    public async Task CreateProject(Project project, RoadmapCreationRequest request)
    {
        string graphqlUrl = "https://api.github.com/graphql";

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.GitHubToken);
        httpClient.DefaultRequestHeaders.Add("User-Agent", request.GitHubOwner);

        // First, get the owner ID (user or organization)
        string ownerQuery = $$"""
            {
                "query": "query { user(login: \"{{request.GitHubOwner}}\") { id } }"
            }
            """;

        HttpContent ownerContent = new StringContent(ownerQuery, Encoding.UTF8, "application/json");
        HttpResponseMessage ownerResponse = await httpClient.PostAsync(graphqlUrl, ownerContent);
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

        HttpContent projectContent = new StringContent(createProjectMutation, Encoding.UTF8, "application/json");
        HttpResponseMessage projectResponse = await httpClient.PostAsync(graphqlUrl, projectContent);
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

        HttpContent viewQueryContent = new StringContent(getViewAndFieldQuery, Encoding.UTF8, "application/json");
        HttpResponseMessage viewQueryResponse = await httpClient.PostAsync(graphqlUrl, viewQueryContent);
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

        // Add "In Review" option to Status field
        if (!string.IsNullOrEmpty(statusFieldId))
        {
            string addOptionMutation = $$"""
                {
                    "query": "mutation { updateProjectV2Field(input: { projectId: \"{{projectId}}\", fieldId: \"{{statusFieldId}}\", singleSelectOptions: [ { name: \"Todo\", color: GRAY }, { name: \"In Progress\", color: YELLOW }, { name: \"In Review\", color: BLUE }, { name: \"Done\", color: GREEN } ] }) { projectV2Field { ... on ProjectV2SingleSelectField { id } } } }"
                }
                """;

            HttpContent addOptionContent = new StringContent(addOptionMutation, Encoding.UTF8, "application/json");
            await httpClient.PostAsync(graphqlUrl, addOptionContent);
        }

        // Update view to Board layout and group by Status
        if (!string.IsNullOrEmpty(viewId) && !string.IsNullOrEmpty(statusFieldId))
        {
            string updateViewMutation = $$"""
                {
                    "query": "mutation { updateProjectV2View(input: { projectId: \"{{projectId}}\", viewId: \"{{viewId}}\", layout: BOARD_LAYOUT, groupByFields: [\"{{statusFieldId}}\"] }) { projectV2View { id } } }"
                }
                """;

            HttpContent updateViewContent = new StringContent(updateViewMutation, Encoding.UTF8, "application/json");
            await httpClient.PostAsync(graphqlUrl, updateViewContent);
        }
        else if (!string.IsNullOrEmpty(viewId))
        {
            string updateViewMutation = $$"""
                {
                    "query": "mutation { updateProjectV2View(input: { projectId: \"{{projectId}}\", viewId: \"{{viewId}}\", layout: BOARD_LAYOUT }) { projectV2View { id } } }"
                }
                """;

            HttpContent updateViewContent = new StringContent(updateViewMutation, Encoding.UTF8, "application/json");
            await httpClient.PostAsync(graphqlUrl, updateViewContent);
        }

        // Link the repository to the project
        string repoQuery = $$"""
            {
                "query": "query { repository(owner: \"{{request.GitHubOwner}}\", name: \"{{request.GitHubRepositoryName}}\") { id } }"
            }
            """;

        HttpContent repoContent = new StringContent(repoQuery, Encoding.UTF8, "application/json");
        HttpResponseMessage repoResponse = await httpClient.PostAsync(graphqlUrl, repoContent);
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

        HttpContent linkContent = new StringContent(linkRepoMutation, Encoding.UTF8, "application/json");
        await httpClient.PostAsync(graphqlUrl, linkContent);
    }

    public async Task LinkIssuesToProject(Project project, IEnumerable<Issue> issues, RoadmapCreationRequest request)
    {
        string graphqlUrl = "https://api.github.com/graphql";

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.GitHubToken);
        httpClient.DefaultRequestHeaders.Add("User-Agent", request.GitHubOwner);

        foreach (Issue issue in issues)
        {
            // Get the issue's node ID
            string issueQuery = $$"""
                {
                    "query": "query { repository(owner: \"{{request.GitHubOwner}}\", name: \"{{request.GitHubRepositoryName}}\") { issue(number: {{issue.Number}}) { id } } }"
                }
                """;

            HttpContent issueContent = new StringContent(issueQuery, Encoding.UTF8, "application/json");
            HttpResponseMessage issueResponse = await httpClient.PostAsync(graphqlUrl, issueContent);
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

            HttpContent addItemContent = new StringContent(addItemMutation, Encoding.UTF8, "application/json");
            await httpClient.PostAsync(graphqlUrl, addItemContent);
        }
    }

    internal class GithubMilestone
    {
        public int? Number { get; set; }

        public required string Title { get; set; }

        public string Description { get; set; } = string.Empty;
    }

    internal class IssueResponse
    {
        public int Number { get; set; }
    }

    internal class IssueToProject
    {
        public string Type => "Issue";

        public required string Repo { get; set; }

        public required int Number { get; set; }
    }
}