using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
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
        private readonly IOfficerUser _officerUser;

        public GetCasesByOfficerId(ICasesClient client, 
                                   ILoggerManager logger,
                                   IOfficerUser officerUser)
        {
            _client = client;
            _logger = logger;
            _officerUser = officerUser;
        }

        public async Task<BaseResponseWithValue<IEnumerable<CaseViewModel>>> RunAsync(Guid officerId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Getting cases by officer id", ("officerId",officerId));

            var casesResponse = await _client.GetByOfficerIdAsync(officerId, _officerUser.AccessToken, cancellationToken);

            if(!casesResponse.Success)
            {
                _logger.LogWarning("MVC - Error getting cases by officer id", ("officerId", officerId), (nameof(casesResponse), casesResponse));

                return casesResponse;
            }

            _logger.LogDebug("MVC - Success getting cases by officer id", ("officerId", officerId));

            return casesResponse;
        }
    }
}
