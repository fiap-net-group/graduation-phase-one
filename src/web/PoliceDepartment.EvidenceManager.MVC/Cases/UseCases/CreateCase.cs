using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.UseCases
{
    public class CreateCase : ICreateCase
    {
        private readonly ILoggerManager _logger;
        private readonly ICasesClient _client;

        public CreateCase(ILoggerManager logger, ICasesClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<BaseResponse> RunAsync(CreateCasePageViewModel model, CancellationToken cancellationToken)
        {
            return new BaseResponse();
        }
    }
}
