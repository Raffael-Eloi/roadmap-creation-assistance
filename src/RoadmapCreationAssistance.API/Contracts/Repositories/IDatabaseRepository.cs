using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.Contracts.Repositories;

public interface IDatabaseRepository
{
	Task Save(AsyncTaskRequest asyncTask);
	Task UpdateStatus(string jobId, AsyncTaskStatus status, List<string>? errors = null);
}