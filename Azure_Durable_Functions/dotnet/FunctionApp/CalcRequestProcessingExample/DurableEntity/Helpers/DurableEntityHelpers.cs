using FunctionApp.PayrunBatchProcessingExample.DurableEntity;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.CalcRequestProcessingExample.DurableEntity.Helpers
{
	public static class DurableEntityHelpers
	{
		public static EntityId PayrunBatchDurableEntityKey(int clientId, int batchId)
		{
			return new EntityId(nameof(PayrunBatchDurableEntity), $"{clientId}-{batchId}");
		}
	}
}
