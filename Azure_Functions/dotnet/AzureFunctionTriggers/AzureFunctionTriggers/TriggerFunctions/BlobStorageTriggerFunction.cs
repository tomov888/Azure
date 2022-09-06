using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureFunctionTriggers.TriggerFunctions
{
	public class BlobStorageTriggerFunction
	{
		[FunctionName("BlobStorageTriggerFunction-StreamInputExample")]
		public async Task StreamInputExample(
			[BlobTrigger("test-container/test-virtual-path/{name}", Connection = "DataStorageConnection")] Stream myBlob,
			string name,
			ILogger log)
		{
			log.LogWarning($"Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");


			// Read blob as json
			if (Path.GetExtension(name) == ".json")
			{
				using (var streamReader = new StreamReader(myBlob))
				{
					var json = await streamReader.ReadToEndAsync();
					log.LogWarning(json);
				}
			}

			await Task.CompletedTask;
		}

		[FunctionName("BlobStorageTriggerFunction-BlobClientInputExample")]
		public async Task JsonInputExample(
			[BlobTrigger("test-container/blob-client-items/{name}")] BlobClient blob,
			string name,
			ILogger log)
		{
			BlobProperties blobProperties = await blob.GetPropertiesAsync();

			log.LogWarning($"Blob Type: {blobProperties.BlobType} \n Name: {blob.Name}");

			using (var memoryStream = new MemoryStream())
			{
				var content = await blob.DownloadToAsync(memoryStream);
				memoryStream.Position = 0;

				using (var streamReader = new StreamReader(memoryStream))
				{
					var json = await streamReader.ReadToEndAsync();
					log.LogWarning(json);
				}
			}
		}

	}
}