namespace FunctionApp.CalcRequestProcessingExample.Dto
{
	public record PayrunBatchProcessingCompletedNotificationDto
	{
		public int BatchId { get; set; }
		public string MessageLabel { get; set; }
	}
}