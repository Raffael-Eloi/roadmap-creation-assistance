using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Middlewares;
using RoadmapCreationAssistance.API.Policies;
using RoadmapCreationAssistance.API.Repositories.Github;
using RoadmapCreationAssistance.API.Repositories.Github.GraphQL;
using RoadmapCreationAssistance.API.Repositories.OpenAI;
using RoadmapCreationAssistance.API.UseCases;
using Serilog;
using Serilog.Sinks.Datadog.Logs;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.Enrich.FromLogContext()
      .WriteTo.Console();

    string? ddApiKey = Environment.GetEnvironmentVariable("DD_API_KEY");

    if (!string.IsNullOrWhiteSpace(ddApiKey))
    {
        var datadogConfig = new DatadogConfiguration
        {
            Url = "intake.logs.datadoghq.com",
            Port = 10516,
            UseSSL = true,
            UseTCP = true
        };
        lc.WriteTo.DatadogLogs(ddApiKey, configuration: datadogConfig);
    }
});

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<HttpPolicies>();

builder.Services.AddHttpClient(OpenAIRepository.HttpClientName, client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OpenAIApi:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(240);
})
.AddPolicyHandler((sp, _) =>
{
    HttpPolicies policies = sp.GetRequiredService<HttpPolicies>();
    return policies.GetOpenAIRetryPolicy();
});

builder.Services.AddHttpClient(GithubRepository.HttpClientName, client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GitHubApi:BaseUrl"]!);
})
.AddPolicyHandler((sp, _) =>
{
    HttpPolicies policies = sp.GetRequiredService<HttpPolicies>();
    return policies.GetGitHubRetryPolicy();
});

builder.Services.AddHttpClient(GitHubGraphQLClient.HttpClientName, client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GitHubApi:BaseUrl"]!);
})
.AddPolicyHandler((sp, _) =>
{
    HttpPolicies policies = sp.GetRequiredService<HttpPolicies>();
    return policies.GetGitHubRetryPolicy();
});

builder.Services.AddScoped<IMilestonesAIGenerator, MilestonesAIGenerator>();
builder.Services.AddScoped<IReadmeAIGenerator, ReadmeAIGenerator>();
builder.Services.AddScoped<IOpenAIRepository, OpenAIRepository>();
builder.Services.AddScoped<IRoadmapCreator, RoadmapCreator>();
builder.Services.AddScoped<IPromptProvider, PromptProvider>();
builder.Services.AddScoped<IGithubRepository, GithubRepository>();
builder.Services.AddScoped<IGitHubGraphQLClient, GitHubGraphQLClient>();

builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri(builder.Configuration["GitHubApi:HealthCheckUrl"]!), "github")
    .AddUrlGroup(new Uri(builder.Configuration["OpenAIApi:HealthCheckUrl"]!), "openai");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();