using System.Diagnostics;

namespace Domain.Services
{
	public static class CalcEngineProcessRunner
	{
		public static async Task RunAsync(string arguments)
		{
			try
			{
				var location = @"C:\Users\nen040722\source\repos\paycor_poc_s\CalcEngineServerPoC_Hangfire\CalcEngineSimulator\bin\Debug\net6.0\CalcEngineSimulator.exe";
				Console.WriteLine("[RUN SHELL PROCESS] => Start");
				ProcessStartInfo startInfo = new()
				{
					UseShellExecute = false,
					FileName = location,
					RedirectStandardOutput = true,
					CreateNoWindow = true,
					Arguments = arguments
				};

				Process process = Process.Start(startInfo);
				await process.WaitForExitAsync();

				var exitCode = process.ExitCode;

				Console.WriteLine($"[RUN SHELL PROCESS] => Process exit code: {exitCode}");
				var success = exitCode == 0;
				Console.WriteLine($"[RUN SHELL PROCESS] => Process ended with success: {success}");

				Console.WriteLine("[RUN SHELL PROCESS] => End");

			}
			catch (Exception e)
			{
				Console.WriteLine("ERROR");
				Console.WriteLine(e.Message);

				Console.WriteLine("#### RUN SHELL PROCESS ####");
			}
		}
	}
}
