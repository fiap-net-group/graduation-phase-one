using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.SharedKernel.Officer.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.API.Features.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public sealed class OfficerController : ControllerBase
    {
        private readonly ICreateOfficer<CreateOfficerViewModel, BaseResponse> _createOfficer;

        public OfficerController(ICreateOfficer<CreateOfficerViewModel, BaseResponse> createOfficer)
        {
            _createOfficer = createOfficer;
        }

        ///<summary>
        /// Create officer
        /// </summary>
        /// <param name="officer">The officer body</param>
        /// <param name="cancellationToken"></param>
        /// <response code="201">Officer has been created</response>
        /// <response code="400">Invalid case properties</response>
        /// <response code="401">Invalid API-KEY</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, StatusCode = StatusCodes.Status201Created, Type = typeof(BaseResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, StatusCode = StatusCodes.Status400BadRequest, Type = typeof(BaseResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, StatusCode = StatusCodes.Status401Unauthorized, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Create(CreateOfficerViewModel officer, CancellationToken cancellationToken)
        {
            var response = await _createOfficer.RunAsync(officer, cancellationToken);

            if (response.Success)
                return CreatedAtAction(nameof(Create), response);

            return BadRequest(response);
        }

    }
}