namespace BlobStorageShared.Dto
{
	public record PersonDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime DateOfBirth { get; set; }
	}
}