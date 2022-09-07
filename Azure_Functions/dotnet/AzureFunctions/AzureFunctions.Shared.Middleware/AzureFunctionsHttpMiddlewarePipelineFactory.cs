using AzureFunctions.Shared.Middleware.Contracts;
using AzureFunctions.Shared.Middleware.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Shared.Middleware
{
	public class AzureFunctionsHttpMiddlewarePipelineFactory
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly IAuthorizeService _authorizer;

		public AzureFunctionsHttpMiddlewarePipelineFactory(
			ILoggerFactory loggerFactory,
			IAuthorizeService authorizer)
		{
			_loggerFactory = loggerFactory;
			_authorizer = authorizer;
		}

		public async Task<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>> AuthenticatedPipeline<TRequestPayload, TRequestResult>(
			HttpRequest request,
			Permission[] permissions)
		{
			ILogger<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>> logger = _loggerFactory.CreateLogger<AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>>();

			var middleware = await AzureFunctionsHttpMiddleware<TRequestPayload, TRequestResult>.For(request, permissions, _authorizer.AuthorizeAsync, logger);

			return middleware;
		}

		public async Task<AzureFunctionsHttpMiddleware<object, TRequestResult>> AuthenticatedPipeline<TRequestResult>(
			HttpRequest request,
			Permission[] permissions)
		{
			ILogger<AzureFunctionsHttpMiddleware<object, TRequestResult>> logger = _loggerFactory.CreateLogger<AzureFunctionsHttpMiddleware<object, TRequestResult>>();

			var middleware = await AzureFunctionsHttpMiddleware<object, TRequestResult>.For(request, permissions, _authorizer.AuthorizeAsync, logger);

			return middleware;
		}
	}
}