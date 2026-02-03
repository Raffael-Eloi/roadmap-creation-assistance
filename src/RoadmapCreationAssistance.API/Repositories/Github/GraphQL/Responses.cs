namespace RoadmapCreationAssistance.API.Repositories.Github.GraphQL;

// Query responses
public sealed record UserResponse(UserData User);
public sealed record UserData(string Id);

public sealed record RepositoryResponse(RepositoryData Repository);
public sealed record RepositoryData(string Id);

public sealed record RepositoryWithIssueResponse(RepositoryWithIssueData Repository);
public sealed record RepositoryWithIssueData(IssueData Issue);
public sealed record IssueData(string Id);

// Mutation responses
public sealed record CreateProjectResponse(CreateProjectV2Data CreateProjectV2);
public sealed record CreateProjectV2Data(ProjectV2Data ProjectV2);
public sealed record ProjectV2Data(string Id);

public sealed record LinkRepositoryResponse(LinkProjectV2ToRepositoryData LinkProjectV2ToRepository);
public sealed record LinkProjectV2ToRepositoryData(RepositoryData Repository);

public sealed record AddItemResponse(AddProjectV2ItemByIdData AddProjectV2ItemById);
public sealed record AddProjectV2ItemByIdData(ProjectItemData Item);
public sealed record ProjectItemData(string Id);
