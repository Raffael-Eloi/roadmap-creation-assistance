using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.UseCases;

public record CreateRoadmapResponse(string JobId);

public enum AsyncTaskStatus
{
	Pedding,
	Starting,
	Processing,
	Completed,
	Error
}

public record AsyncTaskRequest(string JobId, AsyncTaskStatus Status = AsyncTaskStatus.Pedding, List<string>? Errors = null);

public class CreateRoadmap(IDatabaseRepository databaseRepository, IQueueService enqueueService)
{
	public async Task<CreateRoadmapResponse> Execute(RoadmapCreationRequest request)
	{
		Guid jobId = Guid.NewGuid();

		AsyncTaskRequest asyncTask = new(jobId.ToString());

		await databaseRepository.Save(asyncTask);

		string content = System.Text.Json.JsonSerializer.Serialize(request);

		await enqueueService.Send(new SendMessage(jobId, content));

		return new CreateRoadmapResponse(jobId.ToString());
	}
}

public record SendMessage(Guid Id, string Content);

public interface IQueueService
{
	Task Send(SendMessage sendMessage);
}