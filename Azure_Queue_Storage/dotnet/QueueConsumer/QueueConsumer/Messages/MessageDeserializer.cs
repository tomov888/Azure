using System.Reflection;
using System.Text.Json;

namespace QueueConsumer.Messages
{
	public class MessageDeserializer
	{
		private readonly Dictionary<string, Func<string, IMessage>> _deserializationStrategy;
		public MessageDeserializer()
		{
			_deserializationStrategy = Assembly
							.GetAssembly(typeof(Message))
							.DefinedTypes
							.Where(x => typeof(IMessage).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface && x.AsType() != typeof(Message))
							.Select(x => x.AsType())
							.ToList().ToDictionary
							(
								type => type.Name,
								(Func<Type, Func<string, IMessage>>)(type => json => (IMessage)JsonSerializer.Deserialize(json, type))
							);
		}

		public IMessage Deserialize(string json)
		{
			var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

			try
			{
				var messageTypeName = jsonElement.GetProperty(nameof(Message.MessageTypeName)).GetString();

				if (messageTypeName is null) throw new KeyNotFoundException($"Passed json is not valid {nameof(IMessage)} object since it does not have {nameof(Message.MessageTypeName)} property.");

				return Deserialize(json, messageTypeName);

			}
			catch (KeyNotFoundException)
			{
				Console.WriteLine($"Passed json is not valid {nameof(IMessage)} object since it does not have {nameof(Message.MessageTypeName)} property.");
				throw;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception {ex.ToString()} occured while deserializing message");
				Console.WriteLine($"{ex.Message}");
				throw;
			}
		}

		private IMessage Deserialize(string json, string messageTypeName)
		{
			var message = _deserializationStrategy[messageTypeName](json);
			//IMessage? message = messageTypeName switch
			//{
			//	nameof(ItemCreatedMessage) => JsonSerializer.Deserialize<ItemCreatedMessage>(json),
			//	nameof(ItemDeletedMessage) => JsonSerializer.Deserialize<ItemDeletedMessage>(json),
			//	nameof(ItemArchivedMessage) => JsonSerializer.Deserialize<ItemArchivedMessage>(json),
			//	nameof(ItemUpdatedMessage) => JsonSerializer.Deserialize<ItemUpdatedMessage>(json),
			//	_ => throw new InvalidCastException($"{messageTypeName} is not valid message type")
			//};

			return message;
		}
	}
}