using AutoMapper;
using PoliceDepartment.EvidenceManager.Domain.Case.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Case.UseCases
{
    public class GetCaseById : IGetById<BaseResponseWithValue<CaseViewModel>>
    {
        private readonly ILoggerManager _logger;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly BaseResponseWithValue<CaseViewModel> _response;

        public GetCaseById(ILoggerManager logger, IUnitOfWork uow, IMapper mapper)
        {
            _logger = logger;
            _uow = uow;
            _mapper = mapper;
            _response = new();
        }

        public async Task<BaseResponseWithValue<CaseViewModel>> RunAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Begin Get case by id", ("id", id));

            var entity = await _uow.Case.GetByIdAsync(id, cancellationToken);

            if(!entity.Exists())
            {
                _logger.LogWarning("Case doesn't exists", ("id", id));

                return _response.AsError(ResponseMessage.CaseDontExists);
            }

            _logger.LogDebug("Begin Get case by id", ("id", id), ("case",entity));

            return _response.AsSuccess(_mapper.Map<CaseViewModel>(entity));
        }
    }
}
