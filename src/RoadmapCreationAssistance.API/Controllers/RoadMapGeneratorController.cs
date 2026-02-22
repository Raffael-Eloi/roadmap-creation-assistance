using Microsoft.AspNetCore.Mvc;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class RoadMapGeneratorController(IRoadmapCreator roadmapCreator, ILogger<RoadMapGeneratorController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> GenerateRoadMap([FromBody] RoadmapCreationRequest request)
    {
        logger.LogInformation("Roadmap generation request received for repository {Owner}/{Repo}", request.GitHubOwner, request.GitHubRepositoryName);

        RoadmapCreationResponse response = await roadmapCreator.CreateAsync(request);

        return Ok(response);
    }
}