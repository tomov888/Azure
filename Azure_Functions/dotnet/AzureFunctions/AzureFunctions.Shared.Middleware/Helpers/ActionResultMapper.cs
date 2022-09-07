using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace AzureFunctions.Shared.Middleware.Helpers
{
	public static class ActionResultMapper
	{
		public static IActionResult Map(Exception ex) =>
			ex switch
			{
				AuthenticationException => new UnauthorizedResult(),
				UnauthorizedAccessException => new ForbidResult(),
				_ => new BadRequestObjectResult(new { Message = ex.Message })
			};
	}
}
