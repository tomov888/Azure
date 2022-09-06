using Azure.Storage.Queues;
using Azure;
using Azure.Storage.Queues.Models;
using QueueConsumer.Settings;
using QueueConsumer.Extensions;
using QueueConsumer.Messages;
using QueueConsumer.MessageHandlers;

namespace QueueConsumer.Services
{
	public class AzureStorageQueueConsumerService : BackgroundService
	{
		private readonly ILogger<AzureStorageQueueConsumerService> _logger;
		private readonly QueueClient _queueClient;
		private readonly MessageDispatcher _messageDispatcher;
		private readonly MessageDeserializer _messageDeserializer;
		private readonly AzureStorageQueueSettings _azureStorageQueueSettings;

		public AzureStorageQueueConsumerService(
			ILogger<AzureStorageQueueConsumerService> logger,
			QueueClient queueClient,
			MessageDispatcher messageDispatcher, 
			AzureStorageQueueSettings azureStorageQueueSettings,
			MessageDeserializer messageDeserializer)
		{
			_logger = logger;
			_queueClient = queueClient;
			_messageDispatcher = messageDispatcher;
			_azureStorageQueueSettings = azureStorageQueueSettings;
			_messageDeserializer = messageDeserializer;
		}

		protected override async Task ExecuteAsync(CancellationToken token)
		{
			await ConsumeQueue(token);
		}

		private async Task ConsumeQueue(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				await Task.Delay(_azureStorageQueueSettings.PollingFrequency, token);

				_logger.LogWarning($"[{DateTime.UtcNow.ToString("o")}]::[{nameof(AzureStorageQueueConsumerService)}] => Polling queue...");

				var queueMessages = await _queueClient.ReceiveMessagesAsync(maxMessages: _azureStorageQueueSettings.MaxNumberOfMessagesToDequeue, cancellationToken: token);

				_logger.LogWarning($"[{DateTime.UtcNow.ToString("o")}]::[{nameof(AzureStorageQueueConsumerService)}] => Dequeued {queueMessages.Value.Count()} messages");

				if (!queueMessages.Value.Any())
				{
					_logger.LogWarning($"[{DateTime.UtcNow.ToString("o")}]::[{nameof(AzureStorageQueueConsumerService)}] => No messages received !!!");
					continue;
				}

				await ProcessMessages(queueMessages, token);
			}
		}

		private async Task ProcessMessages(Response<QueueMessage[]> queueMessages, CancellationToken token)
		{
			foreach (var queueMessage in queueMessages.Value)
			{
				IMessage message;
				try
				{
					message = _messageDeserializer.Deserialize(queueMessage.MessageText);
				}
				catch (Exception ex)
				{
					_logger.LogCritical(ex, $"[{nameof(AzureStorageQueueConsumerService)}] => Exception happened while deserializing message : {queueMessage.MessageText}");
					continue;
				}

				try
				{
					if (!_messageDispatcher.CanHandleMessage(message))
					{
						_logger.LogWarning($"[{nameof(AzureStorageQueueConsumerService)}] => {nameof(MessageDispatcher)} cant handle message with type : {message.MessageTypeName}");
						await _queueClient.DeleteMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt, token);

						// TODO => add invalid message type message handler (move to deadletter queue etc...)
						continue;
					}

					await _messageDispatcher.DispatchAsync(message);

					await _queueClient.DeleteMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt, token);
				}
				catch (Exception ex)
				{
					_logger.LogCritical(ex, $"[{nameof(AzureStorageQueueConsumerService)}] => Exception occured while handling message : {message.SerializeToJson()}");
				}
			}
		}
	}
}
