using Microsoft.Extensions.Logging;
using QueueConsumer.Extensions;
using QueueConsumer.Messages;

namespace QueueConsumer.MessageHandlers
{
	public class ItemDeletedMessageHandler : MessageHandler
	{
		public static Type MessageType { get; } = typeof(ItemDeletedMessage);
		private readonly ILogger<ItemDeletedMessageHandler> _logger;

		public ItemDeletedMessageHandler(ILogger<ItemDeletedMessageHandler> logger)
		{
			_logger = logger;
		}

		public override async Task HandleAsync(IMessage message)
		{
			var itemMessage = (ItemDeletedMessage)message;

			_logger.LogWarning($"[{nameof(ItemDeletedMessageHandler)}] => Handling message : {itemMessage.SerializeToJson()}");

			_logger.LogWarning($"[{nameof(ItemDeletedMessageHandler)}] => Message Handled.");

			await Task.CompletedTask;
		}
	}
}
