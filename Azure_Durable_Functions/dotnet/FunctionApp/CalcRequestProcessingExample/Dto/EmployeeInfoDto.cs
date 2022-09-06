namespace FunctionApp.CalcRequestProcessingExample.Dto
{
	public record EmployeeInfoDto
	{
		public int EmployeeId { get; set; }
		public int ClientId { get; set; }
		public int BatchId { get; set; }
		public string MessageLabel { get; set; }
	}
}