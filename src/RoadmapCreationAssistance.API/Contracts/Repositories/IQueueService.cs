using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.Contracts.Repositories;

public interface IQueueService
{
	Task Send(SendMessage sendMessage);
}