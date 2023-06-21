using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace PoliceDepartment.EvidenceManager.Infra.FileManager
{
    public interface IEvidenceFileServer
    {
        Task<string> GetEvidenceAsync(string name, string containerName);
        Task<BlobDownloadInfo> GetEvidenceBytesAsync(string name, string containerName);
        Task<bool> UploadEvidenceAsync(string fileExtension, IFormFile file, string containerName);
        Task<bool> DeleteEvidenceAsync(string name, string containerName);
    }
}