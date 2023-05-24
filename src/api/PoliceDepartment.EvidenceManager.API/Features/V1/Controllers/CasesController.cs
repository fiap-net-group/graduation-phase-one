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
        private readonly IGetCasesByOfficerId<BaseResponseWithValue<IEnumerable<CaseViewModel>>> _getByOfficerId;

        public CasesController(IGetCasesByOfficerId<BaseResponseWithValue<IEnumerable<CaseViewModel>>> getByOfficerId)
        {
            _getByOfficerId = getByOfficerId;
        }

        /// <summary>
        /// Get all cases of a police officer
        /// </summary>
        /// <param name="officerId">The police officer id</param>
        /// <param name="cancellationToken"></param>]
        /// <response code="200">The list of cases</response>
        [HttpGet("officer/{officerId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(IEnumerable<CaseViewModel>))]
        public async Task<IActionResult> GetByOfficerId(Guid officerId, CancellationToken cancellationToken)
        {
            var cases = await _getByOfficerId.RunAsync(officerId, cancellationToken);

            return Ok(cases);
        }
    }
}
