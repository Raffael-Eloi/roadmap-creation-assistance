using RoadmapCreationAssistance.API.Models;

namespace RoadmapCreationAssistance.API.Contracts.UseCases
{
	public interface ICreateRoadmap
	{
		Task<CreateRoadmapResponse> Execute(RoadmapCreationRequest request);
	}
}