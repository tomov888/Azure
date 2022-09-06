Console.WriteLine("[Calc Engine Simulator] Start...");

if (args[0] == "single-employee") 
{
	Console.WriteLine($"[Calc Engine Simulator] => Single Employee Processing started...");
	Thread.Sleep(3_000);
	Console.WriteLine($"[Calc Engine Simulator] => Single Employee Processing done...");
	
	Console.WriteLine($"[Calc Engine Simulator] End...");
	return;
}
if (args[0] == "payrun-batch") 
{
	Console.WriteLine($"[Calc Engine Simulator] => Payrun Batch Processing started...");
	Thread.Sleep(30_000);
	Console.WriteLine($"[Calc Engine Simulator] => Payrun Batch Processing done...");
	
	Console.WriteLine($"[Calc Engine Simulator] End...");
	return;
}

throw new Exception("ARGUMENT[0] IS INVALID !!!");
