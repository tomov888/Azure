using Microsoft.Extensions.Logging;
using QueueConsumer.Extensions;
using QueueConsumer.Messages;

namespace QueueConsumer.MessageHandlers
{
	public class ItemCreatedMessageHandler : MessageHandler
	{
		public static Type MessageType { get; } = typeof(ItemCreatedMessage);
		private readonly ILogger<ItemCreatedMessageHandler> _logger;

		public ItemCreatedMessageHandler(ILogger<ItemCreatedMessageHandler> logger)
		{
			_logger = logger;
		}

		public override async Task HandleAsync(IMessage message)
		{
			var itemMessage = (ItemCreatedMessage)message;

			_logger.LogWarning($"[{nameof(ItemCreatedMessageHandler)}] => Handling message : {itemMessage.SerializeToJson()}");

			_logger.LogWarning($"[{nameof(ItemCreatedMessageHandler)}] => Message Handled.");
			await Task.CompletedTask;
		}
	}
}
