using FunctionApp.CalcRequestProcessingExample.Dto;
using FunctionApp.CalcRequestProcessingExample.DurableEntity;
using FunctionApp.CalcRequestProcessingExample.DurableEntity.Helpers;
using FunctionApp.PayrunBatchProcessingExample.DurableEntity;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FunctionApp.CalcRequestProcessingExample.Orchestration
{

	public static class PayrunBatchOrchestrationFunction
	{
		[FunctionName(nameof(PayrunBatchOrchestration))]
		public static async Task PayrunBatchOrchestration(
			[OrchestrationTrigger] IDurableOrchestrationContext context,
			
			ILogger logger)
		{

			//============================================
			// 1) Get Employees for Payrun Batch
			// 2) Initialize PayrunBatchDurableEntity
			// 3) Fan Out on Single Employee Processing
			//============================================

			logger.LogWarning($"[{nameof(PayrunBatchOrchestration)}]::[{context.InstanceId}] => Start");

			var input = context.GetInput<BatchInfoDto>();
			
			var employeesForPayrunBatch = await context.CallActivityAsync<List<int>>(nameof(PayrunBatchOrchestrationGetEmployeesActivity), input);

			var durableEntityCreated = await context.CallActivityAsync<bool>(nameof(PayrunBatchOrchestrationInitializePayrunBatchDurableEntityActivity), new BatchEmployeesDto { BatchInfo = input, EmployeeIds = employeesForPayrunBatch});

			var signalSignelEmployeeProcessingTasks = employeesForPayrunBatch
				.Select(x => context.CallActivityAsync(nameof(PayrunBatchOrchestrationSignalSingleEmployeeProcessingActivity), new EmployeeInfoDto { ClientId = input.ClientId, BatchId = input.BatchId, EmployeeId = x, MessageLabel = input.MessageLabel}))
				.ToList();

			await Task.WhenAll(signalSignelEmployeeProcessingTasks);

			logger.LogWarning($"[{nameof(PayrunBatchOrchestration)}]::[{context.InstanceId}] => End");
		}

		[FunctionName(nameof(PayrunBatchOrchestrationGetEmployeesActivity))]
		public static async Task<List<int>> PayrunBatchOrchestrationGetEmployeesActivity(
			[ActivityTrigger] BatchInfoDto input,
			[DurableClient] IDurableEntityClient durableEntityClient,
			ILogger log)
		{
			
			log.LogWarning($"[{nameof(PayrunBatchOrchestrationGetEmployeesActivity)}] => Start");
			
			log.LogWarning($"[{nameof(PayrunBatchOrchestrationGetEmployeesActivity)}] => End");

			var random = new Random();
			await Task.CompletedTask;
			return Enumerable.Repeat(0, 10).Select(x => random.Next(1000, 2000)).ToList(); ;
		}

		[FunctionName(nameof(PayrunBatchOrchestrationInitializePayrunBatchDurableEntityActivity))]
		public static async Task<bool> PayrunBatchOrchestrationInitializePayrunBatchDurableEntityActivity(
			[ActivityTrigger] BatchEmployeesDto input,
			[DurableClient] IDurableEntityClient durableEntityClient,
			ILogger log)
		{
			log.LogWarning($"[{nameof(PayrunBatchOrchestrationInitializePayrunBatchDurableEntityActivity)}] => Start");


			PayrunBatchDurableEntityInitializeDto operationInput = new PayrunBatchDurableEntityInitializeDto
			{
				BatchId = input.BatchInfo.BatchId,
				ClientId = input.BatchInfo.ClientId,
				EmployeeIds = input.EmployeeIds,
				MessageLabel = input.BatchInfo.MessageLabel
			};
			EntityId entityId = DurableEntityHelpers.PayrunBatchDurableEntityKey(input.BatchInfo.ClientId, input.BatchInfo.BatchId);

			await durableEntityClient.SignalEntityAsync(entityId, nameof(PayrunBatchDurableEntity.Initialize), operationInput);

			log.LogWarning($"[{nameof(PayrunBatchOrchestrationInitializePayrunBatchDurableEntityActivity)}] => End");

			return true;
		}

		[FunctionName(nameof(PayrunBatchOrchestrationSignalSingleEmployeeProcessingActivity))]
		public static async Task PayrunBatchOrchestrationSignalSingleEmployeeProcessingActivity(
			[ActivityTrigger] EmployeeInfoDto input,
			[DurableClient] IDurableOrchestrationClient durableOrchestrationClient,
			ILogger log)
		{
			log.LogWarning($"{nameof(PayrunBatchOrchestrationSignalSingleEmployeeProcessingActivity)} => Start");
			await durableOrchestrationClient.StartNewAsync(nameof(SingleEmployeeOrchestrationFunction.SingleEmployeeOrchestration), $"{input.ClientId}-{input.BatchId}-{input.EmployeeId}-Employee", input);
			log.LogWarning($"{nameof(PayrunBatchOrchestrationSignalSingleEmployeeProcessingActivity)} => End");
		}

		[FunctionName(nameof(PayrunBatchOrchestrationSummary))]
		public static async Task PayrunBatchOrchestrationSummary(
			[OrchestrationTrigger] IDurableOrchestrationContext context,
			ILogger logger)
		{
			var input = context.GetInput<PayrunBatchProcessingCompletedNotificationDto>();
			logger.LogWarning($"[{nameof(PayrunBatchOrchestrationSummary)}] => Start");
			logger.LogWarning($"[{nameof(PayrunBatchOrchestrationSummary)}] => ######### PayrunBatch: {input.BatchId} processing completed. ######### ");

			await context.CallActivityAsync(nameof(PayrunBatchOrchestrationSummarySignalPayrunBatchProcessedActivity), input);

			await Task.CompletedTask;
			logger.LogWarning($"[{nameof(PayrunBatchOrchestrationSummary)}] => End");
		}

		[FunctionName(nameof(PayrunBatchOrchestrationSummarySignalPayrunBatchProcessedActivity))]
		public static async Task PayrunBatchOrchestrationSummarySignalPayrunBatchProcessedActivity(
			[ActivityTrigger] PayrunBatchProcessingCompletedNotificationDto input,
			[DurableClient] IDurableEntityClient durableEntityClient,
			ILogger logger)
		{
			logger.LogWarning($"{nameof(PayrunBatchOrchestrationSummarySignalPayrunBatchProcessedActivity)} => Start");
			
			var entityId = new EntityId(nameof(CalcRequestDurableEntity), input.MessageLabel);
			logger.LogWarning($"[{nameof(PayrunBatchOrchestrationSummary)}] => Signaling to {entityId.ToString()} that PayrunBatch: {input.BatchId} is processed ");
			await durableEntityClient.SignalEntityAsync(entityId, nameof(CalcRequestDurableEntity.PayrunBatchProcessed), input.BatchId);
			
			logger.LogWarning($"{nameof(PayrunBatchOrchestrationSummarySignalPayrunBatchProcessedActivity)} => End");
		}

	}
}