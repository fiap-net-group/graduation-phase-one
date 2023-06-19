using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces
{
    public interface IGetCaseDetails
    {
        Task<BaseResponseWithValue<CaseViewModel>> RunAsync(Guid caseId, CancellationToken cancellationToken);
    }
}
