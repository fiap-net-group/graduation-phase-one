using AutoMapper;
using FluentValidation;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.Domain.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases
{
    public class CreateEvidence : ICreateEvidence<CreateEvidenceViewModel, BaseResponse>
    {
        private readonly ILoggerManager _logger;
        private readonly IUnitOfWork _uow;
        private readonly BaseResponse _response;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateEvidenceViewModel> _validator;

        public CreateEvidence(ILoggerManager logger, IUnitOfWork uow, IMapper mapper, IValidator<CreateEvidenceViewModel> validator)
        {
            _logger = logger;
            _uow = uow;
            _response = new();
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<BaseResponse> RunAsync(CreateEvidenceViewModel evidence, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Validating evidence properties");

            var validationResult = await _validator.ValidateAsync(evidence, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Invalid viewEvidence properties");
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));

                return _response.AsError(ResponseMessage.InvalidEvidence, errorMessages);
            }

            _logger.LogDebug("Begin Create evidence");

            var evidenceEntity = _mapper.Map<EvidenceEntity>(evidence);

            if(evidenceEntity is null)
            {
                _logger.LogError("An error ocurred at the mapper"); 
                throw new BusinessException("An unexpected error ocurred"); 
            }

            _logger.LogDebug("Adding evidence to database");

            await _uow.Evidence.CreateAsync(evidenceEntity, cancellationToken);

            if(!await _uow.SaveChangesAsync(cancellationToken))
            {
                _logger.LogError("An error ocurred at the database");
                throw new BusinessException("An unexpected error ocurred");
            }

            _logger.LogDebug("End Create evidence");
            return _response.AsSuccess();
        }
    }
}
