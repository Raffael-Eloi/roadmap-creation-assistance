using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using RoadmapCreationAssistance.API.Contracts.Services;
using AzQueueMessage = Azure.Storage.Queues.Models.QueueMessage;

namespace RoadmapCreationAssistance.API.Services;

public class QueueServices(ILogger<QueueServices> _logger, string _connectionString) : IQueueService
{
	public async Task SendAsync(Models.QueueMessage message)
	{
		QueueClientOptions options = new QueueClientOptions
		{
			MessageEncoding = QueueMessageEncoding.Base64
		};

		QueueClient queue = new(_connectionString, message.QueueName, options);

		if (await queue.CreateIfNotExistsAsync() != null)
		{
			_logger.LogInformation("The queue was created.");
		}

		await queue.SendMessageAsync(message.Content);
	}

	public async Task<string?> RetrieveNextAsync(string queueName)
	{
		QueueClientOptions options = new QueueClientOptions
		{
			MessageEncoding = QueueMessageEncoding.Base64
		};

		QueueClient queue = new(_connectionString, queueName, options);

		if (!await queue.ExistsAsync())
		{
			_logger.LogInformation("The queue does not exists.");
			return null;
		}

		QueueProperties properties = await queue.GetPropertiesAsync();

		if (properties.ApproximateMessagesCount == 0)
		{
			_logger.LogInformation("The queue is empty. Attempt to delete it");

			await queue.DeleteIfExistsAsync();

			return null;
		}

		AzQueueMessage[] retrievedMessage = await queue.ReceiveMessagesAsync(1);
		string theMessage = retrievedMessage[0].Body.ToString();
		await queue.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);
		return theMessage;
	}
}
