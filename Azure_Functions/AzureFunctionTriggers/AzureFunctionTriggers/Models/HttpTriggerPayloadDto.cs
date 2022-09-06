namespace AzureFunctionTriggers.Models
{
	public record HttpTriggerPayloadDto
	{
		public string Name { get; init; }
	}
}