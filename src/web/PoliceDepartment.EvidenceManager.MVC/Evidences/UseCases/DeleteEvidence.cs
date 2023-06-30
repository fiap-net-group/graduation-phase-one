using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
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
                _logger.LogInformation("MVC - Delete evidence logic - Invalid", ("evidenceId", id), ("officerId", _officerUser.Id));

                return new BaseResponse().AsError();
            }

            var evidenceResponse = await _client.GetEvidenceByIdAsync(id, _officerUser.AccessToken, cancellationToken);

            if(!evidenceResponse.Success)
            {
                _logger.LogWarning("MVC - Delete evidence logic - Evidence doesn't exists", ("evidenceId", id), ("officerId", _officerUser.Id), (nameof(evidenceResponse), evidenceResponse));

                return evidenceResponse;
            }

            var response = await _client.DeleteEvidenceAsync(id, _officerUser.AccessToken, cancellationToken);

            if (!response.Success)
            {
                _logger.LogWarning("MVC - Delete evidence logic - Error", ("evidenceId", id), ("officerId", _officerUser.Id), (nameof(response), response));

                return response;
            }

            await _client.DeleteEvidenceImageAsync(evidenceResponse.Value.ImageId.ToString(), _officerUser.AccessToken, cancellationToken);

            _logger.LogDebug("MVC - Delete evidence logic - Success", ("evidenceId", id), ("officerId", _officerUser.Id));

            return response;
        }
    }
}
