using AutoMapper;
using FluentValidation;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Case.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Case.UseCases
{
    public class CreateCase : ICreateCase<CreateCaseViewModel, BaseResponse>
    {
        private readonly ILoggerManager _logger;
        private readonly IUnitOfWork _uow;
        private readonly BaseResponse _response;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCaseViewModel> _validator;

        public CreateCase(ILoggerManager logger, IUnitOfWork uow, IMapper mapper, IValidator<CreateCaseViewModel> validator)
        {
            _logger = logger;
            _uow = uow;
            _response = new();
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<BaseResponse> RunAsync(CreateCaseViewModel @case, CancellationToken cancellationToken)
        {

            _logger.LogDebug("Validating case properties");

            var validationResult = await _validator.ValidateAsync(@case, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Invalid viewCase properties");
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));

                return _response.AsError(ResponseMessage.InvalidCase, errorMessages);
            }

            _logger.LogDebug("Begin Create case");

            var caseEntity = _mapper.Map<CaseEntity>(@case);

            if (caseEntity is null) 
            { 
                _logger.LogError("An error ocurred at the mapper"); 
                throw new BusinessException("An unexpected error ocurred"); 
            }

            _logger.LogDebug("Adding case to database");

            await _uow.Case.AddAsync(caseEntity, cancellationToken);

            if (!await _uow.SaveChangesAsync(cancellationToken))
            {
                _logger.LogError("An error ocurred at the database");

                throw new InfrastructureException("An unexpected error ocurred");
            }

            _logger.LogDebug("Success Create case");

            return _response.AsSuccess();
        }
    }
}
