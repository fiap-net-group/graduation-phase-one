using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.Infra.FileManager;
using PoliceDepartment.EvidenceManager.SharedKernel.Authorization;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

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
        [HttpPost("upload/file")]
        [Authorize(AuthorizationPolicies.IsPoliceOfficer)]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, StatusCode = StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict, StatusCode = StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UploadAsync(IFormFile formFile)
        {
            if (formFile is null)
                return BadRequest("The file is null");

            var extension = Path.GetExtension(formFile.FileName);
            await using var stream = formFile.OpenReadStream();
            var result = await _evidenceFileServer.UploadEvidenceAsync(extension, formFile, _containerName);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Upload evidence file with the image as bytes
        /// </summary>
        /// <param name="file">Evidence file</param>
        /// <param name="cancellationToken">Evidence file</param>
        /// <response code="200">Successful upload</response>
        /// <response code="400">Unsuccessful upload</response>
        [HttpPost("upload")]
        [Authorize(AuthorizationPolicies.IsPoliceOfficer)]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, StatusCode = StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict, StatusCode = StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UploadAsync(EvidenceFileViewModel file, CancellationToken cancellationToken)
        {
            var result = await _evidenceFileServer.UploadEvidenceAsync(file.ImageByte, file.FileExtension, _containerName, cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get URI to download evidence file
        /// </summary>
        /// <param name="evidenceImageId">Evidence image GUID</param>
        /// <response code="200">Successful download</response>
        /// <response code="400">Unsuccessful download</response>
        [HttpGet("download/{evidenceImageId:guid}")]
        [Authorize(AuthorizationPolicies.IsPoliceOfficer)]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> DownloadAsync(Guid evidenceImageId)
        {
            var result = await _evidenceFileServer.GetEvidenceAsync(Convert.ToString(evidenceImageId), _containerName);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("delete/{evidenceImageId:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid evidenceImageId)
        {
            var result = await _evidenceFileServer.DeleteEvidenceAsync(Convert.ToString(evidenceImageId), _containerName);

            return Ok(result);
        }
    }
}