using Azure.Storage.Queues;

using Microsoft.Extensions.Azure;
using QueueConsumer.MessageHandlers;
using QueueConsumer.Messages;
using QueueConsumer.Services;
using QueueConsumer.Settings;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<MessageDeserializer>();

RegisterAzureStorageQueueDependencies(builder);

builder.Services.AddSingleton<MessageDispatcher>();

RegisterMessageHandlers(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapControllers();

app.Run();

static void RegisterAzureStorageQueueDependencies(WebApplicationBuilder builder)
{
	var azureStorageQueueSettings = new AzureStorageQueueSettings
	{
		AzureStorageAccount = builder.Configuration.GetValue<string>("AzureStorageQueueSettings:AzureStorageAccount") ?? throw new ArgumentNullException("AzureStorageQueueSettings:AzureStorageAccount"),
		QueueName = builder.Configuration.GetValue<string>("AzureStorageQueueSettings:QueueName") ?? throw new ArgumentNullException("AzureStorageQueueSettings:QueueName"),
		MaxNumberOfMessagesToDequeue = int.Parse(builder.Configuration.GetValue<string>("AzureStorageQueueSettings:MaxNumberOfMessagesToDequeue") ?? throw new ArgumentNullException("AzureStorageQueueSettings:MaxNumberOfMessagesToDequeue")),
		PollingFrequency = TimeSpan.Parse(builder.Configuration.GetValue<string>("AzureStorageQueueSettings:PollingFrequency") ?? throw new ArgumentNullException("AzureStorageQueueSettings:PollingFrequency"))
	};
	builder.Services.AddSingleton(azureStorageQueueSettings);


	builder.Services.AddAzureClients(azureClientsBuilder =>
	{
		azureClientsBuilder.AddClient<QueueClient, QueueClientOptions>((options, _, _) =>
		{
			options.Diagnostics.IsLoggingEnabled = false;
			options.MessageEncoding = QueueMessageEncoding.Base64;

			var queueClient = new QueueClient(azureStorageQueueSettings.AzureStorageAccount, azureStorageQueueSettings.QueueName, options);

			queueClient.CreateIfNotExists();

			return queueClient;
		});
	});

	builder.Services.AddHostedService<AzureStorageQueueConsumerService>();
}

static void RegisterMessageHandlers(WebApplicationBuilder builder)
{
	Assembly
			.GetAssembly(typeof(MessageHandler))
			.DefinedTypes
			.Where(x => typeof(MessageHandler).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)
			.Select(x => x.AsType())
			.ToList()
			.ForEach(x => builder.Services.AddScoped(x));

	//builder.Services.AddScoped<ItemCreatedMessageHandler>();
	//builder.Services.AddScoped<ItemDeletedMessageHandler>();
	//builder.Services.AddScoped<ItemUpdatedMessageHandler>();
	//builder.Services.AddScoped<ItemArchivedMessageHandler>();
}