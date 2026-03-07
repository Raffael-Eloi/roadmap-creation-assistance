namespace RoadmapCreationAssistance.API.Models;

public record AsyncTaskRequest(string JobId, AsyncTaskStatus Status = AsyncTaskStatus.Pedding, List<string>? Errors = null);
