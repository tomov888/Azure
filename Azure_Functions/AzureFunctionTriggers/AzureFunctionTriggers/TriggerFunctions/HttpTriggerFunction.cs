using AzureFunctionTriggers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureFunctionTriggers.TriggerFunctions
{
	public static class HttpTriggerFunction
	{
		// POST http://localhost:7239/api/HttpTriggerFunction
		// Body: {"Name":"nenad"}

		[FunctionName(nameof(HttpTriggerFunction))]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");

			string payload = await req.ReadAsStringAsync();

			var deserializedPayload = JsonSerializer.Deserialize<HttpTriggerPayloadDto>(payload);

			log.LogInformation($"Name : {deserializedPayload.Name}");
			log.LogInformation($"Payload : {deserializedPayload}");

			return new OkObjectResult(deserializedPayload);
		}
	}
}