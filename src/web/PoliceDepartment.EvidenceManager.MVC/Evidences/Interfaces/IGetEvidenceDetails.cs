using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces
{
    public interface IGetEvidenceDetails
    {
        Task<BaseResponseWithValue<EvidenceDetailViewModel>> RunAsync(Guid id, CancellationToken cancellationToken);
    }
}
