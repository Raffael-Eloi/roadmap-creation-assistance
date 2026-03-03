using Microsoft.AspNetCore.Mvc;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class RoadMapGeneratorController(IRoadmapCreator roadmapCreator, ILogger<RoadMapGeneratorController> logger) : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        logger.LogInformation("Ping endpoint hit at {Time}", DateTime.UtcNow);
        logger.LogWarning("This is a test warning log for Datadog");
        return Ok(new { message = "pong", timestamp = DateTime.UtcNow });
    }

    [HttpPost]
    public async Task<IActionResult> GenerateRoadMap([FromBody] RoadmapCreationRequest request)
    {
        logger.LogInformation("Roadmap generation request received for repository {Owner}/{Repo}", request.GitHubOwner, request.GitHubRepositoryName);

        RoadmapCreationResponse response = await roadmapCreator.CreateAsync(request);

        return Ok(response);
    }
}