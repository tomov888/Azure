using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace EternalOrchestrationFunctionApp.CustomSerialization
{
	public class SafeSerializationInfoContractResolver : DefaultContractResolver
	{
		protected override JsonContract CreateContract(Type objectType)
		{
			var contract = base.CreateContract(objectType);

			if (contract is JsonISerializableContract)
			{
				var serializable = contract as JsonISerializableContract;
				if (serializable.ISerializableCreator == null && typeof(Exception).IsAssignableFrom(objectType))
				{
					serializable.ISerializableCreator = p =>
					{
						var info = (SerializationInfo)p[0];
						var context = (StreamingContext)p[1];

						var exception = (Exception)Activator.CreateInstance(typeof(Exception), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
						new object[] { info, context }, CultureInfo.InvariantCulture);

						var realException = Activator.CreateInstance(objectType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, CultureInfo.InvariantCulture);

						var fields = typeof(Exception).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

						foreach (var field in fields)
						{
							field.SetValue(realException, field.GetValue(exception));
						}

						return realException;
					};
				}
			}

			return contract;
		}
	}
}
