using AzureFunctions.Shared.Middleware;
using AzureFunctions.Shared.Middleware.Helpers;
using AzureFunctions.Shared.Middleware.Models;
using AzureFunctionsCustomMiddlewareExample.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureFunctionsCustomMiddlewareExample
{
	public class ProtectedEndpointFunction
	{
		private readonly ILogger<ProtectedEndpointFunction> _logger;
		private readonly AzureFunctionsHttpMiddlewarePipelineFactory _middlewarePipeline;

		public ProtectedEndpointFunction(ILogger<ProtectedEndpointFunction> logger, AzureFunctionsHttpMiddlewarePipelineFactory middlewarePipeline)
		{
			_logger = logger;
			_middlewarePipeline = middlewarePipeline;
		}


		// GET http://localhost:7097/api/get/{clientId:int}
		[FunctionName(nameof(ProtectedGetEndpointExample))]
		public async Task<IActionResult> ProtectedGetEndpointExample([HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/{clientId:int}")] HttpRequest req, int clientId)
		{
			var permissions = new List<Permission> { Permission.FakePermission1 }.ToArray();

			var middleware = await _middlewarePipeline.AuthenticatedPipeline<List<ResponseDto>>(req, permissions);

			return await middleware
				.WithRequestValidation((request, session) => ValidateClientIdAsync(clientId))
				.WithAction((request, _, session) => GetBusinessLogicAsync(clientId))
				.ToIActionResultAsync();
		}


		// POST http://localhost:7097/api/post | {}
		[FunctionName(nameof(ProtectedPostEndpointExample))]
		public async Task<IActionResult> ProtectedPostEndpointExample([HttpTrigger(AuthorizationLevel.Function, "post", Route = "post")] HttpRequest req)
		{
			var permissions = new List<Permission> { Permission.FakePermission1 }.ToArray();

			var middleware = await _middlewarePipeline.AuthenticatedPipeline<RequestDto, ResponseDto>(req, permissions);

			var operationResult = await middleware
				.WithRequestPayloadValidation((payload, session) => ValidateRequestPayload(payload))
				.WithAction((request, payload, session) => PostBusinessLogicAsync(payload))
				.OperationResultAsync();

			return operationResult.IsSuccess
				? new OkObjectResult(operationResult.Payload)
				: ActionResultMapper.Map(operationResult.FailureReason);
		}

		private async Task ValidateClientIdAsync(int clientId)
		{
			await Task.CompletedTask;
		}

		private async Task ValidateRequestPayload(RequestDto payload)
		{
			await Task.CompletedTask;
		}

		private async Task<List<ResponseDto>> GetBusinessLogicAsync(int clientId)
		{
			await Task.CompletedTask;
			return new List<ResponseDto>();
		}

		private async Task<ResponseDto> PostBusinessLogicAsync(RequestDto dto)
		{
			await Task.CompletedTask;
			return new ResponseDto { };
		}
	}
}