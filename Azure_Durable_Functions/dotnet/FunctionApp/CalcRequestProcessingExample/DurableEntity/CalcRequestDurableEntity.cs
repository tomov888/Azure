using FunctionApp.CalcRequestProcessingExample.Dto;
using FunctionApp.CalcRequestProcessingExample.Orchestration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.CalcRequestProcessingExample.DurableEntity
{

	[JsonObject(MemberSerialization.OptIn)]
	public class CalcRequestDurableEntity
	{
		[JsonProperty("messageLabel")]
		public string MessageLabel { get; set; }

		[JsonProperty("clientId")]
		public int ClientId { get; set; }

		[JsonProperty("payrunBatchIds")]
		public List<int> PayrunBatchIds { get; set; }

		[JsonProperty("processedPayrunBatchIds")]
		public List<int> ProcessedPayrunBatchIds { get; set; }

		[JsonIgnore]
		private readonly ILogger _logger;

		public CalcRequestDurableEntity(ILogger logger)
		{
			_logger = logger;
		}

		public Task Initialize(CalcRequestDurableEntityInitializeDto dto)
		{
			_logger.LogWarning($"[{nameof(CalcRequestDurableEntity)}]::[{nameof(Initialize)}] => Start");

			MessageLabel = dto.MessageLabel;
			ClientId = dto.ClientId;
			PayrunBatchIds = dto.PayrunBatchIds;
			ProcessedPayrunBatchIds = new List<int>();

			_logger.LogWarning($"[{nameof(CalcRequestDurableEntity)}]::[{nameof(Initialize)}] => End");
			return Task.CompletedTask;
		}

		public Task PayrunBatchProcessed(int payrunBatchId)
		{
			_logger.LogWarning($"[{nameof(CalcRequestDurableEntity)}]::[{nameof(PayrunBatchProcessed)}] => Start");

			var untillNowProcessed = ProcessedPayrunBatchIds.Select(x => x).ToList();
			untillNowProcessed.Add(payrunBatchId);

			ProcessedPayrunBatchIds = untillNowProcessed.Distinct().ToList();

			_logger.LogWarning($"[{nameof(CalcRequestDurableEntity)}]::[{nameof(PayrunBatchProcessed)}] => Processed {ProcessedPayrunBatchIds.Count} / {PayrunBatchIds.Count}");

			if (ProcessedPayrunBatchIds.Count == PayrunBatchIds.Count)
			{
				_logger.LogWarning($"[{nameof(CalcRequestDurableEntity)}]::[{nameof(PayrunBatchProcessed)}] => All PayrunBatches have been processed successfully, starting {nameof(CalcRequestOrchestrationFunction.CalcRequestOrchestrationSummary)}.");
				Entity.Current.StartNewOrchestration(nameof(CalcRequestOrchestrationFunction.CalcRequestOrchestrationSummary), MessageLabel);
			}

			_logger.LogWarning($"[{nameof(CalcRequestDurableEntity)}]::[{nameof(PayrunBatchProcessed)}] => End");
			return Task.CompletedTask;
		}

		[FunctionName(nameof(CalcRequestDurableEntity))]
		public static Task Run([EntityTrigger] IDurableEntityContext ctx, ILogger logger) => ctx.DispatchAsync<CalcRequestDurableEntity>(logger);

		[FunctionName(nameof(GetCalcRequestDurableEntity))]
		public static async Task<OkObjectResult> GetCalcRequestDurableEntity(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetCalcRequestDurableEntity/{id}")] HttpRequestMessage req,
			string id,
			[DurableClient] IDurableEntityClient client,
			ILogger logger)
		{
			logger.LogWarning($"[{nameof(CalcRequestDurableEntity)}]::[{nameof(GetCalcRequestDurableEntity)}] => Start");

			var entityId = new EntityId(nameof(CalcRequestDurableEntity), id);

			logger.LogWarning($"[{nameof(CalcRequestDurableEntity)}]::[{nameof(GetCalcRequestDurableEntity)}] => Reading entity state for Id: {entityId}");

			var entityState = await client.ReadEntityStateAsync<CalcRequestDurableEntity>(entityId);

			logger.LogWarning($"[{nameof(CalcRequestDurableEntity)}]::[{nameof(GetCalcRequestDurableEntity)}] => End");
			return new OkObjectResult(entityState);
		}
	}
}
