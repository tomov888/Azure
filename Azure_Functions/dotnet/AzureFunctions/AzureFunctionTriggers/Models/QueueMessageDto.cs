using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionTriggers.Models
{
	public record QueueMessageDto
	{
		public int Id { get; init; }
		public string Name { get; set; }
	}
}
