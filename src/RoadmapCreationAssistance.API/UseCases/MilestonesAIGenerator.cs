using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Entities;
using System.Text.Json;

namespace RoadmapCreationAssistance.API.UseCases;

public class MilestonesAIGenerator(IOpenAIRepository openAIRepository) : IMilestonesAIGenerator
{
    public async Task<IEnumerable<Milestone>> GenerateWithIssues()
    {
        string prompt = await GetRoadmapPrompt();

        string response = await openAIRepository.GetResponse(prompt);

        IEnumerable<Milestone>? milestones = JsonSerializer.Deserialize<IEnumerable<Milestone>>(response);

        if (milestones is null)
            return [];

        return milestones;
    }

    private static async Task<string> GetRoadmapPrompt()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string promptPath = Path.Combine(currentDirectory, "PromptBase", "roadmap_base.md");
        string prompt = await File.ReadAllTextAsync(promptPath);

        string extraInstructionToThePrompt = """
            Given this prompt, I want you to generate milestones with issues based on the content.
            IMPORTANT: The answer needs to be exactly an array of Milestones, because I will deserialize the answer to this model in my .NET application.
            Here's my classes:
            public class Milestone
            {
                public int Id { get; set; }

                public required string Title { get; set; }

                public string Description { get; set; } = string.Empty;

                public string State { get; set; } = string.Empty;

                public IEnumerable<Issue> Issues { get; set; } = [];
            }

            public class Issue
            {
                public required string Title { get; set; }

                public string Body { get; set; } = string.Empty;
            }
        """;

        return prompt + extraInstructionToThePrompt;
    }
}