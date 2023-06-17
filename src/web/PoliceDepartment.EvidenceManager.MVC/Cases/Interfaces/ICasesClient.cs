using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces
{
    public interface ICasesClient
    {
        Task<BaseResponseWithValue<IEnumerable<CaseViewModel>>> GetByOfficerIdAsync(Guid officerId, CancellationToken cancellationToken);
    }
}
