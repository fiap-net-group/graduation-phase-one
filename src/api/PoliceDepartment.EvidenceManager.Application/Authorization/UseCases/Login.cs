using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.Application.Authorization.UseCases
{
    public sealed class Login : ILogin<LoginViewModel, BaseResponseWithValue<AccessTokenModel>>
    {
        private readonly ILoggerManager _logger;
        private readonly IIdentityManager _identityManager;
        private readonly BaseResponseWithValue<AccessTokenModel> _response;

        public Login(ILoggerManager logger,
                     IIdentityManager identityManager)
        {
            _logger = logger;
            _identityManager = identityManager;
            _response = new();
        }

        public async Task<BaseResponseWithValue<AccessTokenModel>> RunAsync(LoginViewModel login, CancellationToken cancellationToken)
        {
            BusinessException.ThrowIfNull(login);

            _logger.LogDebug("Begin Login", ("username", login.Username));

            var accessToken = await _identityManager.AuthenticateAsync(login.Username, login.Password);

            if (!accessToken.Valid())
            {
                _logger.LogWarning("Invalid credentials", ("username", login.Username));

                return _response.AsError(ResponseMessage.InvalidCredentials);
            }

            _logger.LogDebug("Success Login", ("username", login.Username));

            return _response.AsSuccess(accessToken);
        }
    }
}
