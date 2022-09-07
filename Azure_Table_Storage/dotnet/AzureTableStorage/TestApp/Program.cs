using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;
using TableStorageShared;

namespace TestApp 
{
	public class Program 
	{
		private static string ConnectionString = "fake-cs";
		private static TableStorageRepository<PersonEntity> _personRepository;

		static void Main(string[] args)
		{
			Console.WriteLine("#### Table Storage ####");
			var logger = LoggerFactory.Create(x => x.SetMinimumLevel(LogLevel.Information)).CreateLogger(nameof(TableStorageRepository<PersonEntity>));
			var tableServiceClient = new TableServiceClient(ConnectionString);
			_personRepository = new TableStorageRepository<PersonEntity>(logger, tableServiceClient, "person");

			var id1 = Guid.NewGuid();
			var person1 = new PersonEntity 
			{
				Id = id1,
				Name = "Nenad",
				DateOfBirth = new DateTime(1992,12,7,0,0,0, DateTimeKind.Utc),
				RowKey = id1.ToString(),
				PartitionKey = "Nenad",
				Timestamp = DateTime.UtcNow
			};
			var id2 = Guid.NewGuid();
			var person2 = new PersonEntity
			{
				Id = id2,
				Name = "Milan",
				DateOfBirth = new DateTime(1982, 11, 4, 0, 0, 0, DateTimeKind.Utc),
				RowKey = id2.ToString(),
				PartitionKey = "Milan",
				Timestamp = DateTime.UtcNow
			};

			_personRepository.InsertAsync(person1).GetAwaiter().GetResult();
			_personRepository.InsertAsync(person2).GetAwaiter().GetResult();
			

			var persons = _personRepository.QueryAsync(x => x.PartitionKey == "Nenad" && x.Name == "Nenad").GetAwaiter().GetResult();
			Console.WriteLine(JsonSerializer.Serialize(persons));

			_personRepository.DeleteAsync("Nenad", id1.ToString()).GetAwaiter().GetResult();

		}
	}
}
