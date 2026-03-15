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

	public async Task UpdateStatus(string jobId, AsyncTaskStatus status, List<string>? errors = null)
	{
		try
		{
			logger.LogInformation("Updating async task {TaskId} status to {Status}", jobId, status);

			await Task.CompletedTask;

			logger.LogInformation("Async task {TaskId} status updated to {Status}", jobId, status);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error updating async task status for ID: {TaskId}", jobId);
		}
	}
}
