using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace EternalOrchestrationFunctionApp.CustomSerialization
{
	public class CustomMessageSerializerSettingsFactory : IMessageSerializerSettingsFactory
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
