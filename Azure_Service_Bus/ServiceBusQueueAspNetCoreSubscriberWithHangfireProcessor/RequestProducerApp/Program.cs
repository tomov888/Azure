using Azure.Messaging.ServiceBus;
using Domain;
using System.Text.Json;

Console.WriteLine("Producer started.");

var connectionString = "Endpoint=sb://nen040722calcenginepoc.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=saCEaBlYebYyTMRFHvy05fmNcLW96Jy2msYS/hGXqtM=";
ServiceBusClient serviceBusClient = new ServiceBusClient(connectionString);
var producer = serviceBusClient.CreateSender("calc-request-queue");

while (true) 
{
	var input = Console.ReadLine();
	var numOfMessages = int.Parse(input);

	var messages = new List<CalculationRequest>();
	var random = new Random();
	Console.WriteLine($"Creating messages...[{numOfMessages}]");
	for (int i = 0; i < numOfMessages; i++) 
	{

		var message = new CalculationRequest 
		{
			RequestType = RequestType.SingleEmployeeProcessing,
			MessageNumber = i,
			Id = Guid.NewGuid(),
			Items = new List<CalculationQueueEntity> { new CalculationQueueEntity { ClientId = random.Next(1, 500), EmployeeId = random.Next(1000, 5000) } },
		};
		messages.Add(message);
	}
	Console.WriteLine($"{numOfMessages} messages created.");

	var options = new ParallelOptions
	{
		MaxDegreeOfParallelism = 20
	};

	Parallel.ForEach(messages, options, message =>
	{
		producer.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(message))).GetAwaiter().GetResult();
		Console.WriteLine($"Message {message.MessageNumber} is sent to AzureServiceBus.");
	});

}

//while (true)
//{
//	Console.WriteLine("Enter prefix for message type and a number of messages to send:  [se,200 / pb,200]");
//	try
//	{
//		var input = Console.ReadLine();
//		var parsedInput = input.Split(",");
//		var messageType = parsedInput[0];
//		var numOfMessages = int.Parse(parsedInput[1]);
//		for (int i = 0; i < numOfMessages; i++)
//		{
//			Console.WriteLine($"Sending message : #{i+1}");
//			var request = new CalculationRequest 
//			{
//				Id = Guid.NewGuid(),

//				RequestType = messageType == "se"
//					? RequestType.SingleEmployeeProcessing 
//					: RequestType.PayrunBatchProcessing,

//				Items = messageType == "se"
//					? new List<CalculationQueueEntity> { new CalculationQueueEntity { ClientId = new Random().Next(1, 500), EmployeeId = new Random().Next(1000,5000)} }
//					: new List<CalculationQueueEntity> 
//						{ 
//							new CalculationQueueEntity { ClientId = 499, EmployeeId = new Random().Next(1000, 5000) },
//							new CalculationQueueEntity { ClientId = 499, EmployeeId = new Random().Next(1000, 5000) },
//							new CalculationQueueEntity { ClientId = 499, EmployeeId = new Random().Next(1000, 5000) },
//							new CalculationQueueEntity { ClientId = 499, EmployeeId = new Random().Next(1000, 5000) },
//							new CalculationQueueEntity { ClientId = 499, EmployeeId = new Random().Next(1000, 5000) }
//						}
//			};
//			producer.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(request))).GetAwaiter().GetResult();
//			Console.WriteLine($"Message : #{i + 1} sent.");
//		}
//		Console.WriteLine($"{numOfMessages} sent");
//	}
//	catch (Exception ex)
//	{
//		Console.WriteLine($"{ex.Message}");
//	}

//}