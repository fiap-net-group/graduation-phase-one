using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.Infra.FileManager
{
    public class EvidenceFileServer : IEvidenceFileServer
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BaseResponseWithValue<string> _response;
        private readonly string _fileExtension = ".png";


        public EvidenceFileServer(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _response = new();
        }

        public async Task<BaseResponseWithValue<string>> GetEvidenceAsync(string evidenceImageId, string containerName)
        {
            var fileName = $"{evidenceImageId}{_fileExtension}";
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var exists = await blobClient.ExistsAsync();

            if (!exists)            
                return _response.AsError("The evidence doesn't exists");
            
            return _response.AsSuccess(await Task.Run(() => blobClient.Uri.AbsoluteUri));
        }

        public async Task<BaseResponseWithValue<string>> UploadEvidenceAsync(byte[] imageArray, string fileExtension, string containerName, CancellationToken cancellationToken)
        {
            if (fileExtension != _fileExtension)            
                return _response.AsError("The file extension is not valid");
            
            var stream = new MemoryStream(imageArray);
            var fileId = Guid.NewGuid().ToString();
            var fileName = $"{fileId}{fileExtension}";
            
            IFormFile file = new FormFile(stream, 0, imageArray.Length, fileId, fileName);

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            bool exists = await blobClient.ExistsAsync(cancellationToken);

            if (exists)
            {
                return _response.AsError("The evidence already exists");
            }

            var res = await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = $"image{fileExtension.Replace('.', '/')}" }, cancellationToken: cancellationToken);

            return _response.AsSuccess(Convert.ToString(res.GetRawResponse().Status == StatusCodes.Status201Created ? fileId : Guid.Empty));
        }

        public async Task<BaseResponseWithValue<string>> UploadEvidenceAsync(string fileExtension, IFormFile file, string containerName)
        {
            if (fileExtension != _fileExtension)
            {
                return _response.AsError("The file extension is not valid");
            }

            var fileId = Guid.NewGuid();
            var fileName = $"{fileId}{fileExtension}";
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            bool exists = await blobClient.ExistsAsync();

            if (exists)
            {
                return _response.AsError("The evidence already exists");
            }

            var res = await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });

            return _response.AsSuccess(Convert.ToString(res.GetRawResponse().Status == StatusCodes.Status201Created ? fileId : Guid.Empty));
        }


        public async Task<BaseResponseWithValue<string>> DeleteEvidenceAsync(string imageId, string containerName)
        {
            var fileName = $"{imageId}{_fileExtension}";
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            bool imageIsDeleted = await blobClient.DeleteIfExistsAsync();

            if (!imageIsDeleted)
            {
                return _response.AsError("The evidence doesn't exists");
            }

            return _response.AsSuccess("The evidence was deleted");
        }
    }
}