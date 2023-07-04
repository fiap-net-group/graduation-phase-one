using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces
{
    public interface ICreateCase
    {
        Task<BaseResponse> RunAsync(CreateCasePageViewModel viewModel, CancellationToken cancellationToken);
    }
}
