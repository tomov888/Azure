using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionTriggers.Models
{
	public class ServiceBusTopicMessageDto
	{
		public string Id { get; set; }
		public string Name  { get; set; }
	}
}
