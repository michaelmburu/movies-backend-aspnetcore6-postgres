using System;
using Azure.Storage.Blobs;

namespace Movies_API.Helpers
{
    public class AzureStorageService : IFileStorageService
    {
        private string _connectionString;
        private BlobContainerClient _client;

        public AzureStorageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorageConnection");
        }

        //Delete File
        public async Task DeleteFile(string fileRoute, string containerName)
        {
            //Create container 
            var client = new BlobContainerClient(_connectionString, containerName);
            client.CreateIfNotExists();

            if (string.IsNullOrEmpty(fileRoute))
            {
                return;
            }

            //Get file
            var fileName = Path.GetFullPath(fileRoute);
            var blob = _client.GetBlobClient(fileName);

            //Delete file
            await blob.DeleteIfExistsAsync();
        }

        //Update File
        public async Task<string> EditFile(string containerName, IFormFile file, string fileRoute)
        {
            await DeleteFile(fileRoute, containerName);
            return await SaveFile(containerName, file);
        }

        //Save New File
        public async Task<string> SaveFile(string containerName, IFormFile file)
        {
            //Create container 
            var client = new BlobContainerClient(_connectionString, containerName);
            client.CreateIfNotExists();
            client.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            // Create filename
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";

            //Upload file
            var blob = _client.GetBlobClient(fileName);
            await blob.UploadAsync(file.OpenReadStream());

            // Return string
            return blob.Uri.ToString();
        }
    }
}

