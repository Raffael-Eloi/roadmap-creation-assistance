using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.Functions;

public class Function1(ILogger<Function1> logger, IRoadmapCreator _roadmapCreator)
{
	private readonly ILogger<Function1> _logger = logger;

	[Function(nameof(Function1))]
	public async Task Run(
		[QueueTrigger("create-roadmap-queue", 
		Connection = "AzureWebJobsStorage")] string message)
    {
		try
		{
			RoadmapCreationRequest? request = JsonSerializer.Deserialize<RoadmapCreationRequest>(message);

			if (request == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			RoadmapCreationResponse? response = await _roadmapCreator.CreateAsync(request);

			_logger.LogInformation("Roadmap created successfully. Project ID: {projectID}, " + response.ProjectId);

		}
		catch (Exception ex)
		{
			_logger.LogError("Error on creating roadmap {ex}", ex);
		}
	}
}