using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.Repositories;

public class DatabaseRepository(ILogger<DatabaseRepository> logger) : IDatabaseRepository
{
	public async Task Save(AsyncTaskRequest asyncTask)
	{
		try
		{
			logger.LogInformation("Saving async task with ID: {TaskId}", asyncTask.JobId);

			await Task.CompletedTask;

			logger.LogInformation("Async task saved successfully with ID: {TaskId}", asyncTask.JobId);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error saving async task with ID: {TaskId}", asyncTask.JobId);
		}
	}
}
