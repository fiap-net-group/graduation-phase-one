using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces
{
    public interface IEditEvidence
    {
        Task<BaseResponse> RunAsync(Guid id, EditEvidencePageViewModel viewModel, CancellationToken cancellationToken);
    }
}
