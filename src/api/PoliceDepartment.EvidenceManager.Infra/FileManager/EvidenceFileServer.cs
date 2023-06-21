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

        public async Task<string> GetEvidenceAsync(string name, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(name);

            return await Task.Run(() => blobClient.Uri.AbsoluteUri);
        }

        public async Task<BlobDownloadInfo> GetEvidenceBytesAsync(string name, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(name);
            var blobDownloadInfo = await blobClient.DownloadAsync();

            return blobDownloadInfo.Value;
        }

        public async Task<bool> UploadEvidenceAsync(string fileExtension, IFormFile file, string containerName)
        {
            var fileName = $"{Guid.NewGuid()}_{fileExtension}";
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var res = await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });
            return res != null;
        }

        public async Task<bool> DeleteEvidenceAsync(string name, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(name);
            return await blobClient.DeleteIfExistsAsync();
        }

    }
}