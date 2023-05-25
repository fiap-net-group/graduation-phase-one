using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.Domain.Officer;
using PoliceDepartment.EvidenceManager.Domain.Officer.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.Application.Officer.UseCases
{
    public class Officer : ICreateOfficer<OfficerViewModel, BaseResponse>
    {
        private readonly ILoggerManager _logger;
        private readonly BaseResponse _response;
        private readonly IOfficerRepository _repository;
        
        public Officer(ILoggerManager logger, IOfficerRepository repository)
        {
            _logger = logger;
            _repository = repository;
            _response = new();
        }

        public async Task<BaseResponse> RunAsync(OfficerViewModel viewModel, CancellationToken cancellationToken)
        {
            var officer = new OfficerEntity();

            officer.UserName = viewModel.UserName;
            officer.Email = viewModel.Email;

            await _repository.CreateAsync(officer, cancellationToken);

            return await Task.Run(() => _response.AsSuccess());

        }
    }
}