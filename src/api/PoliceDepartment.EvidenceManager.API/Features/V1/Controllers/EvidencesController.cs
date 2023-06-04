using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.API.Features.V1.Controllers
{
    /// <summary>
    /// Evidences business rules access point
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public sealed class EvidencesController : ControllerBase
    {
        private readonly IGetEvidenceById<BaseResponseWithValue<EvidenceViewModel>> _getById;

        public EvidencesController(IGetEvidenceById<BaseResponseWithValue<EvidenceViewModel>> getById)
        {
            _getById = getById;
        }

        /// <summary>
        /// Ping method
        /// </summary>
        /// <returns>
        /// pong
        /// </returns>
        [HttpGet("ping")]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> Ping()
        {
            return await Task.Run(() => Ok("pong"));
        }

        /// <summary>
        /// Get evidence by id
        /// </summary>
        /// <param name="id">The evidence id</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">The evidence</response>
        /// <response code="401">Invalid access code or API-TOKEN</response>
        [HttpGet("{id:guid}")]
        [Authorize(AuthorizationPolicies.IsPoliceOfficer)]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(EvidenceViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, StatusCode = StatusCodes.Status401Unauthorized, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _getById.RunAsync(id, cancellationToken);

            if(response.Success)
                return Ok(response);

            if(response.ResponseMessageEqual(ResponseMessage.EvidenceDontExists))
                return NotFound(response);

            return Forbid();
        }
    }
}
