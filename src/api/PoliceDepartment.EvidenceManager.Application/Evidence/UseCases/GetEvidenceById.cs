using AutoMapper;
using PoliceDepartment.EvidenceManager.SharedKernel.Database;
using PoliceDepartment.EvidenceManager.SharedKernel.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases
{
    public class GetEvidenceById : IGetEvidenceById<BaseResponseWithValue<EvidenceViewModel>>
    {
        private readonly ILoggerManager _logger;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly BaseResponseWithValue<EvidenceViewModel> _response;

        public GetEvidenceById(ILoggerManager logger,
                        IUnitOfWork uow,
                        IMapper mapper)
        {
            _logger = logger;
            _uow = uow;
            _response = new();
            _mapper = mapper;
        }                                       

        public async Task<BaseResponseWithValue<EvidenceViewModel>> RunAsync(Guid evidenceId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Begin Get evidence by id", ("evidenceId", evidenceId));

            var evidence = await _uow.Evidence.GetByIdAsync(evidenceId, cancellationToken);

            _logger.LogDebug("End Get evidence by id", ("evidenceId", evidenceId), ("evidence", evidence));

            if(!evidence.Exists()){
                _logger.LogWarning("Evidence doesn't exists", ("evidenceId", evidenceId));
                return _response.AsError(ResponseMessage.EvidenceDontExists);
            }
            
            return _response.AsSuccess(_mapper.Map<EvidenceViewModel>(evidence));
            //return _response.AsSuccess(ev.Any() ? _mapper.Map<IEnumerable<CaseViewModel>>(cases) : Enumerable.Empty<CaseViewModel>());
        }
    }
}
