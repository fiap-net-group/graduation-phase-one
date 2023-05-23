using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.Application.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.API.Features.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public sealed class AuthorizationController : ControllerBase
    {
        private readonly ILogin<LoginViewModel, BaseResponseWithValue<AccessTokenModel>> _login;

        public AuthorizationController(ILogin<LoginViewModel, BaseResponseWithValue<AccessTokenModel>> login)
        {
            _login = login;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(BaseResponseWithValue<AccessTokenModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, StatusCode = StatusCodes.Status400BadRequest, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Login(LoginViewModel login, CancellationToken cancellationToken)
        {
            var response = await _login.RunAsync(login, cancellationToken);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }
    }
}
