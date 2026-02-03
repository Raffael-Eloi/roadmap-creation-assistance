using RoadmapCreationAssistance.API.Contracts.UseCases;

namespace RoadmapCreationAssistance.API.UseCases
{
    public class PromptProvider : IPromptProvider
    {
        public async Task<string> GetMilestoneInstructionAsync(string language)
        {
            string prompt = await GetRoadmapBaseAsync(language);
            
            string currentDirectory = Directory.GetCurrentDirectory();
            string milestonesGenerationPromptPath = Path.Combine(currentDirectory, "PromptBase", "milestones_generation_instruction.md");
            string milestonesGenerationPrompt = await File.ReadAllTextAsync(milestonesGenerationPromptPath);

            prompt += Environment.NewLine + milestonesGenerationPrompt + Environment.NewLine;

            return prompt;
        }

        public async Task<string> GetRoadmapBaseAsync(string language)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string promptPath = Path.Combine(currentDirectory, "PromptBase", "roadmap_base.md");
            string prompt = await File.ReadAllTextAsync(promptPath);
            prompt += $"All documentation, milestones, issues and code must be written **in {language}**";
            return prompt;
        }
    }
}