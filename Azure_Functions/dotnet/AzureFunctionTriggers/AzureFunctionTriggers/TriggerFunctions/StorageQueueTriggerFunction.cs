using AzureFunctionTriggers.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureFunctionTriggers.TriggerFunctions
{
	public class StorageQueueTriggerFunction
	{
		private readonly ILogger<StorageQueueTriggerFunction> _logger;

		public StorageQueueTriggerFunction(ILogger<StorageQueueTriggerFunction> logger)
		{
			_logger = logger;
		}

		[FunctionName(nameof(StorageQueueTriggerFunction))]
		public async Task Run([QueueTrigger("test-queue", Connection = "DataStorageConnection")] QueueMessageDto message)
		{
			_logger.LogWarning($"[{nameof(StorageQueueTriggerFunction)}] => Received message: {JsonSerializer.Serialize(message)}");

			await Task.CompletedTask;
		}
	}
}