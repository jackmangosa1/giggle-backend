using Azure.Storage.Blobs;

namespace ServiceManagementAPI.Utils
{
    public class BlobStorageUtil
    {

        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageUtil(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> UploadImageToBlobAsync(Stream imageStream, string? fileName, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + "-" + fileName);
            await blobClient.UploadAsync(imageStream, true);

            return blobClient.Uri.ToString();
        }
    }
}
