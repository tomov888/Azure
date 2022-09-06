namespace Domain
{
	public record CalculationRequest
	{
		public long MessageNumber { get; set; }
		public Guid Id { get; set; }
		public RequestType RequestType { get; set; }
		public List<CalculationQueueEntity> Items { get; set; }

	}

	public record CalculationQueueEntity 
	{
		public long ClientId { get; set; }
		public long EmployeeId { get; set; }
	}

	public enum RequestType 
	{
		None = 0,
		SingleEmployeeProcessing = 1,
		PayrunBatchProcessing = 2,
		ClientTotals = 3
	}
}