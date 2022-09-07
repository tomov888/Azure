using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionApp.ConcurrentDurableEntityCreationExample
{
	public static class DummyOrchestrationFunction
	{
		[FunctionName("DummyOrchestrationHttpStart")]
		public static async Task<HttpResponseMessage> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
			[DurableClient] IDurableOrchestrationClient starter,
			ILogger log)
		{
			string instanceId = await starter.StartNewAsync("DummyOrchestration", null);

			log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

			return starter.CreateCheckStatusResponse(req, instanceId);
		}

		[FunctionName("DummyOrchestration")]
		public static async Task RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			var label = "nenad-dymmy-entity";
			var dto = new DummyDurableEntityInitDto
			{
				Label = label,
				TaskIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
			};


			var activityTasks = dto.TaskIds.Select(x => context.CallActivityAsync("DummyOrchestrationActivity", dto));

			await Task.WhenAll(activityTasks);

			await Task.CompletedTask;
		}

		[FunctionName("DummyOrchestrationActivity")]
		public static async Task DummyOrchestrationActivity([ActivityTrigger] DummyDurableEntityInitDto dto, [DurableClient] IDurableEntityClient durableEntityClient, ILogger log)
		{
			log.LogWarning($"DummyOrchestrationActivity => {dto.Label}.");
			await durableEntityClient.SignalEntityAsync(new EntityId(nameof(DummyDurableEntity), dto.Label), nameof(DummyDurableEntity.Initialize), dto);
			await Task.CompletedTask;
		}

	}
}