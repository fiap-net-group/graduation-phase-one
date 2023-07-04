using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.Infra.FileManager
{
    public interface IEvidenceFileServer
    {
        Task<BaseResponseWithValue<string>> GetEvidenceAsync(string evidenceImageId, string containerName);
        Task<BaseResponseWithValue<string>> UploadEvidenceAsync(byte[] imageArray, string fileExtension, string containerName, CancellationToken cancellationToken);
        Task<BaseResponseWithValue<string>> UploadEvidenceAsync(string fileExtension, IFormFile file, string containerName);
        Task<BaseResponseWithValue<string>> DeleteEvidenceAsync(string evidenceImageId, string containerName);
    }
}