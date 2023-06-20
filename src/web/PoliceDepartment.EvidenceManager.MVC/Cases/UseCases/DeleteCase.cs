using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.UseCases
{
    public class DeleteCase : IDeleteCase
    {
        private readonly ICasesClient _client;
        private readonly ILoggerManager _logger;
        private readonly IOfficerUser _officerUser;

        public DeleteCase(ICasesClient client, ILoggerManager logger, IOfficerUser officerUser)
        {
            _client = client;
            _logger = logger;
            _officerUser = officerUser;
        }

        public async Task<BaseResponse> RunAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Delete case logic", ("caseId", id), ("officerId", _officerUser.Id));

            var response = await _client.DeleteAsync(id, _officerUser.AccessToken, cancellationToken);

            if (!response.Success)
            {
                _logger.LogWarning("MVC - Delete case logic - Error", ("officerId", _officerUser.Id), (nameof(response), response));

                return response;
            }

            _logger.LogDebug("MVC - Delete case logic - Success", ("caseId", id), ("officerId", _officerUser.Id));

            return response;
        }
    }
}
