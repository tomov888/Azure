using Domain;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Domain.Services
{
	public class PayrunBatchProcessingService
	{
		private readonly ILogger<PayrunBatchProcessingService> _logger;

		public PayrunBatchProcessingService(ILogger<PayrunBatchProcessingService> logger)
		{
			_logger = logger;
		}

		public async Task ProcessAsync(CalculationRequest dto)
		{
			_logger.LogWarning($"[PayrunBatchProcessingService] => Started processing {JsonSerializer.Serialize(dto)}");

			//await Task.Delay(30_000);
			await CalcEngineProcessRunner.RunAsync("payrun-batch");

			_logger.LogWarning($"[PayrunBatchProcessingService] => Finished processing {JsonSerializer.Serialize(dto)}");
		}
	}
}
