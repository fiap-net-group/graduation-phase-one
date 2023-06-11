using AutoMapper;
using FluentValidation;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.Domain.Officer;
using PoliceDepartment.EvidenceManager.Domain.Officer.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Officer.UseCases
{
    public class CreateOfficer : ICreateOfficer<CreateOfficerViewModel, BaseResponse>
    {
        private readonly ILoggerManager _logger;
        private readonly BaseResponse _response;
        private readonly IMapper _mapper;
        private readonly IOfficerRepository _Officerrepository;
        private readonly IIdentityManager _identityManager;
        private readonly IValidator<CreateOfficerViewModel> _validator;

        public CreateOfficer( ILoggerManager logger, 
                        IIdentityManager identityManager, 
                        IOfficerRepository officerrepository, 
                        IMapper mapper, 
                        IValidator<CreateOfficerViewModel> validator)
        {
            _logger = logger;
            _identityManager = identityManager;
            _Officerrepository = officerrepository;
            _response = new();
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<BaseResponse> RunAsync(CreateOfficerViewModel viewModel, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Validating user properties");

            var validationResult = await _validator.ValidateAsync(viewModel, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Invalid viewCase properties");
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));

                return _response.AsError(ResponseMessage.InvalidCredentials, errorMessages);
            }

            _logger.LogDebug("Begin Create user");

            var resultIdentity = await _identityManager.CreateAsync(viewModel.Email, viewModel.UserName, viewModel.Password, Enum.GetName(viewModel.OfficerType));

            if(!resultIdentity.Succeeded){
                _logger.LogWarning("Error on create user");
                var errorMessages = string.Join(", ", resultIdentity.Errors.Select(e => e.Description));

                return _response.AsError(ResponseMessage.InvalidCredentials, errorMessages);
            }

            _logger.LogDebug("Begin Create officer");

            var entity = await _identityManager.FindByEmailAsync(viewModel.Email);

            OfficerEntity officer = _mapper.Map<OfficerEntity>(entity);

            await _Officerrepository.CreateAsync(officer, cancellationToken);

            return _response.AsSuccess();

        }
    }
}