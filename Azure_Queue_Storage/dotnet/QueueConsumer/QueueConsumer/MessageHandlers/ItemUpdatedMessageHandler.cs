using Microsoft.Extensions.Logging;
using QueueConsumer.Extensions;
using QueueConsumer.Messages;

namespace QueueConsumer.MessageHandlers
{
	public class ItemUpdatedMessageHandler : MessageHandler
	{
		public static Type MessageType { get; } = typeof(ItemUpdatedMessage);
		private readonly ILogger<ItemUpdatedMessageHandler> _logger;

		public ItemUpdatedMessageHandler(ILogger<ItemUpdatedMessageHandler> logger)
		{
			_logger = logger;
		}

		public override async Task HandleAsync(IMessage message)
		{
			var itemMessage = (ItemUpdatedMessage)message;

			_logger.LogWarning($"[{nameof(ItemUpdatedMessageHandler)}] => Handling message : {itemMessage.SerializeToJson()}");

			_logger.LogWarning($"[{nameof(ItemUpdatedMessageHandler)}] => Message Handled.");

			await Task.CompletedTask;
		}
	}
}
