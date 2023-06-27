using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.UseCases
{
    public class DeleteEvidence : IDeleteEvidence
    {
        private readonly ILoggerManager _logger;
        private readonly IEvidencesClient _client;
        private readonly IOfficerUser _officerUser;

        public DeleteEvidence(ILoggerManager logger, IEvidencesClient client, IOfficerUser officerUser)
        {
            _logger = logger;
            _client = client;
            _officerUser = officerUser;
        }

        public async Task<BaseResponse> RunAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Delete evidence logic", ("evidenceId", id), ("officerId", _officerUser.Id));

            if (id == Guid.Empty)
            {
                _logger.LogInformation("MVC - Delete evidence logic - Invalid id", ("evidenceId", id));

                return new BaseResponse().AsError();
            }

            var response = await _client.DeleteAsync(id, _officerUser.AccessToken, cancellationToken);

            if (!response.Success)
            {
                _logger.LogWarning("MVC - Delete evidence logic - Error", ("evidenceId", id), (nameof(response), response));

                return response;
            }

            _logger.LogDebug("MVC - Delete evidence logic - Success", ("evidenceId", id), ("officerId", _officerUser.Id));

            return response;
        }
    }
}
