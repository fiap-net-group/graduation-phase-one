using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.UseCases
{
    public class CreateEvidence : ICreateEvidence
    {
        private readonly ILoggerManager _logger;
        private readonly IEvidencesClient _client;
        private readonly IOfficerUser _officerUser;

        public CreateEvidence(ILoggerManager logger, IOfficerUser officerUser, IEvidencesClient client)
        {
            _logger = logger;
            _officerUser = officerUser;
            _client = client;
        }
        public async Task<BaseResponse> RunAsync(CreateEvidencePageViewModel viewModel, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Create evidence logic", ("CaseId", viewModel.CaseId));

            var request = new CreateEvidenceViewModel
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                CaseId = viewModel.CaseId,
                ImageId = Guid.NewGuid(),
                OfficerId = viewModel.OfficerId
            };

            var response = await _client.CreateEvidenceAsync(request, _officerUser.AccessToken, cancellationToken);

            if (!response.Success)
            {
                _logger.LogWarning("MVC - Create evidence logic - Error", ("CaseId", viewModel.CaseId), (nameof(response), response));

                return response;
            }

            _logger.LogDebug("MVC - Create evidence logic - Success", ("CaseId", viewModel.CaseId));

            return response;

        }
    }
}
