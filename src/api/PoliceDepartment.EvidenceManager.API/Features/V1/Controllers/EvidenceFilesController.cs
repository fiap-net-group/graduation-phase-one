using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.Infra.FileManager;
using PoliceDepartment.EvidenceManager.SharedKernel.Authorization;

namespace PoliceDepartment.EvidenceManager.API.Features.V1.Controllers
{
    /// <summary>
    /// Evidences business rules access point
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public sealed class EvidenceFilesController : ControllerBase
    {
        private readonly IEvidenceFileServer _evidenceFileServer;
        private readonly string _containerName = "evidences";

        public EvidenceFilesController(IEvidenceFileServer evidenceFileServer)
        {
            _evidenceFileServer = evidenceFileServer;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync(ICollection<IFormFile> files)
        {
            bool result = false;
            foreach (var formFile in files)
            {
                if (formFile.Length <= 0)
                    continue;

                var extension = Path.GetExtension(formFile.FileName);
                await using var stream = formFile.OpenReadStream();
                result = await _evidenceFileServer.UploadEvidenceAsync(extension, formFile, _containerName);
            }
            return Ok(result);
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadAsync([FromQuery] string fileName)
        {
            var result = await _evidenceFileServer.GetEvidenceAsync(fileName, _containerName);

            return Ok(result);
        }
    }

}