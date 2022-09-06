using AzureFunctionTriggers.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureFunctionTriggers.TriggerFunctions
{
	public class ServiceBusTopicTriggerFunction
	{
		private readonly ILogger<ServiceBusTopicTriggerFunction> _logger;

		//public ServiceBusTopicTriggerFunction(ILogger<ServiceBusTopicTriggerFunction> log)
		//{
		//	_logger = log;
		//}

		//[FunctionName("ServiceBusTopicTriggerFunction")]
		//public async Task RunAsync(
		//	[ServiceBusTrigger(
		//		"test-topic", // topic name
		//		"test-topic-subscriber", // topic subscribtion name
		//		Connection = "ServiceBusConnectionString" // name of connection string param that holds service bus connection string value
		//	)]
		//	ServiceBusTopicMessageDto message
		//)
		//{
		//	_logger.LogWarning($"[{nameof(ServiceBusTopicTriggerFunction)}] => Received message: {System.Text.Json.JsonSerializer.Serialize(message)}");

		//	_logger.LogWarning($"[{nameof(ServiceBusTopicTriggerFunction)}] => Processed message.");

		//	await Task.CompletedTask;
		//}
	}
}