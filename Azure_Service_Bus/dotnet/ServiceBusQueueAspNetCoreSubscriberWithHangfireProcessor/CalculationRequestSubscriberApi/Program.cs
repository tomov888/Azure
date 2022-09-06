using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using RequestAgentApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAzureClients(clientBuilder =>
{
	var connectionString = "Endpoint=sb://nen040722calcenginepoc.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=saCEaBlYebYyTMRFHvy05fmNcLW96Jy2msYS/hGXqtM=";
	clientBuilder.AddServiceBusClient(connectionString);
});

builder.Services.AddHttpClient();
//builder.Services.AddHostedService<ConsumerBackgroundService>();
for (int i = 1; i <= 5; i++)
{
	builder.Services.AddSingleton<IHostedService, ConsumerBackgroundService>();
}

//builder.Services.AddSingleton<IHostedService, ConsumerBackgroundService>();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
