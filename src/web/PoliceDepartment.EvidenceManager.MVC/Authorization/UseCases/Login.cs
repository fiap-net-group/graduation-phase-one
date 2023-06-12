using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PoliceDepartment.EvidenceManager.MVC.Authorization.UseCases
{
    public sealed class Login : ILogin
    {
        private readonly IAuthenticationService _authService;
        private readonly IOfficerUser _officerUser;
        private readonly IAuthorizationClient _client;
        private readonly ILoggerManager _logger;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly BaseResponse _response;

        public Login(IAuthenticationService authService,
                     IOfficerUser officerUser,
                     IAuthorizationClient client,
                     ILoggerManager logger)
        {
            _authService = authService;
            _officerUser = officerUser;
            _client = client;
            _logger = logger;
            _tokenHandler = new();
            _response = new();
        }

        public async Task<BaseResponse> RunAsync(string username, string password, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Begin Login logic", ("username", username));

            var authResponse = await _client.AuthorizeAsync(username, password, cancellationToken);

            if (!authResponse.Success || !authResponse.Value.Valid())            
                return authResponse.ResponseDetails.Errors is not null && authResponse.ResponseDetails.Errors.Any() 
                    ? _response.AsError(errors: authResponse.ResponseDetails.Errors) 
                    : _response.AsError(ResponseMessage.InvalidCredentials);
            
            var accessToken = _tokenHandler.ReadToken(authResponse.Value.AccessToken) as JwtSecurityToken;

            var claims = new List<Claim>
            {
                new Claim("JWT", authResponse.Value.AccessToken),
            };

            claims.AddRange(accessToken.Claims);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await _authService.SignInAsync(_officerUser.HttpContext,
                                            CookieAuthenticationDefaults.AuthenticationScheme,
                                            new ClaimsPrincipal(claimsIdentity),
                                            new AuthenticationProperties
                                            {
                                                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                                                IsPersistent = true
                                            });

            _logger.LogDebug("MVC - Success Login logic", ("username", username));

            return _response.AsSuccess();
        }
    }
}
