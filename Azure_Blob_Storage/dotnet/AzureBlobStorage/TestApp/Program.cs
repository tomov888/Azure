using BlobStorageShared.Dto;
using BlobStorageShared.Repository;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TestApp
{
	public class Program
	{
		private static string ConnectionString = "fake-connection-stirng";
		private static BlobStorageJsonRepository<PersonDto> _personRepo;
		public static void Main(string[] args)
		{
			var logger = LoggerFactory.Create(x => x.SetMinimumLevel(LogLevel.Information)).CreateLogger(nameof(BlobStorageJsonRepository<PersonDto>));
			_personRepo = new BlobStorageJsonRepository<PersonDto>(logger, ConnectionString, "test-container");

			Console.WriteLine("#### BlobStorage Test App ####");
			TestWriteAndRead();
			TestWriteAndSerachByTags();
		}

		private static void TestWriteAndSerachByTags()
		{
			var persons = new List<PersonDto>
			{
				new PersonDto
				{
					Id = 100,
					Name = "Nenad",
					DateOfBirth = DateTime.UtcNow,
				},
				new PersonDto
				{
					Id = 200,
					Name = "Marko",
					DateOfBirth = DateTime.UtcNow,
				},
				new PersonDto
				{
					Id = 300,
					Name = "Srdjan",
					DateOfBirth = DateTime.UtcNow,
				},
				new PersonDto
				{
					Id = 400,
					Name = "Petar",
					DateOfBirth = DateTime.UtcNow,
				},
				new PersonDto
				{
					Id = 500,
					Name = "Milan",
					DateOfBirth = DateTime.UtcNow,
				}
			};

			persons.ForEach(p => { WriteJson(p); });

			var blobsByTag = _personRepo.SearchByTagsAsync("Name = 'Milan'").GetAwaiter().GetResult();

			Console.WriteLine(JsonSerializer.Serialize(blobsByTag));
		}

		private static void TestWriteAndRead()
		{
			var person = new PersonDto
			{
				Id = 1,
				Name = "Nenad",
				DateOfBirth = DateTime.UtcNow,
			};

			WriteJson(person);
			var personFromStorage = ReadJson(person.Id);
		}

		private static void WriteJson(PersonDto person)
		{
			var tags = new Dictionary<string, string>
			{
				{ "Name", person.Name },
				{ "Id", person.Id.ToString() },
				{ "DateOfBirth", person.DateOfBirth.ToString("yyyy-MM-dd") },
			};
			_personRepo.WriteAsync(person, $"{person.Id}.json", tags).GetAwaiter().GetResult();
		}

		private static PersonDto ReadJson(int personId)
		{
			var personOption = _personRepo.ReadAsync($"{personId}.json").GetAwaiter().GetResult();
			if (personOption.HasValue) Console.WriteLine($"For path: {personId}.json found json: {JsonSerializer.Serialize(personOption.Value)}");
			if (personOption.Empty) Console.WriteLine($"For path: {personId}.json did not found any json");

			return personOption.Value;
		}
	}
}