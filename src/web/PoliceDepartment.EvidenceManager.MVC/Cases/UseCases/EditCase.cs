using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.UseCases
{
    public class EditCase : IEditCase
    {
        public Task<BaseResponse> RunAsync(Guid id, CaseViewModel viewModel, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
