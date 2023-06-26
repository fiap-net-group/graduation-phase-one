using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.UseCases
{
    public class EvidenceCreate : ICreateEvidence
    {
        private readonly ILoggerManager _logger;
        private readonly IEvidencesClient _client;
        private readonly IOfficerUser _officerUser;

        public EvidenceCreate(ILoggerManager logger, IOfficerUser officerUser, IEvidencesClient client)
        {
            _logger = logger;
            _officerUser = officerUser;
            _client = client;
        }
        public async Task<BaseResponse> RunAsync(CreateEvidencePageViewModel viewModel, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Create evidence logic", ("CaseId", viewModel.CaseId));

            var createImageResponse = await _client.CreateEvidenceImageAsync(viewModel.Image, _officerUser.AccessToken, cancellationToken);

            if (!createImageResponse.Success)
            {
                _logger.LogWarning("MVC - Create evidence logic - Error creating image", ("CaseId", viewModel.CaseId), (nameof(createImageResponse), createImageResponse));

                return createImageResponse;
            }

            var request = new CreateEvidenceViewModel
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                CaseId = viewModel.CaseId,
                ImageId = Guid.Parse(createImageResponse.Value),
                OfficerId = viewModel.OfficerId
            };

            var response = await _client.CreateEvidenceAsync(request, _officerUser.AccessToken, cancellationToken);

            if (!response.Success)
            {
                _logger.LogWarning("MVC - Create evidence logic - Error creating evidence", ("CaseId", viewModel.CaseId), (nameof(response), response));

                await _client.DeleteEvidenceImageAsync(createImageResponse.Value, _officerUser.AccessToken, cancellationToken);

                return response;
            }

            _logger.LogDebug("MVC - Create evidence logic - Success", ("CaseId", viewModel.CaseId));

            return response;
        }
    }
}
