namespace RoadmapCreationAssistance.API.Contracts.Repositories;

public interface IGitHubGraphQLClient
{
    Task<string> GetUserIdAsync(string login, string token);
    
    Task<string> CreateProjectAsync(string ownerId, string title, string token);
    
    Task<string> GetRepositoryIdAsync(string owner, string name, string token);
    
    Task LinkRepositoryToProjectAsync(string projectId, string repositoryId, string token);
    
    Task<string> GetIssueNodeIdAsync(string owner, string repo, int issueNumber, string token);
    
    Task AddItemToProjectAsync(string projectId, string contentId, string token);
}