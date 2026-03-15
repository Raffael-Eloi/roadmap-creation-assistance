using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.Services;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Policies;
using RoadmapCreationAssistance.API.Repositories;
using RoadmapCreationAssistance.API.Repositories.Github;
using RoadmapCreationAssistance.API.Repositories.Github.GraphQL;
using RoadmapCreationAssistance.API.Repositories.OpenAI;
using RoadmapCreationAssistance.API.Services;
using RoadmapCreationAssistance.API.UseCases;

namespace RoadmapCreationAssistance.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRoadmapServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<HttpPolicies>();

        services.AddHttpClient(OpenAIRepository.HttpClientName, client =>
        {
            client.BaseAddress = new Uri(configuration["OpenAIApi:BaseUrl"]!);
            client.Timeout = TimeSpan.FromSeconds(240);
        })
        .AddPolicyHandler((sp, _) =>
        {
            HttpPolicies policies = sp.GetRequiredService<HttpPolicies>();
            return policies.GetOpenAIRetryPolicy();
        });

        services.AddHttpClient(GithubRepository.HttpClientName, client =>
        {
            client.BaseAddress = new Uri(configuration["GitHubApi:BaseUrl"]!);
        })
        .AddPolicyHandler((sp, _) =>
        {
            HttpPolicies policies = sp.GetRequiredService<HttpPolicies>();
            return policies.GetGitHubRetryPolicy();
        });

        services.AddHttpClient(GitHubGraphQLClient.HttpClientName, client =>
        {
            client.BaseAddress = new Uri(configuration["GitHubApi:BaseUrl"]!);
        })
        .AddPolicyHandler((sp, _) =>
        {
            HttpPolicies policies = sp.GetRequiredService<HttpPolicies>();
            return policies.GetGitHubRetryPolicy();
        });

        services.AddScoped<IDatabaseRepository, DatabaseRepository>();
        services.AddScoped<IMilestonesAIGenerator, MilestonesAIGenerator>();
        services.AddScoped<IReadmeAIGenerator, ReadmeAIGenerator>();
        services.AddScoped<IOpenAIRepository, OpenAIRepository>();
        services.AddScoped<IRoadmapCreator, RoadmapCreator>();
        services.AddScoped<IPromptProvider, PromptProvider>();
        services.AddScoped<IGithubRepository, GithubRepository>();
        services.AddScoped<IGitHubGraphQLClient, GitHubGraphQLClient>();
        services.AddScoped<ICreateRoadmap, CreateRoadmap>();

		services.AddScoped<IQueueService>(sp =>
        {
            string connectionString = configuration["AzureStorage:ConnectionString"]!;
            ILogger<QueueServices> logger = sp.GetRequiredService<ILogger<QueueServices>>();
            return new QueueServices(logger, connectionString);
        });

        return services;
    }
}
