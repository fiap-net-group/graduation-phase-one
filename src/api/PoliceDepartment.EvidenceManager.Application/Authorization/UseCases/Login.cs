using AutoMapper;
using Microsoft.Extensions.Configuration;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Authorization.UseCases
{
    public sealed class Login : ILogin<LoginViewModel, BaseResponseWithValue<AccessTokenViewModel>>
    {
        private readonly ILoggerManager _logger;
        private readonly IIdentityManager _identityManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        private readonly string _adminUserName;
        private readonly BaseResponseWithValue<AccessTokenViewModel> _response;

        public Login(ILoggerManager logger,
                     IIdentityManager identityManager,
                     IMapper mapper,
                     IUnitOfWork uow,
                     IConfiguration configuration)
        {
            _logger = logger;
            _identityManager = identityManager;
            _mapper = mapper;
            _uow = uow;

            _adminUserName = configuration["Admin:Email"];
            _response = new();
        }

        public async Task<BaseResponseWithValue<AccessTokenViewModel>> RunAsync(LoginViewModel login, CancellationToken cancellationToken)
        {
            BusinessException.ThrowIfNull(login);

            _logger.LogDebug("Begin Login", ("username", login.Username));

            var officer = login.Username != _adminUserName ? 
                        await _uow.Officer.GetByEmailAsync(login.Username, cancellationToken)
                        : new Domain.Officer.OfficerEntity().AsAdmin();

            if(!officer.Exists())
            {
                _logger.LogWarning("Officer don't exists", ("username", login.Username));

                return _response.AsError(ResponseMessage.InvalidCredentials);
            }

            var accessToken = await _identityManager.AuthenticateAsync(login.Username, login.Password, officer.Name, cancellationToken);

            if (!accessToken.Valid())
            {
                _logger.LogWarning("Invalid credentials", ("username", login.Username));

                return _response.AsError(ResponseMessage.InvalidCredentials);
            }

            _logger.LogDebug("Success Login", ("username", login.Username));

            return _response.AsSuccess(_mapper.Map<AccessTokenViewModel>(accessToken));
        }
    }
}
