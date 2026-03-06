namespace RoadmapCreationAssistance.API.Contracts.Repositories;

public interface IDatabaseRepository
{
	Task Save(API.UseCases.AsyncTaskRequest asyncTask);
}