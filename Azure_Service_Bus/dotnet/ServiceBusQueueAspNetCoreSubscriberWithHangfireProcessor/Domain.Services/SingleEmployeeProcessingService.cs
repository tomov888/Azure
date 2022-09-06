using Domain;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace Domain.Services
{
	public class SingleEmployeeProcessingService
	{
		private readonly ILogger<SingleEmployeeProcessingService> _logger;

		public SingleEmployeeProcessingService(ILogger<SingleEmployeeProcessingService> logger)
		{
			_logger = logger;
		}

		public async Task ProcessAsync(CalculationRequest dto)
		{
			_logger.LogWarning($"[SingleEmployeeProcessingService] => Started processing {JsonSerializer.Serialize(dto)}");

			//await Task.Delay(5_000);
			await CalcEngineProcessRunner.RunAsync("single-employee");
			//var location = @"C:\Users\nen040722\source\repos\paycor_poc_s\CalcEngineServerPoC_Hangfire\CalcEngineSimulator\bin\Debug\net6.0\CalcEngineSimulator.exe";
			//Console.WriteLine("[RUN SHELL PROCESS] => Start");
			//ProcessStartInfo startInfo = new()
			//{
			//	UseShellExecute = false,
			//	FileName = location,
			//	RedirectStandardOutput = true,
			//	CreateNoWindow = true,
			//	Arguments = "single-employee"
			//};

			//Process process = Process.Start(startInfo);
			//await process.WaitForExitAsync();

			//var exitCode = process.ExitCode;

			//Console.WriteLine($"[RUN SHELL PROCESS] => Process exit code: {exitCode}");
			//var success = exitCode == 0;
			//Console.WriteLine($"[RUN SHELL PROCESS] => Process ended with success: {success}");

			//Console.WriteLine("[RUN SHELL PROCESS] => End");

			_logger.LogWarning($"[SingleEmployeeProcessingService] => Finished processing {JsonSerializer.Serialize(dto)}");
		}
	}
}
