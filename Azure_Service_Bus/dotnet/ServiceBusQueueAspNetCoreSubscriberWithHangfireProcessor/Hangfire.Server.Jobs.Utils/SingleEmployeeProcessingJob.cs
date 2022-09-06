using Domain;
using Domain.Services;
using Hangfire;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace Hangfire.Server.Jobs.Utils
{
	public class SingleEmployeeProcessingJob
	{
		private readonly ILogger<PayrunBatchProcessingJob> _logger;
		private readonly SingleEmployeeProcessingService _service;

		public SingleEmployeeProcessingJob(ILogger<PayrunBatchProcessingJob> logger, SingleEmployeeProcessingService service)
		{
			_logger = logger;
			_service = service;
		}


		[Queue("single-employee-job")]
		[AutomaticRetry(Attempts = 3, DelaysInSeconds = new int[] { 1, 2, 3 }, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
		public string Execute(params object[] args)
		{
			return AsyncHelper.RunSync(() => ExecuteAsync(args));
		}

		public async Task<string> ExecuteAsync(params object[] args)
		{
			var jobInternalCode = Guid.NewGuid().ToString();

			var jobInfoJobject = args[0] as JObject;
			var jobInfo = jobInfoJobject.ToObject<CalculationRequest>();

			var item = JsonSerializer.Serialize(jobInfo);
			_logger.LogWarning($"[SingleEmployeeProcessingService]:;[{jobInternalCode}] =>  with params : {item}  started!");

			try
			{
				await _service.ProcessAsync(jobInfo);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, $"[SingleEmployeeProcessingService]:;[{jobInternalCode}] =>  with params : {item}  resulted in exception!!");
				throw;
			}

			_logger.LogWarning($"[SingleEmployeeProcessingService]:;[{jobInternalCode}] =>  with params : {item}  finished!");

			return "Success";
		}
	}
}
