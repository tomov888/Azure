namespace QueueConsumer.Messages
{
	public class ItemCreatedMessage : Message, IMessage
	{
		public string MessageTypeName => nameof(ItemCreatedMessage);
		public string Guid { get; set; }
		public string ClientId { get; set; }
		public DateTime EnqueuedAtUtc { get; set; }
		public string ItemName { get; set; }
		public int ItemNumber { get; set; }
		public DateTime ItemCreatedAtUtc { get; set; }
		public string ItemCreatedByUserId { get; set; }
	}
}
