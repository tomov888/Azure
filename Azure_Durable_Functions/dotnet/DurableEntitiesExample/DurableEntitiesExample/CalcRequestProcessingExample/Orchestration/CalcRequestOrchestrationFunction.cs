using FunctionApp.CalcRequestProcessingExample.Dto;
using FunctionApp.CalcRequestProcessingExample.DurableEntity;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionApp.CalcRequestProcessingExample.Orchestration
{
	public static class CalcRequestOrchestrationFunction
	{
		[FunctionName(nameof(CalcRequestStarter))]
		public static async Task<HttpResponseMessage> CalcRequestStarter(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
			[DurableClient] IDurableOrchestrationClient client,
			ILogger log)
		{
			var payload = JsonConvert.DeserializeObject<CalcRequestInfoDto>(await req.Content.ReadAsStringAsync());

			string instanceId = await client.StartNewAsync(nameof(CalcRequestOrchestration), payload);

			log.LogWarning($"Started orchestration with ID = '{instanceId}'.");

			return client.CreateCheckStatusResponse(req, instanceId);
		}

		[FunctionName(nameof(CalcRequestOrchestration))]
		public static async Task CalcRequestOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
		{
			//============================================
			// 1) Get Payrun Batches
			// 2) Initialize CalcRequestDurableEntity
			// 3) Fan Out on PayrunBatch processing
			//============================================

			logger.LogWarning($"{nameof(CalcRequestOrchestration)} => Start");

			var input = context.GetInput<CalcRequestInfoDto>();

			logger.LogWarning($"{nameof(CalcRequestOrchestration)} => Input: {System.Text.Json.JsonSerializer.Serialize(input)}");

			var payrunBatches = await context.CallActivityAsync<List<int>>(nameof(CalcRequestGetPayrunBatchesActivity), new { });

			var initDto = new CalcRequestDurableEntityInitializeDto
			{
				ClientId = input.ClientId,
				MessageLabel = input.MessageLabel,
				PayrunBatchIds = payrunBatches.Select(x => x).ToList()
			};

			await context.CallActivityAsync(nameof(CalcRequestInitCalcRequestDurableEntityActivity), initDto);

			var signalProcessPayrunBatchesActivityTasks = payrunBatches.Select(x => x).Select(x => context.CallActivityAsync(nameof(CalcRequestSignalProcessPayrunBatchActivity), new BatchInfoDto { BatchId = x, ClientId = input.ClientId, MessageLabel = input.MessageLabel }));

			await Task.WhenAll(signalProcessPayrunBatchesActivityTasks);

			logger.LogWarning($"{nameof(CalcRequestOrchestration)} => End");
		}

		[FunctionName(nameof(CalcRequestSignalProcessPayrunBatchActivity))]
		public static async Task CalcRequestSignalProcessPayrunBatchActivity(
			[ActivityTrigger] BatchInfoDto input,
			[DurableClient] IDurableOrchestrationClient durableOrchestrationClient,
			ILogger log)
		{
			log.LogWarning($"{nameof(CalcRequestSignalProcessPayrunBatchActivity)} => Start");
			log.LogWarning($"{nameof(CalcRequestSignalProcessPayrunBatchActivity)} => Processing batchId: {input.BatchId}");
			await durableOrchestrationClient.StartNewAsync(nameof(PayrunBatchOrchestrationFunction.PayrunBatchOrchestration), $"{input.ClientId}-{input.BatchId}-PayrunBatch", input);
			log.LogWarning($"{nameof(CalcRequestSignalProcessPayrunBatchActivity)} => End");
		}

		[FunctionName(nameof(CalcRequestGetPayrunBatchesActivity))]
		public static List<int> CalcRequestGetPayrunBatchesActivity([ActivityTrigger] object dto, ILogger log)
		{
			log.LogWarning($"{nameof(CalcRequestGetPayrunBatchesActivity)} => Start");

			log.LogWarning($"{nameof(CalcRequestGetPayrunBatchesActivity)} => End");

			var random = new Random();
			return Enumerable.Repeat(0, 10).Select(x => random.Next(500, 600)).ToList();
		}

		[FunctionName(nameof(CalcRequestInitCalcRequestDurableEntityActivity))]
		public static async Task CalcRequestInitCalcRequestDurableEntityActivity(
			[ActivityTrigger] CalcRequestDurableEntityInitializeDto dto,
			[DurableClient] IDurableEntityClient client,
			ILogger log)
		{
			log.LogWarning($"{nameof(CalcRequestInitCalcRequestDurableEntityActivity)} => Start");

			var entityId = new EntityId(nameof(CalcRequestDurableEntity), dto.MessageLabel);
			await client.SignalEntityAsync(entityId, nameof(CalcRequestDurableEntity.Initialize), dto);

			log.LogWarning($"[{nameof(CalcRequestInitCalcRequestDurableEntityActivity)}] => Created DurableEntity with EntityId = {dto.MessageLabel}");

			log.LogWarning($"{nameof(CalcRequestInitCalcRequestDurableEntityActivity)} => End");
		}

		[FunctionName(nameof(CalcRequestOrchestrationSummary))]
		public static async Task CalcRequestOrchestrationSummary([OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			Console.WriteLine("CalcRequestCompletedOrchestration => started");
			Console.WriteLine("### CALC REQUEST COMPLETED ###");
			Console.WriteLine("CalcRequestCompletedOrchestration => ended");
			await Task.CompletedTask;
		}
	}
}