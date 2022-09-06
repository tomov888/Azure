using Azure.Messaging.ServiceBus;
using Domain;
using System.Text;
using System.Text.Json;

namespace RequestAgentApi.Services
{
	public class ConsumerBackgroundService : BackgroundService, IHostedService
	{
		private readonly Guid _consumerId;
		private ServiceBusClient _serviceBusClient;
		private ServiceBusProcessor _messageProcessor;
		private readonly IHttpClientFactory _httpClientFactory;
		private HttpClient _httpClient;

		public ConsumerBackgroundService(ServiceBusClient serviceBusClient, IHttpClientFactory httpClientFactory)
		{
			_serviceBusClient = serviceBusClient;
			_messageProcessor = _serviceBusClient.CreateProcessor("calc-request-queue", new ServiceBusProcessorOptions { AutoCompleteMessages = true });
			_messageProcessor.ProcessMessageAsync += HandleMessage;
			_messageProcessor.ProcessErrorAsync += HandleError;
			_httpClientFactory = httpClientFactory;
			_httpClient = _httpClientFactory.CreateClient();
			_httpClient.BaseAddress = new Uri("http://localhost:5015");
			_consumerId = Guid.NewGuid();
			Console.WriteLine($"ConsumerBackgroundService: [{_consumerId}] created.");

		}

		public override async Task StartAsync(CancellationToken stoppingToken)
		{
			Console.WriteLine("Starting listener...");

			await _messageProcessor.StartProcessingAsync().ConfigureAwait(false);

			Console.WriteLine("Listener started.");
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Console.WriteLine("Starting listener...");

			await _messageProcessor.StartProcessingAsync().ConfigureAwait(false);

			Console.WriteLine("Listener started.");
		}

		public override async Task StopAsync(CancellationToken stoppingToken)
		{
			Console.WriteLine($"Closing processor...");
			await _messageProcessor.CloseAsync().ConfigureAwait(false);
		}

		private async Task HandleMessage(ProcessMessageEventArgs args)
		{

			ServiceBusReceivedMessage message = args.Message;
			var messageBody = Encoding.UTF8.GetString(message.Body);
			var payload = JsonSerializer.Deserialize<CalculationRequest>(messageBody);

			Console.WriteLine($"[{_consumerId}] => Message received...MessageId: {message.MessageId}");

			try
			{
				var result = await _httpClient.PostAsJsonAsync("CalculationRequest", payload);
				Console.WriteLine($"Contacted HangfireServer: {result.IsSuccessStatusCode}");
			}
			catch (Exception ex)
			{

				Console.WriteLine($"Error on contacting hangfire server: {ex.Message}");
			}


			await Task.CompletedTask;
		}

		private async Task HandleError(ProcessErrorEventArgs args)
		{
			Console.WriteLine($"Error happened...");
			Console.WriteLine($"Error => {args.Exception.Message}");
			await Task.CompletedTask;
		}

	}
}
