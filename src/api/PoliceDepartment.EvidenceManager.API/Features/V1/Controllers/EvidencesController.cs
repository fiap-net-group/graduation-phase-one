using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.Application.Authorization;
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
        private readonly IDeleteEvidence<BaseResponse> _deleteEvidence;
        private readonly IGetEvidencesByCaseId<BaseResponseWithValue<IEnumerable<EvidenceViewModel>>> _getByCaseId;

        public EvidencesController(
            IGetEvidenceById<BaseResponseWithValue<EvidenceViewModel>> getById, 
            IDeleteEvidence<BaseResponse> deleteEvidence,
            IGetEvidencesByCaseId<BaseResponseWithValue<IEnumerable<EvidenceViewModel>>> getByCaseId)
        {
            _getById = getById;
            _deleteEvidence = deleteEvidence;
            _getByCaseId = getByCaseId;
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
        [ProducesResponseType(StatusCodes.Status404NotFound, StatusCode = StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
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

        /// <summary>
        /// Get evidences by case id
        /// </summary>
        /// <param name="caseId">The case id</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">The evidences</response>
        /// <response code="401">Invalid access code or API-TOKEN</response>
        [HttpGet("case/{caseId:guid}")]
        [Authorize(AuthorizationPolicies.IsPoliceOfficer)]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(IEnumerable<EvidenceViewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, StatusCode = StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, StatusCode = StatusCodes.Status401Unauthorized, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetByCaseId(Guid caseId, CancellationToken cancellationToken)
        {
            var response = await _getByCaseId.RunAsync(caseId, cancellationToken);

            if(response.Success)
                return Ok(response);

            if(response.ResponseMessageEqual(ResponseMessage.EvidenceDontExists))
                return NotFound(response);

            return Forbid();
        }

        /// <summary>
        /// Delete evidence
        /// </summary>
        /// <param name="id">The evidence id</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">The evidence was deleted</response>
        /// <response code="401">Invalid access code or API-TOKEN</response>
        [HttpDelete("{id:guid}")]
        [Authorize(AuthorizationPolicies.IsPoliceOfficer)]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(BaseResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, StatusCode = StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, StatusCode = StatusCodes.Status401Unauthorized, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var response = await _deleteEvidence.RunAsync(id,User.GetUserId(), cancellationToken);

            if(response.Success)
                return Ok(response);

            if(response.ResponseMessageEqual(ResponseMessage.EvidenceDontExists))
                return NotFound(response);

            return Forbid();
        }
    }
}
