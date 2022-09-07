using Azure;
using Azure.Data.Tables;

namespace TableStorageShared
{
	public record PersonEntity : ITableEntity
	{
		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }

		public Guid Id { get; set; }
		public string Name { get; set; }
		public DateTime DateOfBirth { get; set; }

	}
}