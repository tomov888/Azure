using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace EternalOrchestrationFunctionApp.CustomSerialization
{
	public class CustomErrorSerializerSettingsFactory : IErrorSerializerSettingsFactory
	{
		public JsonSerializerSettings CreateJsonSerializerSettings()
		{
			return new JsonSerializerSettings
			{
				ContractResolver = new SafeSerializationInfoContractResolver(),
				TypeNameHandling = TypeNameHandling.All,
			};
		}
	}
}
