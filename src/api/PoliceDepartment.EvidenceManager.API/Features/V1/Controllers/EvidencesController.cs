using Microsoft.AspNetCore.Mvc;

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
    }
}
