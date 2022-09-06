namespace QueueConsumer.Messages
{
	public class ItemDeletedMessage : Message, IMessage
	{
		public string MessageTypeName => nameof(ItemDeletedMessage);
		public string Guid { get; set; }
		public string ClientId { get; set; }
		public DateTime EnqueuedAtUtc { get; set; }
		public string ItemName { get; set; }
		public int ItemNumber { get; set; }
		public DateTime ItemDeletedAtUtc { get; set; }
		public string ItemDeletedByUserId { get; set; }
	}
}