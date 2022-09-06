using Microsoft.AspNetCore.Mvc;

namespace RequestAgentApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CalculationRequestController : ControllerBase
	{

		private readonly ILogger<CalculationRequestController> _logger;

		public CalculationRequestController(ILogger<CalculationRequestController> logger)
		{
			_logger = logger;
		}

		[HttpPost]
		public IActionResult PostCalculationRequest()
		{
			return Ok();
		}
	}
}