using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.Application.Officer;
using PoliceDepartment.EvidenceManager.Domain.Officer.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.API.Features.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public sealed class OfficerController : ControllerBase
    {
        private readonly ICreateOfficer<OfficerViewModel,BaseResponse> _createOfficer;

        public OfficerController(ICreateOfficer<OfficerViewModel,BaseResponse> createOfficer)
        {
            _createOfficer = createOfficer;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(OfficerViewModel officer, CancellationToken cancellationToken)
        {
            var response = await _createOfficer.RunAsync(officer,cancellationToken);
            if(response.Success)
                return Created(new Uri("http://teste.com.br"), response);

            return BadRequest(response);
        }

    }
}