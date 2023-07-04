using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.UseCases
{
    public class EditEvidence : IEditEvidence
    {
        private readonly ILoggerManager _logger;
        private readonly IEvidencesClient _client;
        private readonly IOfficerUser _officerUser;

        public EditEvidence(ILoggerManager logger, IEvidencesClient client, IOfficerUser officerUser)
        {
            _logger = logger;
            _client = client;
            _officerUser = officerUser;
        }

        public async Task<BaseResponse> RunAsync(Guid id, EditEvidencePageViewModel evidenceViewModel, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Edit evidence logic", ("evidenceId", id), ("officerId", _officerUser.Id));

            var evidenceResponse = await _client.GetEvidenceByIdAsync(id, _officerUser.AccessToken, cancellationToken);

            if (!evidenceResponse.Success)
            {
                _logger.LogWarning("MVC - Edit evidence logic - Evidence doesn't exists", ("officerId", _officerUser.Id), (nameof(evidenceResponse), evidenceResponse));

                return evidenceResponse;
            }

            var imageId = evidenceResponse.Value.ImageId;

            if(evidenceViewModel.Image is null)
            {
                var imageResponse = await _client.CreateEvidenceImageAsync(evidenceViewModel.Image, _officerUser.AccessToken, cancellationToken);

                await _client.DeleteEvidenceImageAsync(evidenceResponse.Value.ImageId, _officerUser.AccessToken, cancellationToken);

                if (!imageResponse.Success)
                {
                    _logger.LogWarning("MVC - Edit evidence logic - Error updating image", ("officerId", _officerUser.Id), (nameof(imageResponse), imageResponse));

                    return imageResponse;
                }

                imageId = imageResponse.Value;
            }

            var viewModel = new EvidenceViewModel
            {
                Id = id,
                Name = evidenceViewModel.Name,
                Description = evidenceViewModel.Description,
                CaseId = evidenceResponse.Value.CaseId,
                ImageId = imageId
            };

            var updateResponse = await _client.EditAsync(id, viewModel, _officerUser.AccessToken, cancellationToken);

            if(!updateResponse.Success)
            {
                _logger.LogWarning("MVC - Edit evidence logic - Error", ("officerId", _officerUser.Id), (nameof(updateResponse), updateResponse));

                return updateResponse;
            }

            _logger.LogDebug("MVC - Edit evidence logic - Success", ("evidenceId", id), ("officerId", _officerUser.Id));

            return updateResponse;
        }
    }
}
