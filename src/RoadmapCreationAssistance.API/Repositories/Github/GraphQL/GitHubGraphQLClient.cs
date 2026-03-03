using System.Net.Http.Headers;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using RoadmapCreationAssistance.API.Contracts.Repositories;

namespace RoadmapCreationAssistance.API.Repositories.Github.GraphQL;

public sealed class GitHubGraphQLClient(IHttpClientFactory httpClientFactory) : IGitHubGraphQLClient
{
	public const string HttpClientName = "GitHubGraphQL";

	public async Task<string> GetUserIdAsync(string login, string token)
	{
		GraphQLRequest request = new GraphQLRequest
		{
			Query = @"
				query($login: String!) {
					user(login: $login) {
						id
					}
				}",
			Variables = new { login }
		};

		UserResponse response = await ExecuteAsync<UserResponse>(request, token);

		return response.User?.Id
			?? throw new InvalidOperationException("Could not retrieve user ID from GitHub");
	}

	public async Task<string> CreateProjectAsync(string ownerId, string title, string token)
	{
		GraphQLRequest request = new GraphQLRequest
		{
			Query = @"
				mutation($ownerId: ID!, $title: String!) {
					createProjectV2(input: { ownerId: $ownerId, title: $title }) {
						projectV2 {
							id
						}
					}
				}",
			Variables = new { ownerId, title }
		};

		CreateProjectResponse response = await ExecuteAsync<CreateProjectResponse>(request, token);

		return response.CreateProjectV2?.ProjectV2?.Id
			?? throw new InvalidOperationException("Could not create project on GitHub");
	}

	public async Task<string> GetRepositoryIdAsync(string owner, string name, string token)
	{
		GraphQLRequest request = new GraphQLRequest
		{
			Query = @"
				query($owner: String!, $name: String!) {
					repository(owner: $owner, name: $name) {
						id
					}
				}",
			Variables = new { owner, name }
		};

		RepositoryResponse response = await ExecuteAsync<RepositoryResponse>(request, token);

		return response.Repository?.Id
			?? throw new InvalidOperationException("Could not retrieve repository ID from GitHub");
	}

	public async Task LinkRepositoryToProjectAsync(string projectId, string repositoryId, string token)
	{
		GraphQLRequest request = new GraphQLRequest
		{
			Query = @"
				mutation($projectId: ID!, $repositoryId: ID!) {
					linkProjectV2ToRepository(input: { projectId: $projectId, repositoryId: $repositoryId }) {
						repository {
							id
						}
					}
				}",
			Variables = new { projectId, repositoryId }
		};

		await ExecuteAsync<LinkRepositoryResponse>(request, token);
	}

	public async Task<string> GetIssueNodeIdAsync(string owner, string repo, int issueNumber, string token)
	{
		GraphQLRequest request = new GraphQLRequest
		{
			Query = @"
				query($owner: String!, $repo: String!, $issueNumber: Int!) {
					repository(owner: $owner, name: $repo) {
						issue(number: $issueNumber) {
							id
						}
					}
				}",
			Variables = new { owner, repo, issueNumber }
		};

		RepositoryWithIssueResponse response = await ExecuteAsync<RepositoryWithIssueResponse>(request, token);

		return response.Repository?.Issue?.Id
			?? throw new InvalidOperationException($"Could not retrieve issue #{issueNumber} node ID from GitHub");
	}

	public async Task AddItemToProjectAsync(string projectId, string contentId, string token)
	{
		GraphQLRequest request = new GraphQLRequest
		{
			Query = @"
				mutation($projectId: ID!, $contentId: ID!) {
					addProjectV2ItemById(input: { projectId: $projectId, contentId: $contentId }) {
						item {
							id
						}
					}
				}",
			Variables = new { projectId, contentId }
		};

		await ExecuteAsync<AddItemResponse>(request, token);
	}

	private async Task<T> ExecuteAsync<T>(GraphQLRequest request, string token)
	{
		HttpClient httpClient = httpClientFactory.CreateClient(HttpClientName);
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		using GraphQLHttpClient graphQLClient = new GraphQLHttpClient(
			new GraphQLHttpClientOptions { EndPoint = new Uri(httpClient.BaseAddress!, "/graphql") },
			new SystemTextJsonSerializer(),
			httpClient);

		GraphQLResponse<T> response = await graphQLClient.SendQueryAsync<T>(request);

		if (response.Errors?.Length > 0)
		{
			string errorMessages = string.Join("; ", response.Errors.Select(e => e.Message));
			throw new InvalidOperationException($"GitHub GraphQL API error: {errorMessages}");
		}

		return response.Data;
	}
}