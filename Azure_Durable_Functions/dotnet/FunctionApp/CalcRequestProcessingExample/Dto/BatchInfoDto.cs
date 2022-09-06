namespace FunctionApp.CalcRequestProcessingExample.Dto
{
	public record BatchInfoDto
	{
		public int ClientId { get; init; }
		public string MessageLabel { get; init; }
		public int BatchId { get; set; }
	}
}