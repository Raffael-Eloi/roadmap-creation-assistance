using System.Text.Json;
using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.UseCases;

public class CreateRoadmap(IDatabaseRepository databaseRepository, IQueueService enqueueService)
{
	public async Task<CreateRoadmapResponse> Execute(RoadmapCreationRequest request)
	{
		Guid jobId = Guid.NewGuid();

		AsyncTaskRequest asyncTask = new(jobId.ToString());

		await databaseRepository.Save(asyncTask);

		string content = JsonSerializer.Serialize(request);

		await enqueueService.Send(new SendMessage(jobId, content));

		return new CreateRoadmapResponse(jobId.ToString());
	}
}
