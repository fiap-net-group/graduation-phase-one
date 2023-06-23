using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace PoliceDepartment.EvidenceManager.Infra.FileManager
{
    public class EvidenceFileServer : IEvidenceFileServer
    {
        private readonly BlobServiceClient _blobServiceClient;

        public EvidenceFileServer(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> GetEvidenceAsync(string evidenceImageId, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(evidenceImageId);

            return await Task.Run(() => blobClient.Uri.AbsoluteUri);
        }

        public async Task<Guid> UploadEvidenceAsync(string fileExtension, IFormFile file, string containerName)
        {
            var fileId = Guid.NewGuid();
            var fileName = $"{fileId}_{fileExtension}";
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            bool exists = await blobClient.ExistsAsync();

            if (exists)
            {
                return Guid.Empty;
            }

            var res = await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });
            return res.GetRawResponse().Status == StatusCodes.Status201Created ? fileId : Guid.Empty;
        }

        public async Task<bool> DeleteEvidenceAsync(string name, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(name);
            return await blobClient.DeleteIfExistsAsync();
        }
    }
}