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

        // Method to upload an image to Azure Blob Storage
        public async Task<string> UploadImageToBlobAsync(Stream imageStream, string fileName, string containerName)
        {
            // Ensure the container exists or create it
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            // Generate unique name for the blob (e.g., using a GUID)
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + "-" + fileName);

            // Upload the image to Azure Blob Storage
            await blobClient.UploadAsync(imageStream, true);

            // Return the URL of the uploaded image
            return blobClient.Uri.ToString();
        }
    }
}
