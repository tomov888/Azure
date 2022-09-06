using Domain.Services;
using Hangfire;
using Hangfire.Server.Jobs.Utils;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var options = new SqlServerStorageOptions
{
	//SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
	QueuePollInterval = TimeSpan.FromSeconds(1),
};
builder.Services.AddHangfire(x => x.UseSqlServerStorage(@"Server=.\sqlexpress; Database=CalcEngineServerPoCHangfire; Integrated Security=SSPI;", options));
builder.Services.AddHangfireServer(options =>
{
	options.Queues = new[] { "single-employee-job", "payrun-batch-job" }; // define different type of job queues => useful when examinig processing state
																		  //options.WorkerCount = 20; // number of conccurent jobs of all types allowed to run
	options.WorkerCount = Environment.ProcessorCount * 10;
});

builder.Services.AddScoped<SingleEmployeeProcessingService>();
builder.Services.AddScoped<PayrunBatchProcessingService>();
builder.Services.AddTransient<SingleEmployeeProcessingJob>();
builder.Services.AddTransient<PayrunBatchProcessingJob>();
builder.Services.AddTransient<JobManager>();


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

app.UseHangfireDashboard();

app.UseAuthorization();

app.MapControllers();

app.Run();
