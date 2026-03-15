using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace RoadmapCreationAssistance.Functions;

public class Function1(ILogger<Function1> logger)
{
	private readonly ILogger<Function1> _logger = logger;

	[Function(nameof(Function1))]
	public void Run(
		[QueueTrigger("create-roadmap-queue", 
		Connection = "AzureWebJobsStorage")] string message)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message);
        _logger.LogInformation("---");
	}
}