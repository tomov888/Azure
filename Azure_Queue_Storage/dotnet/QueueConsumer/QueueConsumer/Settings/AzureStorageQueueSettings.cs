namespace QueueConsumer.Settings
{
	public class AzureStorageQueueSettings
	{
		public int MaxNumberOfMessagesToDequeue { get; set; }
		public TimeSpan PollingFrequency { get; set; }
		public string AzureStorageAccount { get; set; }
		public string QueueName { get; set; }
	}
}
