using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces
{
    public interface ICreateEvidence
    {
        Task<BaseResponse> RunAsync(CreateEvidencePageViewModel viewModel, CancellationToken cancellationToken);
    }
}
