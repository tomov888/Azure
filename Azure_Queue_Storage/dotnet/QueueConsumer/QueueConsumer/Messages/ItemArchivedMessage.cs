namespace QueueConsumer.Messages
{
	public class ItemArchivedMessage : Message, IMessage
	{
		public string MessageTypeName => nameof(ItemUpdatedMessage);
		public string Guid { get; set; }
		public string ClientId { get; set; }
		public DateTime EnqueuedAtUtc { get; set; }
		public string ItemName { get; set; }
		public int ItemNumber { get; set; }
		public DateTime ItemArchivedAtUtc { get; set; }
		public string ItemArchivedByUserId { get; set; }
	}
}