using AzureFunctions.Shared.Middleware.Models;
using Microsoft.AspNetCore.Http;

namespace AzureFunctions.Shared.Middleware.Contracts
{
	public class FakeAuthorizeService : IAuthorizeService
	{
		public Task<OperationResult<UserSession>> AuthorizeAsync(HttpRequest httpRequest, Permission[] permissions)
		{
			// TODO => Parse Http Request and do real authorization here !!!
			// you can do jwt auth, header auth ...

			return Task.FromResult(OperationResult<UserSession>.Success(new UserSession 
			{
				UserId = 100,
				Email = "dummy@test.com",
				Username = "dummyUser",
				Permissions = new List<Permission> { Permission.FakePermission1, Permission.FakePermission2 }
			}));
		}
	}
}