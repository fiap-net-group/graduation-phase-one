using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PoliceDepartment.EvidenceManager.SharedKernel.Database;
using PoliceDepartment.EvidenceManager.SharedKernel.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Evidence.UseCases
{
    public class GetEvidencesByCaseId : IGetEvidencesByCaseId<BaseResponseWithValue<IEnumerable<EvidenceViewModel>>>
    {
        private readonly ILoggerManager _logger;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly BaseResponseWithValue<IEnumerable<EvidenceViewModel>> _response;

        public GetEvidencesByCaseId(ILoggerManager logger,
                            IUnitOfWork uow,
                            IMapper mapper)
        {
            _logger = logger;
            _uow = uow;
            _response = new();
            _mapper = mapper;
        }
        public async Task<BaseResponseWithValue<IEnumerable<EvidenceViewModel>>> RunAsync(Guid caseId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Begin Get evidences by case id", ("caseId", caseId));

            var evidences = await _uow.Evidence.GetByCaseIdAsync(caseId, cancellationToken);

            _logger.LogDebug("End Get evidences by case id", ("caseId", caseId), ("evidences", evidences));
            
            return _response.AsSuccess(evidences.Any() ? _mapper.Map<IEnumerable<EvidenceViewModel>>(evidences) : Enumerable.Empty<EvidenceViewModel>());
        }
    }
}