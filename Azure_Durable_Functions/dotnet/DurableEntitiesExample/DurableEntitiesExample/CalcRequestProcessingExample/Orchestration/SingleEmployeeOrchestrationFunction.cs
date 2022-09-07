using FunctionApp.CalcRequestProcessingExample.Dto;
using FunctionApp.CalcRequestProcessingExample.DurableEntity.Helpers;
using FunctionApp.PayrunBatchProcessingExample.DurableEntity;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FunctionApp.CalcRequestProcessingExample.Orchestration
{
	public static class SingleEmployeeOrchestrationFunction
	{
		[FunctionName(nameof(SingleEmployeeOrchestration))]
		public static async Task SingleEmployeeOrchestration(
			[OrchestrationTrigger] IDurableOrchestrationContext context,

			ILogger logger)
		{
			//============================================
			// 1) Process Employee
			// 2) Signal EmployeeProcessed to PayrunBatchDurableEntity
			//============================================

			logger.LogWarning($"[{nameof(SingleEmployeeOrchestration)}]::[{context.InstanceId}] => Start");

			var input = context.GetInput<EmployeeInfoDto>();

			await context.CallActivityAsync(nameof(SingleEmployeeOrchestrationProcessSingleEmployeeActivity), input);

			var payrunBatchDurableEntityUpdated = await context.CallActivityAsync<bool>(nameof(SingleEmployeeOrchestrationSignalEmployeeProcessedActivity), input);

			logger.LogWarning($"[{nameof(SingleEmployeeOrchestration)}]::[{context.InstanceId}] => End");
		}

		[FunctionName(nameof(SingleEmployeeOrchestrationProcessSingleEmployeeActivity))]
		public static async Task SingleEmployeeOrchestrationProcessSingleEmployeeActivity(
			[ActivityTrigger] EmployeeInfoDto input,
			ILogger log)
		{
			log.LogWarning($"{nameof(SingleEmployeeOrchestrationProcessSingleEmployeeActivity)} => Start");
			log.LogWarning($"{nameof(SingleEmployeeOrchestrationProcessSingleEmployeeActivity)} => Processing employee: {input.EmployeeId}");
			log.LogWarning($"{nameof(SingleEmployeeOrchestrationProcessSingleEmployeeActivity)} => End");
			await Task.CompletedTask;
		}

		[FunctionName(nameof(SingleEmployeeOrchestrationSignalEmployeeProcessedActivity))]
		public static async Task SingleEmployeeOrchestrationSignalEmployeeProcessedActivity(
			[ActivityTrigger] EmployeeInfoDto input,
			[DurableClient] IDurableEntityClient durableEntityClient,
			ILogger log)
		{
			log.LogWarning($"{nameof(SingleEmployeeOrchestrationSignalEmployeeProcessedActivity)} => Start");

			EntityId entityId = DurableEntityHelpers.PayrunBatchDurableEntityKey(input.ClientId, input.BatchId);
			await durableEntityClient.SignalEntityAsync(entityId, nameof(PayrunBatchDurableEntity.EmployeeProcessed), input.EmployeeId);

			log.LogWarning($"{nameof(SingleEmployeeOrchestrationSignalEmployeeProcessedActivity)} => End");
			await Task.CompletedTask;
		}

	}
}