namespace RoadmapCreationAssistance.API.Contracts.UseCases;

public interface IPromptProvider
{
    Task<string> GetRoadmapBaseAsync(string language);

    Task<string> GetMilestoneInstructionAsync(string language);
}