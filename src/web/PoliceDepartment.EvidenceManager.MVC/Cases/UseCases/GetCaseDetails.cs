using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.UseCases
{
    public class GetCaseDetails : IGetCaseDetails
    {
        private readonly ICasesClient _client;
        private readonly ILoggerManager _logger;
        private readonly IOfficerUser _officerUser;

        public GetCaseDetails(ICasesClient client, ILoggerManager logger, IOfficerUser officerUser)
        {
            _client = client;
            _logger = logger;
            _officerUser = officerUser;
        }

        public async Task<BaseResponseWithValue<CaseViewModel>> RunAsync(Guid caseId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Getting case details by id", (nameof(caseId), caseId), ("officerId", _officerUser.Id));

            var detailsResponse = await _client.GetDetailsAsync(caseId, _officerUser.AccessToken, cancellationToken);

            if (!detailsResponse.Success)
            {
                _logger.LogWarning("MVC - Error getting case details by id", (nameof(caseId), caseId), ("officerId", _officerUser.Id), (nameof(detailsResponse), detailsResponse));

                return detailsResponse;
            }

            _logger.LogDebug("MVC - Success getting case details by id", (nameof(caseId), caseId), ("officerId", _officerUser.Id));

            return detailsResponse;
        }
    }
}
