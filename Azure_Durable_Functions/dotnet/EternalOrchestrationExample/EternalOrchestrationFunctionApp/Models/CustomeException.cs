using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EternalOrchestrationFunctionApp.Models
{
	public class CustomeException : Exception
	{
		public CustomeException(string message) : base(message)
		{
		}
	}
}
