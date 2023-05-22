using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PoliceDepartment.EvidenceManager.Application.Authorization.UseCases
{
    public sealed class Login : ILogin<LoginViewModel, BaseResponse<AccessTokenModel>>
    {
        private readonly ILoggerManager _logger;
        private readonly IIdentityManager _identityManager;
        private readonly BaseResponse<AccessTokenModel> _response;

        public Login(ILoggerManager logger,
                     IIdentityManager identityManager)
        {
            _logger = logger;
            _identityManager = identityManager;
            _response = new();
        }

        public async Task<BaseResponse<AccessTokenModel>> RunAsync(LoginViewModel login, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Begin Login", ("username", login.Username));

            BusinessException.ThrowIfNull(login);

            var accessToken = await _identityManager.AuthenticateAsync(login.Username, login.Password);

            if (!accessToken.Valid())
            {
                _logger.LogWarning("Invalid credentials", ("username", login.Username));

                return _response.AsError("Invalid credentials");
            }

            _logger.LogDebug("Success Login", ("username", login.Username));

            return _response.AsSuccess(accessToken);
        }
    }
}
