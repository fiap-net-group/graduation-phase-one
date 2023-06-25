using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.UseCases
{
    public class GetEvidenceDetails : IGetEvidenceDetails
    {
        private readonly ILoggerManager _logger;
        private readonly IEvidencesClient _client;
        private readonly IOfficerUser _officerUser;
        private readonly BaseResponseWithValue<EvidenceDetailViewModel> _response;

        public GetEvidenceDetails(ILoggerManager logger, IEvidencesClient client, IOfficerUser officerUser)
        {
            _logger = logger;
            _client = client;
            _officerUser = officerUser;

            _response = new();
        }

        public async Task<BaseResponseWithValue<EvidenceDetailViewModel>> RunAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Begin get evidence's details logic", ("evidenceId", id));

            var evidenceResponse = await _client.GetEvidenceByIdAsync(id, _officerUser.AccessToken, cancellationToken);

            if(!evidenceResponse.Success || evidenceResponse.Value is null || !evidenceResponse.Value.Valid())
            {
                _logger.LogWarning("MVC - Get evidence's details logic - Error getting evidence", ("evidenceId", id), (nameof(evidenceResponse), evidenceResponse));

                return _response.AsError(evidenceResponse.GetMessage(), evidenceResponse.ResponseDetails.Errors);
            }

            _logger.LogDebug("MVC - Success getting evidence, getting now the image", ("evidenceId", id), ("imageId", evidenceResponse.Value.ImageId));
            
            var evidenceImageResponse = await _client.GetEvidenceImageAsync(evidenceResponse.Value.ImageId, _officerUser.AccessToken, cancellationToken);

            if(!evidenceImageResponse.Success)
            {
                _logger.LogWarning("MVC - Create evidence logic - Error creating image", ("evidenceId", id), ("imageId", evidenceResponse.Value.ImageId), (nameof(evidenceImageResponse), evidenceImageResponse));

                return _response.AsError(evidenceImageResponse.GetMessage(), evidenceImageResponse.ResponseDetails.Errors);
            }

            _logger.LogDebug("MVC - Get evidence's details logic - Success", ("evidenceId", id));

            return _response.AsSuccess(new EvidenceDetailViewModel(evidenceResponse.Value, evidenceImageResponse.Value));
        }
    }
}
