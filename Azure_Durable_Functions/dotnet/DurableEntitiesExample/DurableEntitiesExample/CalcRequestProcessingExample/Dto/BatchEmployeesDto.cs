using System.Collections.Generic;

namespace FunctionApp.CalcRequestProcessingExample.Dto
{
	public record BatchEmployeesDto
	{
		public BatchInfoDto BatchInfo { get; set; }
		public List<int> EmployeeIds { get; set; }
	}
}