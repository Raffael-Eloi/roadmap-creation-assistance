namespace RoadmapCreationAssistance.API.Contracts.UseCases;

public interface IPromptProvider
{
    Task<string> GetRoadmapBaseAsync(string language, string apiDomainDefinition);

    Task<string> GetMilestoneInstructionAsync(string language, string apiDomainDefinition);
}