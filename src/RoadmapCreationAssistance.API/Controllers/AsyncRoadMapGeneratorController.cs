using Microsoft.AspNetCore.Mvc;
using RoadmapCreationAssistance.API.Models;
using RoadmapCreationAssistance.API.UseCases;

namespace RoadmapCreationAssistance.API.Controllers;

[Route("api/roadmap/async")]
[ApiController]
public sealed class AsyncRoadMapGeneratorController(CreateRoadmap createRoadmapUseCase, ILogger<AsyncRoadMapGeneratorController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateRoadmapResponse), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> GenerateRoadMapAsync([FromBody] RoadmapCreationRequest request)
    {
        logger.LogInformation("Async roadmap generation request received for repository {Owner}/{Repo}", request.GitHubOwner, request.GitHubRepositoryName);

        CreateRoadmapResponse response = await createRoadmapUseCase.Execute(request);

        return Accepted(response);
    }
}