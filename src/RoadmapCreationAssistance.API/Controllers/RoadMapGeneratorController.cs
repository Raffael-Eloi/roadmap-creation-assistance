using Microsoft.AspNetCore.Mvc;

namespace RoadmapCreationAssistance.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoadMapGeneratorController : ControllerBase
{
    [HttpPost]
    public IActionResult GenerateRoadMap()
    {
        return Ok("success");
    }
}