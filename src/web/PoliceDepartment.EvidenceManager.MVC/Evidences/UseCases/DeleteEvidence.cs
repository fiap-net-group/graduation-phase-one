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

        public async Task<BaseResponse> RunAsync(Guid id, Guid imageId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Delete case logic", ("evidenceId", id), ("imageId", imageId), ("officerId", _officerUser.Id));

            if (id == Guid.Empty || imageId == Guid.Empty)
            {
                _logger.LogInformation("MVC - Delete case logic - Invalid", ("evidenceId", id), ("imageId", imageId), ("officerId", _officerUser.Id));

                return new BaseResponse().AsError();
            }

            var response = await _client.DeleteEvidenceAsync(id, _officerUser.AccessToken, cancellationToken);

            if (!response.Success)
            {
                _logger.LogWarning("MVC - Delete case logic - Error", ("evidenceId", id), ("imageId", imageId), ("officerId", _officerUser.Id), (nameof(response), response));

                return response;
            }

            await _client.DeleteEvidenceImageAsync(imageId.ToString(), _officerUser.AccessToken, cancellationToken);

            _logger.LogDebug("MVC - Delete case logic - Success", ("evidenceId", id), ("imageId", imageId), ("officerId", _officerUser.Id));

            return response;
        }
    }
}
