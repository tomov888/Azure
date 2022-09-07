using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobStorageShared.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BlobStorageShared.Repository
{
	public class BlobStorageJsonRepository<T> where T : class
	{
		protected string _storageConnectionString;
		protected string _storageContainerName;
		protected BlobContainerClient _containerClient;
		protected readonly ILogger _logger;

		public BlobStorageJsonRepository(ILogger logger, string connectionString, string containerName)
		{
			_logger = logger;
			_storageContainerName = containerName;
			_storageConnectionString = connectionString;
			_containerClient = new BlobContainerClient(_storageConnectionString, _storageContainerName);
		}

		public async Task<IEnumerable<BlobItem>> ListAllBlobsAsync(string prefix = null) 
		{
			var blobs = new	List<BlobItem>();
			
			var result = prefix is null 
				? _containerClient.GetBlobsAsync().AsPages() 
				: _containerClient.GetBlobsAsync(prefix: prefix).AsPages();

			await foreach (Azure.Page<BlobItem> blobPage in result)
			{
				foreach (BlobItem blobItem in blobPage.Values)
				{
					blobs.Add(blobItem);
				}
			}

			return blobs;
		}

		public async Task<IEnumerable<T>> GetAllAsync(string prefix = null)
		{
			var blobs = new List<T>();

			var result = _containerClient.GetBlobsAsync().AsPages();

			await foreach (Azure.Page<BlobItem> blobPage in result)
			{
				foreach (BlobItem blobItem in blobPage.Values)
				{
					var blobClient = _containerClient.GetBlobClient(blobItem.Name);
					var data = await blobClient.DownloadContentAsync();
					var jsonContent = Encoding.UTF8.GetString(data.Value.Content);
					var item = System.Text.Json.JsonSerializer.Deserialize<T>(jsonContent);
					blobs.Add(item);
				}
			}

			return blobs;
		}

		public async Task<IEnumerable<T>> SearchByPrefixAsync(string prefix)
		{
			var blobs = new List<T>();

			var result = _containerClient.GetBlobsAsync(prefix: prefix).AsPages();

			await foreach (Azure.Page<BlobItem> blobPage in result)
			{
				foreach (BlobItem blobItem in blobPage.Values)
				{
					var blobClient = _containerClient.GetBlobClient(blobItem.Name);
					var data = await blobClient.DownloadContentAsync();
					var jsonContent = Encoding.UTF8.GetString(data.Value.Content);
					var item = System.Text.Json.JsonSerializer.Deserialize<T>(jsonContent);
					blobs.Add(item);
				}
			}

			return blobs;
		}

		public async Task<IEnumerable<T>> SearchByTagsAsync(string tagsQuery)
		{
			var blobs = new List<T>();
			
			await foreach (TaggedBlobItem taggedBlobItem in _containerClient.FindBlobsByTagsAsync(tagsQuery))
			{
				var blobClient = _containerClient.GetBlobClient(taggedBlobItem.BlobName);
				var data = await blobClient.DownloadContentAsync();
				var jsonContent = Encoding.UTF8.GetString(data.Value.Content);
				var item = System.Text.Json.JsonSerializer.Deserialize<T>(jsonContent);
				blobs.Add(item);
			}

			return blobs;
		}

		public async Task<Option<T>> ReadAsync(string itemVirtualPath)
		{
			if (string.IsNullOrWhiteSpace(itemVirtualPath))
			{
				throw new ArgumentException(nameof(itemVirtualPath));
			}

			var blobClient = _containerClient.GetBlobClient(itemVirtualPath);
			var blobExists = await blobClient.ExistsAsync();

			if (!blobExists) 
			{
				_logger.LogWarning($"[{nameof(BlobStorageJsonRepository<T>)}] => For virtualPath: {itemVirtualPath} did not found blob.");
				return Option<T>.None;
			}
			

			var data = await blobClient.DownloadContentAsync();

			var jsonContent = Encoding.UTF8.GetString(data.Value.Content);

			_logger.LogWarning($"[{nameof(BlobStorageJsonRepository<T>)}] => For virtualPath: {itemVirtualPath} found json: {jsonContent}");

			var item = System.Text.Json.JsonSerializer.Deserialize<T>(jsonContent);

			return Option<T>.From(item);
		}

		public async Task WriteAsync(T item, string itemVirtualPath)
		{
			if (item is null)
			{
				throw new ArgumentException(nameof(item));
			}

			if (string.IsNullOrWhiteSpace(itemVirtualPath))
			{
				throw new ArgumentException(nameof(itemVirtualPath));
			}

			var blobClient = _containerClient.GetBlobClient(itemVirtualPath);

			var jsonItem = System.Text.Json.JsonSerializer.Serialize(item);
			var bytes = Encoding.UTF8.GetBytes(jsonItem);

			using var stream = new MemoryStream(bytes);
			await blobClient.UploadAsync(stream, overwrite: true);
		}

		public async Task WriteAsync(T item, string itemVirtualPath, Dictionary<string, string> tags)
		{
			if (item is null)
			{
				throw new ArgumentException(nameof(item));
			}

			if (string.IsNullOrWhiteSpace(itemVirtualPath))
			{
				throw new ArgumentException(nameof(itemVirtualPath));
			}

			var blobClient = _containerClient.GetBlobClient(itemVirtualPath);

			var jsonItem = System.Text.Json.JsonSerializer.Serialize(item);
			var bytes = Encoding.UTF8.GetBytes(jsonItem);

			using var stream = new MemoryStream(bytes);
			await blobClient.UploadAsync(stream, overwrite: true);
			await blobClient.SetTagsAsync(tags);
		}

		public async Task<bool> ExistsAsync(string itemVirtualPath)
		{
			if (string.IsNullOrWhiteSpace(itemVirtualPath))
			{
				throw new ArgumentException(nameof(itemVirtualPath));
			}

			var blobClient = _containerClient.GetBlobClient(itemVirtualPath);
			var result = await blobClient.ExistsAsync();

			return result.Value;
		}
	}
}