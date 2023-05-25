using AutoMapper;
using PoliceDepartment.EvidenceManager.Domain.Case.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Case.UseCases
{
    public class GetByOfficerId : IGetCasesByOfficerId<BaseResponseWithValue<IEnumerable<CaseViewModel>>>
    {
        private readonly ILoggerManager _logger;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly BaseResponseWithValue<IEnumerable<CaseViewModel>> _response;

        public GetByOfficerId(ILoggerManager logger,
                              IUnitOfWork uow,
                              IMapper mapper)
        {
            _logger = logger;
            _uow = uow;
            _response = new();
            _mapper = mapper;
        }

        public async Task<BaseResponseWithValue<IEnumerable<CaseViewModel>>> RunAsync(Guid officerId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Begin Get cases by officer id", ("officerId", officerId));

            var cases = await _uow.Case.GetByOfficerId(officerId, cancellationToken);

            _logger.LogDebug("End Get cases by officer id", ("officerId", officerId), ("cases", cases));
            
            return _response.AsSuccess(cases.Any() ? _mapper.Map<IEnumerable<CaseViewModel>>(cases) : Enumerable.Empty<CaseViewModel>());
        }
    }
}
