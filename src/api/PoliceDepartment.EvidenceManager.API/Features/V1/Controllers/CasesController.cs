using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Case.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.API.Features.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class CasesController : ControllerBase
    {
        private readonly IUpdateCase<CaseViewModel, BaseResponse> _updateCase;
        private readonly IGetById<BaseResponseWithValue<CaseViewModel>> _getById;
        private readonly IGetCasesByOfficerId<BaseResponseWithValue<IEnumerable<CaseViewModel>>> _getByOfficerId;
        private readonly ICreateCase<CreateCaseViewModel, BaseResponse> _createCase;

        public CasesController(
            IGetCasesByOfficerId<BaseResponseWithValue<IEnumerable<CaseViewModel>>> getByOfficerId,
            IGetById<BaseResponseWithValue<CaseViewModel>> getById,
            IUpdateCase<CaseViewModel, BaseResponse> updateCase,
            ICreateCase<CreateCaseViewModel, BaseResponse> createCase)
        {
            _getByOfficerId = getByOfficerId;
            _getById = getById;
            _updateCase = updateCase;
            _createCase = createCase;
        }

        /// <summary>
        /// Get all cases of a police officer
        /// </summary>
        /// <param name="officerId">The police officer id</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">The list of cases</response>
        [HttpGet("officer/{officerId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(IEnumerable<CaseViewModel>))]
        public async Task<IActionResult> GetByOfficerId(Guid officerId, CancellationToken cancellationToken)
        {
            var cases = await _getByOfficerId.RunAsync(officerId, cancellationToken);

            return Ok(cases);
        }

        /// <summary>
        /// Update case
        /// </summary>
        /// <param name="id">The case id</param>
        /// <param name="caseViewModel">The new case properties</param>
        /// <param name="cancellationToken"></param>
        /// <response code="204">Success updating the case</response>
        /// <response code="400">Invalid case properties</response>
        /// <response code="404">The case does't exists</response>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, StatusCode = StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, StatusCode = StatusCodes.Status400BadRequest, Type = typeof(BaseResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, StatusCode = StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Patch(Guid id, CaseViewModel caseViewModel, CancellationToken cancellationToken)
        {
            var response = await _updateCase.RunAsync(id, caseViewModel, cancellationToken);

            if (response.Success)
                return NoContent();

            if (response.ResponseMessageEqual(ResponseMessage.CaseDontExists))
                return NotFound(response);

            return BadRequest(response);
        }
        
        /// <summary>
        /// Get case by id
        /// </summary>
        /// <param name="id">The case id</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">The respective case</response>
        /// <response code="404">Case not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(IEnumerable<CaseViewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, StatusCode = StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _getById.RunAsync(id, cancellationToken);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Create case
        /// </summary>
        /// <param name="createCaseViewModel"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="201">The created case</response>
        /// <response code="400">Invalid case properties</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, StatusCode = StatusCodes.Status201Created, Type = typeof(BaseResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, StatusCode = StatusCodes.Status400BadRequest, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CreateCase(CreateCaseViewModel createCaseViewModel, CancellationToken cancellationToken)
        {
            var response = await _createCase.RunAsync(createCaseViewModel, cancellationToken);

            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(null, null, response);
        }
    }
}
