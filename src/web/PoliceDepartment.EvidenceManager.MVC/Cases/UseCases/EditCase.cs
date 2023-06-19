using Microsoft.AspNetCore.Http.HttpResults;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.UseCases
{
    public class EditCase : IEditCase
    {
        private readonly ICasesClient _client;
        private readonly ILoggerManager _logger;
        private readonly IOfficerUser _officerUser;

        public EditCase(ICasesClient client, ILoggerManager logger, IOfficerUser officerUser)
        {
            _client = client;
            _logger = logger;
            _officerUser = officerUser;
        }

        public async Task<BaseResponse> RunAsync(Guid id, CaseViewModel viewModel, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Edit case logic", ("caseId", id), ("officerId", _officerUser.Id));

            var response = await _client.EditAsync(id, viewModel, _officerUser.AccessToken, cancellationToken);

            if (!response.Success)
            {
                _logger.LogWarning("MVC - Edit case logic - Error", ("officerId", _officerUser.Id), (nameof(response), response));

                return response;
            }

            _logger.LogDebug("MVC - Edit case logic - Success", ("caseId", id), ("officerId", _officerUser.Id));

            return response;
        }
    }
}
