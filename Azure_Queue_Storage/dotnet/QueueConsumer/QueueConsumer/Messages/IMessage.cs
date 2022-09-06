namespace QueueConsumer.Messages
{
	public interface IMessage
	{
		public string MessageTypeName { get; init; }
	}
}