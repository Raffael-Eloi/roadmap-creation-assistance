using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Middlewares;
using RoadmapCreationAssistance.API.Repositories.Github;
using RoadmapCreationAssistance.API.Repositories.Github.GraphQL;
using RoadmapCreationAssistance.API.Repositories.OpenAI;
using RoadmapCreationAssistance.API.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient(OpenAIRepository.HttpClientName, client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OpenAIApiBaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(180);
});

builder.Services.AddHttpClient(GithubRepository.HttpClientName, client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GitHubApiBaseUrl"]!);
});

builder.Services.AddHttpClient(GitHubGraphQLClient.HttpClientName, client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GitHubApiBaseUrl"]!);
});

builder.Services.AddScoped<IMilestonesAIGenerator, MilestonesAIGenerator>();
builder.Services.AddScoped<IReadmeAIGenerator, ReadmeAIGenerator>();
builder.Services.AddScoped<IOpenAIRepository, OpenAIRepository>();
builder.Services.AddScoped<IRoadmapCreator, RoadmapCreator>();
builder.Services.AddScoped<IGithubRepository, GithubRepository>();
builder.Services.AddScoped<IGitHubGraphQLClient, GitHubGraphQLClient>();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapHealthChecks("/health");

app.Run();