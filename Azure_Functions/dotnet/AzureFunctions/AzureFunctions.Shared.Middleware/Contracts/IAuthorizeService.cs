using AzureFunctions.Shared.Middleware.Models;
using Microsoft.AspNetCore.Http;

namespace AzureFunctions.Shared.Middleware.Contracts
{
	public interface IAuthorizeService
	{
		Task<OperationResult<UserSession>> AuthorizeAsync(HttpRequest httpRequest, Permission[] permissions);
	}
}