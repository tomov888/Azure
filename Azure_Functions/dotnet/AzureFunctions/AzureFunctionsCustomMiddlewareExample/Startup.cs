using AzureFunctions.Shared.Middleware;
using AzureFunctions.Shared.Middleware.Contracts;
using AzureFunctionsCustomMiddlewareExample;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AzureFunctionsCustomMiddlewareExample
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			builder.Services.AddScoped<IAuthorizeService, FakeAuthorizeService>();
			builder.Services.AddScoped<AzureFunctionsHttpMiddlewarePipelineFactory>();
		}
	}
}