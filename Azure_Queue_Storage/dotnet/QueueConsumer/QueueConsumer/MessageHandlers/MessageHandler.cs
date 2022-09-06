using QueueConsumer.Messages;

namespace QueueConsumer.MessageHandlers
{
	public abstract class MessageHandler
	{
		public abstract Task HandleAsync(IMessage message);

		public static Type MessageType { get; }
	}

}
