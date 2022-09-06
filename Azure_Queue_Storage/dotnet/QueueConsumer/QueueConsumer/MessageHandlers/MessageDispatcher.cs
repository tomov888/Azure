using QueueConsumer.Messages;
using System.Reflection;

namespace QueueConsumer.MessageHandlers
{
	public class MessageDispatcher
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly Dictionary<string, Func<IServiceProvider, MessageHandler>> _handlers;

		public MessageDispatcher(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
			_handlers = Assembly
				.GetAssembly(typeof(MessageHandler))
				.DefinedTypes
				.Where(x => typeof(MessageHandler).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)
				.ToDictionary<TypeInfo, string, Func<IServiceProvider, MessageHandler>>
				(
					typeInfo => ((Type)typeInfo.GetProperty(nameof(MessageHandler.MessageType)).GetValue(null)).Name,
					typeInfo => provider => (MessageHandler)provider.GetRequiredService(typeInfo.AsType())
				);
		}

		public async Task DispatchAsync<TMessage>(TMessage message) where TMessage : IMessage
		{
			using var scope = _scopeFactory.CreateScope();
			var handler = _handlers[message.MessageTypeName](scope.ServiceProvider);
			await handler.HandleAsync(message);
		}

		public bool CanHandleMessage(IMessage message) => _handlers.ContainsKey(message.MessageTypeName);
	}
}