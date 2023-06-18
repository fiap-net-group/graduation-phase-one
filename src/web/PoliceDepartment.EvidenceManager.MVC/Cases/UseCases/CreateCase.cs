using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.UseCases
{
    public class CreateCase : ICreateCase
    {
        private readonly ILoggerManager _logger;
        private readonly ICasesClient _client;
        private readonly IOfficerUser _officerUser;

        public CreateCase(ILoggerManager logger, ICasesClient client, IOfficerUser officerUser)
        {
            _logger = logger;
            _client = client;
            _officerUser = officerUser;
        }

        public async Task<BaseResponse> RunAsync(CreateCasePageViewModel viewModel, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Create case logic", ("officerId", _officerUser.Id));

            var request = new CreateCaseViewModel
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                OfficerId = _officerUser.Id
            };

            var response = await _client.CreateCaseAsync(request, _officerUser.AccessToken, cancellationToken);

            if (!response.Success)
            {
                _logger.LogWarning("MVC - Create case logic - Error", ("officerId", _officerUser.Id), (nameof(response), response));

                return response;
            }

            _logger.LogDebug("MVC - Create case logic - Success", ("officerId", _officerUser.Id));

            return response;
        }
    }
}
