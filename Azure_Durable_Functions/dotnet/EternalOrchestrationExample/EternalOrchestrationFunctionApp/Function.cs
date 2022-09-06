using EternalOrchestrationFunctionApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EternalOrchestrationFunctionApp
{
	public static class Function
	{
		[FunctionName(nameof(Orchestrator))]
		public static async Task Orchestrator([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
		{
			var state = context.GetInput<StateDto>();

			if (state.Counter == state.TimesToRun) return;

			log.LogWarning($"Running function for {state.Counter} time.");

			var tasks = Enumerable.Repeat(10, 10).Select(x => context.CallActivityAsync<string>(nameof(ActivityFunction), x));
			
			await Task.WhenAll(tasks); // fan out


			try
			{
				await context.CallActivityWithRetryAsync<string>(
					nameof(RetryableActivityFunction),
					new RetryOptions(TimeSpan.FromSeconds(1), 3)
					{
						Handle = ex =>
						{
							var isCustomException = ex.InnerException is CustomeException;
							if (isCustomException) log.LogWarning($"it is custome exception, so it will be handled");
							return isCustomException;
						}
					},
					new Random().Next(0, 10)
				);
			}
			catch (Exception ex)
			{
				log.LogWarning($"Retryable function resulted in exception: {ex.Message}");
				log.LogWarning($"Retryable function resulted in inner exception: {ex.InnerException.Message}");
			}

			state.Counter++;

			context.ContinueAsNew(state);
		}

		[FunctionName(nameof(ActivityFunction))]
		public static string ActivityFunction([ActivityTrigger] int taskId, ILogger log)
		{
			log.LogInformation($"ActivityFunction => {taskId}.");
			return $"{taskId}-processed";
		}

		[FunctionName(nameof(RetryableActivityFunction))]
		public static string RetryableActivityFunction([ActivityTrigger] int taskId, ILogger log)
		{
			log.LogInformation($"RetryableActivityFunction => {taskId}.");

			if (taskId % 2 != 0) throw new CustomeException($"task id is {taskId}");

			return $"{taskId}-processed";
		}


		[FunctionName(nameof(HttpStart))]
		public static async Task<HttpResponseMessage> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
			[DurableClient] IDurableOrchestrationClient starter,
			ILogger log)
		{
			string instanceId = await starter.StartNewAsync(nameof(Orchestrator), new StateDto { Counter = 0, TimesToRun = 10});
			log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
			return starter.CreateCheckStatusResponse(req, instanceId);
		}
	}
}