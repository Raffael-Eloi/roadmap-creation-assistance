using Microsoft.AspNetCore.Mvc;
using RoadmapCreationAssistance.API.Contracts.UseCases;
using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoadMapGeneratorController(IRoadmapCreator roadmapCreator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> GenerateRoadMap([FromBody] RoadmapCreationRequest request)
    {
        RoadmapCreationResponse response = await roadmapCreator.CreateAsync(request);

        return Ok(response);
    }
}