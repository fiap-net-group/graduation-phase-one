using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace PoliceDepartment.EvidenceManager.Infra.FileManager
{
    public interface IEvidenceFileServer
    {
        Task<string> GetEvidenceAsync(string evidenceImageId, string containerName);
        Task<Guid> UploadEvidenceAsync(string fileExtension, IFormFile file, string containerName);
        Task<bool> DeleteEvidenceAsync(string evidenceImageId, string containerName);
    }
}