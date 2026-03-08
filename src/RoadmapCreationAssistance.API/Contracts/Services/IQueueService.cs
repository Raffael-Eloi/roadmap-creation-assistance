using Azure.Storage.Queues;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.Contracts.Services;

public interface IQueueService
{
	Task SendAsync(QueueMessage message);
	Task<string?> RetrieveNextAsync(string queueName);
}