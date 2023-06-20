using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces
{
    public interface IEditCase
    {
        Task<BaseResponse> RunAsync(Guid id, CaseViewModel viewModel, CancellationToken cancellationToken);
    }
}
