using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Entities;
using RoadmapCreationAssistance.API.JsonSerialization;
using RoadmapCreationAssistance.API.Models;
using System.Text;
using System.Text.Json;

namespace RoadmapCreationAssistance.API.Repositories;

public class GithubRepository(HttpClient httpClient, IConfiguration configuration) : IGithubRepository
{
    public Task CreateLabels(IEnumerable<Label> labels, RoadmapCreationRequest request)
    {
        throw new NotImplementedException();
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

            HttpContent content = CreateHttpContent(githubMilestone);
            HttpResponseMessage response = await httpClient.PostAsync($"{baseUrl}/repos/{request.GitHubOwner}/{request.GitHubRepositoryName}/milestones", content);
            string responseJson = await response.Content.ReadAsStringAsync();
            GithubMilestone? milestoneResponse = JsonSerializer.Deserialize<GithubMilestone>(responseJson, JsonSerializationOptions.Default);
            if (milestoneResponse is not null)
                milestone.Id = milestoneResponse.Number!.Value;
        }
    }

    private static HttpContent CreateHttpContent(GithubMilestone milestone)
    {
        string json = JsonSerializer.Serialize(milestone, JsonSerializationOptions.Default);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        return content;
    }

    private static HttpContent CreateHttpContent(Issue issue)
    {
        string json = JsonSerializer.Serialize(issue, JsonSerializationOptions.Default);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        return content;
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
            HttpContent content = CreateHttpContent(issue);
            HttpResponseMessage response = await httpClient.PostAsync($"{baseUrl}/repos/{request.GitHubOwner}/{request.GitHubRepositoryName}/issues", content);
            string responseJson = await response.Content.ReadAsStringAsync();
        }
    }

    public class GithubMilestone
    {
        public int? Number { get; set; }

        public required string Title { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}