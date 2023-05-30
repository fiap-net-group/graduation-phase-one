using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

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

        /// <summary>
        /// Authenticate the officer
        /// </summary>
        /// <param name="login">The user credentials</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">The access token</response>
        /// <response code="400">The reason of non-authentication</response>
        /// <response code="401">Invalid API-KEY</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, Type = typeof(AccessTokenModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, StatusCode = StatusCodes.Status400BadRequest, Type = typeof(BaseResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, StatusCode = StatusCodes.Status401Unauthorized, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Login(LoginViewModel login, CancellationToken cancellationToken)
        {
            var response = await _login.RunAsync(login, cancellationToken);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }
    }
}
