using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionApp.ConcurrentDurableEntityCreationExample
{
	public class DummyDurableEntityInitDto 
	{
		[JsonProperty("label")]
		public string Label { get; set; }

		[JsonProperty("taskIds")]
		public List<int> TaskIds { get; set; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class DummyDurableEntity
	{
		[JsonProperty("label")]
		public string Label { get; set; }

		[JsonProperty("taskIds")]
		public List<int> TaskIds { get; set; }

		[JsonProperty("processedTaskIds")]
		public List<int> ProcessedTaskIds { get; set; }

		[JsonIgnore]
		private readonly ILogger _logger;

		public DummyDurableEntity(ILogger logger)
		{
			_logger = logger;
		}
		
		[FunctionName(nameof(DummyDurableEntity))]
		public static Task Run([EntityTrigger] IDurableEntityContext ctx, ILogger logger) => ctx.DispatchAsync<DummyDurableEntity>(logger);

		public Task Initialize(DummyDurableEntityInitDto dto)
		{
			_logger.LogWarning($"[{nameof(DummyDurableEntity)}]::[{nameof(Initialize)}] => Start");

			Label = dto.Label;
			TaskIds = dto.TaskIds;
			ProcessedTaskIds = new List<int>();

			_logger.LogWarning($"[{nameof(DummyDurableEntity)}]::[{nameof(Initialize)}] => End");
			return Task.CompletedTask;
		}

		[FunctionName(nameof(GetDummyDurableEntity))]
		public static async Task<OkObjectResult> GetDummyDurableEntity(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetDummyDurableEntity/{id}")] HttpRequestMessage req,
			string id,
			[DurableClient] IDurableEntityClient client,
			ILogger logger)
		{
			logger.LogWarning($"[{nameof(DummyDurableEntity)}]::[{nameof(GetDummyDurableEntity)}] => Start");

			var entityId = new EntityId(nameof(DummyDurableEntity), id);

			logger.LogWarning($"[{nameof(DummyDurableEntity)}]::[{nameof(GetDummyDurableEntity)}] => Reading entity state for Id: {entityId}");

			var entityState = await client.ReadEntityStateAsync<DummyDurableEntity>(entityId);

			logger.LogWarning($"[{nameof(DummyDurableEntity)}]::[{nameof(GetDummyDurableEntity)}] => End");
			return new OkObjectResult(entityState);
		}
	}
}
