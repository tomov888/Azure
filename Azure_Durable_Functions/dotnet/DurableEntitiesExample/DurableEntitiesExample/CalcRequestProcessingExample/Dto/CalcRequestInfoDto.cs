namespace FunctionApp.CalcRequestProcessingExample.Dto
{
	public record CalcRequestInfoDto
	{
		public int ClientId { get; init; }
		public string MessageLabel { get; init; }
	}
}