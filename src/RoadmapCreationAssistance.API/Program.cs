using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Middlewares;
using RoadmapCreationAssistance.API.Repositories.Github;
using RoadmapCreationAssistance.API.Repositories.OpenAI;
using RoadmapCreationAssistance.API.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IMilestonesAIGenerator, MilestonesAIGenerator>();
builder.Services.AddScoped<IReadmeAIGenerator, ReadmeAIGenerator>();
builder.Services.AddScoped<IOpenAIRepository, OpenAIRepository>();
builder.Services.AddScoped<IRoadmapCreator, RoadmapCreator>();
builder.Services.AddScoped<IGithubRepository, GithubRepository>();

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

app.Run();