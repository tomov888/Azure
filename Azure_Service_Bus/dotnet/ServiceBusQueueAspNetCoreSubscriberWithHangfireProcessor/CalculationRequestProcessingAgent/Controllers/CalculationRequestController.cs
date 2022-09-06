using Domain;
using Hangfire.Server.Jobs.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CalculationRequestProcessingEngine.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CalculationRequestController : ControllerBase
	{

		private readonly ILogger<CalculationRequestController> _logger;
		private readonly JobManager _jobManager;

		public CalculationRequestController(ILogger<CalculationRequestController> logger, JobManager jobManager)
		{
			_logger = logger;
			_jobManager = jobManager;
		}

		[HttpPost]
		public async Task<IActionResult> PostRequest(CalculationRequest dto) 
		{

			if (dto.RequestType == RequestType.SingleEmployeeProcessing) 
			{
				_jobManager.EnqueueJob<SingleEmployeeProcessingJob>(x => x.Execute(dto));
				return Ok();
			}

			if (dto.RequestType == RequestType.PayrunBatchProcessing)
			{
				_jobManager.EnqueueJob<PayrunBatchProcessingJob>(x => x.Execute(dto));
				return Ok();
			}

			await Task.CompletedTask;
			
			return BadRequest();
		}

	}
}