using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.Infra.FileManager;
using PoliceDepartment.EvidenceManager.SharedKernel.Authorization;

namespace PoliceDepartment.EvidenceManager.API.Features.V1.Controllers
{
    /// <summary>
    /// Evidence files rules access point
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

        /// <summary>
        /// Upload evidence file
        /// </summary>
        /// <param name="formFile">Evidence file</param>
        /// <response code="200">Successful upload</response>
        /// <response code="400">Unsuccessful upload</response>
        [HttpPost("upload")]
        [Authorize(AuthorizationPolicies.IsPoliceOfficer)]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, StatusCode = StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict, StatusCode = StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UploadAsync(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var extension = Path.GetExtension(formFile.FileName);
            await using var stream = formFile.OpenReadStream();
            var result = await _evidenceFileServer.UploadEvidenceAsync(extension, formFile, _containerName);

            if (result == Guid.Empty)
                return Conflict("The evidence already exists");

            return Ok(result);
        }

        /// <summary>
        /// Get URI to download evidence file
        /// </summary>
        /// <param name="evidenceImageId">Evidence image GUID</param>
        /// <response code="200">Successful download</response>
        /// <response code="400">Unsuccessful download</response>
        [HttpGet("download")]
        [Authorize(AuthorizationPolicies.IsPoliceOfficer)]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> DownloadAsync([FromQuery] string evidenceImageId)
        {
            var result = await _evidenceFileServer.GetEvidenceAsync(evidenceImageId, _containerName);

            return Ok(result);
        }
    }
}