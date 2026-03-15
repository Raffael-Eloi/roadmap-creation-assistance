using FluentAssertions;
using Moq;
using RoadmapCreationAssistance.API.Contracts.Repositories;
using RoadmapCreationAssistance.API.Contracts.Services;
using RoadmapCreationAssistance.API.Models;
using RoadmapCreationAssistance.API.UseCases;

namespace RoadmapCreationAssistance.API.Tests.UseCases;

public class CreateRoadmapShould
{
	private Mock<IDatabaseRepository> databaseRepositoryMock;
	private Mock<IQueueService> queueServiceMock;
	private CreateRoadmap createRoadmap;
	private RoadmapCreationRequest request;

	[SetUp]
	public void Setup()
	{
		databaseRepositoryMock = new Mock<IDatabaseRepository>();
		queueServiceMock = new Mock<IQueueService>();
		createRoadmap = new CreateRoadmap(databaseRepositoryMock.Object, queueServiceMock.Object);

		request = new RoadmapCreationRequest
		{
			GitHubOwner = "John",
			GitHubRepositoryName = "My repo",
			GitHubToken = "MYTOKEN",
			OpenAIKey = "MYOPENAIKEY",
			ApiDomainDefinition = "MY API SPECS"
		};
	}

	[Test]
	public async Task Save_Async_Task_To_Database()
	{
		#region Arrange

		#endregion

		#region Act

		await createRoadmap.Execute(request);

		#endregion

		#region Assert

		databaseRepositoryMock
			.Verify(repo => repo.Save(It.IsAny<AsyncTaskRequest>()),
			Times.Once);

		#endregion
	}

	[Test]
	public async Task Enqueue_Message_To_Queue()
	{
		#region Arrange

		#endregion

		#region Act

		await createRoadmap.Execute(request);

		#endregion

		#region Assert

		queueServiceMock
			.Verify(queue => queue.SendAsync(It.Is<QueueMessage>(msg =>
				msg.QueueName == "create-roadmap-queue")),
			Times.Once);

		#endregion
	}

	[Test]
	public async Task Return_Response_With_JobId()
	{
		#region Arrange

		#endregion

		#region Act

		CreateRoadmapResponse response = await createRoadmap.Execute(request);

		#endregion

		#region Assert

		response.Should().NotBeNull();
		response.JobId.Should().NotBeNullOrEmpty();
		Guid.TryParse(response.JobId, out _).Should().BeTrue();

		#endregion
	}

	[Test]
	public async Task Serialize_Request_To_Queue_Message()
	{
		#region Arrange

		#endregion

		#region Act

		await createRoadmap.Execute(request);

		#endregion

		#region Assert

		queueServiceMock
			.Verify(queue => queue.SendAsync(It.Is<QueueMessage>(msg =>
				msg.Content.Contains(request.GitHubOwner) &&
				msg.Content.Contains(request.GitHubRepositoryName))),
			Times.Once);

		#endregion
	}

	[Test]
	public async Task Update_Status_To_Error_On_Queue_Send_Failure()
	{
		#region Arrange

		queueServiceMock
			.Setup(queue => queue.SendAsync(It.IsAny<QueueMessage>()))
			.ThrowsAsync(new Exception("Queue service failed"));

		#endregion

		#region Act & Assert

		Assert.ThrowsAsync<Exception>(() => createRoadmap.Execute(request));

		databaseRepositoryMock
			.Verify(repo => repo.UpdateStatus(
				It.IsAny<string>(),
				AsyncTaskStatus.Error,
				It.Is<List<string>>(errors => errors != null && errors.Count > 0)),
			Times.Once);

		#endregion
	}
}
