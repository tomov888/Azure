using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;

namespace BlobStorageShared.Repository
{
	public class BlobFileRepository
	{
		protected string _storageConnectionString;
		protected string _storageContainerName;
		protected BlobContainerClient _containerClient;
		protected readonly ILogger _logger;

		public BlobFileRepository(ILogger logger, string connectionString, string containerName)
		{
			_logger = logger;
			_storageContainerName = containerName;
			_storageConnectionString = connectionString;
			_containerClient = new BlobContainerClient(_storageConnectionString, _storageContainerName);
		}

		public async Task UploadFileAsync(string filePath, string blobPath = null)
		{
			blobPath = blobPath ?? $"{Path.GetFileName(filePath)}";
			var blobClient = _containerClient.GetBlobClient(blobPath);

			using (var fileStream = File.OpenRead(filePath))
			{
				await blobClient.UploadAsync(fileStream);
			}
		}

		public async Task UploadFileAsync(Stream fileStream, string blobPath)
		{
			var blobClient = _containerClient.GetBlobClient(blobPath);
			await blobClient.UploadAsync(fileStream);
		}

		public async Task DownloadFileAsync(string blobPath)
		{
			var blobClient = _containerClient.GetBlobClient(blobPath);
			using (var fileStream = new MemoryStream()) 
			{
				await blobClient.DownloadToAsync(fileStream);
				fileStream.Position = 0;
				Console.WriteLine($"Downloaded {fileStream.Length} bytes");
			}
		}
	}
}