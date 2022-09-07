using FunctionApp.CalcRequestProcessingExample.Dto;
using FunctionApp.CalcRequestProcessingExample.Orchestration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.PayrunBatchProcessingExample.DurableEntity
{
	public record PayrunBatchDurableEntityInitializeDto 
	{
		[JsonProperty("messageLabel")]
		public string MessageLabel { get; set; }

		[JsonProperty("clientId")]
		public int ClientId { get; set; }

		[JsonProperty("batchId")]
		public int BatchId { get; set; }

		[JsonProperty("employeeIds")]
		public List<int> EmployeeIds { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class PayrunBatchDurableEntity
	{
		[JsonProperty("messageLabel")]
		public string MessageLabel { get; set; }

		[JsonProperty("clientId")]
		public int ClientId { get; set; }

		[JsonProperty("batchId")]
		public int BatchId { get; set; }

		[JsonProperty("employeeIds")]
		public List<int> EmployeeIds { get; set; }

		[JsonProperty("processedEmployeeIds")]
		public List<int> ProcessedEmployeeIds { get; set; }

		[JsonIgnore]
		private readonly ILogger _logger;

		public PayrunBatchDurableEntity(ILogger logger)
		{
			_logger = logger;
		}

		[FunctionName(nameof(PayrunBatchDurableEntity))]
		public static Task Run([EntityTrigger] IDurableEntityContext ctx, ILogger logger) => ctx.DispatchAsync<PayrunBatchDurableEntity>(logger);

		public Task Initialize(PayrunBatchDurableEntityInitializeDto dto)
		{
			_logger.LogWarning($"[{nameof(PayrunBatchDurableEntity)}]::[{nameof(Initialize)}] => Start");

			MessageLabel = dto.MessageLabel;
			ClientId = dto.ClientId;
			EmployeeIds = dto.EmployeeIds;
			BatchId = dto.BatchId;
			ProcessedEmployeeIds = new List<int>();

			_logger.LogWarning($"[{nameof(PayrunBatchDurableEntity)}]::[{nameof(Initialize)}] => End");
			return Task.CompletedTask;
		}

		public Task EmployeeProcessed(int employeeId)
		{
			_logger.LogWarning($"[{nameof(PayrunBatchDurableEntity)}]::[{nameof(EmployeeProcessed)}] => Start");

			var untillNowProcessed = ProcessedEmployeeIds.Select(x => x).ToList();
			untillNowProcessed.Add(employeeId);

			ProcessedEmployeeIds = untillNowProcessed.Distinct().ToList();

			_logger.LogWarning($"[{nameof(PayrunBatchDurableEntity)}]::[{nameof(EmployeeProcessed)}] => Processed {ProcessedEmployeeIds.Count} / {EmployeeIds.Count}");

			if (ProcessedEmployeeIds.Count == EmployeeIds.Count)
			{
				_logger.LogWarning($"[{nameof(PayrunBatchDurableEntity)}]::[{nameof(EmployeeProcessed)}] => All Employees have been processed successfully, starting {nameof(PayrunBatchOrchestrationFunction.PayrunBatchOrchestrationSummary)}.");
				Entity.Current.StartNewOrchestration(nameof(PayrunBatchOrchestrationFunction.PayrunBatchOrchestrationSummary), new PayrunBatchProcessingCompletedNotificationDto { MessageLabel = MessageLabel, BatchId = BatchId});
			}

			_logger.LogWarning($"[{nameof(PayrunBatchDurableEntity)}]::[{nameof(EmployeeProcessed)}] => End");
			return Task.CompletedTask;
		}
	}
}
