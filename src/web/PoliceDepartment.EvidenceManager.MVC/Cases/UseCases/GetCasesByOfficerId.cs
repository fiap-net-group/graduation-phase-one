using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.UseCases
{
    public class GetCasesByOfficerId : IGetCasesByOfficerId
    {
        private readonly ICasesClient _client;
        private readonly ILoggerManager _logger;

        public GetCasesByOfficerId(ICasesClient client, 
                                   ILoggerManager logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<BaseResponseWithValue<IEnumerable<CaseViewModel>>> RunAsync(Guid officerId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Getting cases by officer id", ("officerId",officerId));

            var casesResponse = await _client.GetByOfficerIdAsync(officerId, cancellationToken);

            if(!casesResponse.Success)
            {
                _logger.LogWarning("MVC - Error getting cases by officer id", ("officerId", officerId));

                return casesResponse;
            }

            _logger.LogDebug("MVC - Success getting cases by officer id", ("officerId", officerId));

            return casesResponse;
        }
    }
}
