using Newtonsoft.Json;
using System.Collections.Generic;

namespace FunctionApp.CalcRequestProcessingExample.Dto
{
	[JsonObject(MemberSerialization.OptIn)]
	public class CalcRequestDurableEntityInitializeDto
	{
		[JsonProperty("messageLabel")]
		public string MessageLabel { get; set; }

		[JsonProperty("clientId")]
		public int ClientId { get; set; }

		[JsonProperty("payrunBatchIds")]
		public List<int> PayrunBatchIds { get; set; }
	}
}
