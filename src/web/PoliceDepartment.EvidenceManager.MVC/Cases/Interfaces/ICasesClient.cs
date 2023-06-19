using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces
{
    public interface ICasesClient
    {
        Task<BaseResponseWithValue<IEnumerable<CaseViewModel>>> GetByOfficerIdAsync(Guid officerId, string accessToken, CancellationToken cancellationToken);
        Task<BaseResponse> CreateCaseAsync(CreateCaseViewModel caseViewModel, string accessToken, CancellationToken cancellationToken);
        Task<BaseResponseWithValue<CaseViewModel>> GetDetailsAsync(Guid id, string accessToken, CancellationToken cancellationToken);
    }
}
