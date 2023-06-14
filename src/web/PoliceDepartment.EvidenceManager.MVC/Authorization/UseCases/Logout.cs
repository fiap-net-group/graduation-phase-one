using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Authorization.UseCases
{
    public class Logout : ILogout
    {
        private readonly ILoggerManager _logger;
        private readonly IAuthorizationClient _client;
        private readonly IAuthenticationService _authService;
        private readonly IOfficerUser _officerUser;

        private readonly BaseResponse _response;

        public Logout(ILoggerManager logger,
                      IAuthorizationClient authorizationClient,
                      IAuthenticationService authService,
                      IOfficerUser officerUser)
        {
            _logger = logger;
            _client = authorizationClient;
            _authService = authService;
            _officerUser = officerUser;

            _response = new();
        }

        public async Task<BaseResponse> RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Begin logout logic");

            if(!_officerUser.IsAuthenticated)
            {
                _logger.LogInformation("MVC - User can't logout because it's not authenticated");

                return _response.AsError(ResponseMessage.UserIsNotAuthenticated);
            }

            var apiLogoutResponse = await _client.SignOutAsync(_officerUser.AccessToken , cancellationToken);

            if(!apiLogoutResponse.Success)
                _logger.LogWarning("MVC - Error logging out at Auth API");

            await _authService.SignOutAsync(_officerUser.HttpContext, 
                                            CookieAuthenticationDefaults.AuthenticationScheme, 
                                            default);

            _logger.LogDebug("MVC - Success logout logic");

            return _response.AsSuccess();
        }
    }
}
