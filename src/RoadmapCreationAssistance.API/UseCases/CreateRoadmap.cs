using System.Text.Json;
using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.Services;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.UseCases;

public class CreateRoadmap(IDatabaseRepository databaseRepository, IQueueService enqueueService)
{
	private readonly string queueName = "create-roadmap-queue";

	public async Task<CreateRoadmapResponse> Execute(RoadmapCreationRequest request)
	{
		Guid jobId = Guid.NewGuid();

		AsyncTaskRequest asyncTask = new(jobId.ToString());

		await databaseRepository.Save(asyncTask);

		try
		{
			string content = JsonSerializer.Serialize(request);
			await enqueueService.SendAsync(new QueueMessage(queueName, content));
		}
		catch (Exception ex)
		{
			await databaseRepository.UpdateStatus(jobId.ToString(), AsyncTaskStatus.Error, new List<string> { ex.Message });
			throw;
		}

		return new CreateRoadmapResponse(jobId.ToString());
	}
}
