using Microsoft.Extensions.Logging;
using QueueConsumer.Extensions;
using QueueConsumer.Messages;

namespace QueueConsumer.MessageHandlers
{
	public class ItemArchivedMessageHandler : MessageHandler
	{
		public static Type MessageType { get; } = typeof(ItemArchivedMessage);
		private readonly ILogger<ItemArchivedMessageHandler> _logger;

		public ItemArchivedMessageHandler(ILogger<ItemArchivedMessageHandler> logger)
		{
			_logger = logger;
		}

		public override async Task HandleAsync(IMessage message)
		{
			var itemMessage = (ItemArchivedMessage)message;

			_logger.LogWarning($"[{nameof(ItemArchivedMessageHandler)}] => Handling message : {itemMessage.SerializeToJson()}");

			_logger.LogWarning($"[{nameof(ItemArchivedMessageHandler)}] => Message Handled.");

			await Task.CompletedTask;
		}
	}
}
