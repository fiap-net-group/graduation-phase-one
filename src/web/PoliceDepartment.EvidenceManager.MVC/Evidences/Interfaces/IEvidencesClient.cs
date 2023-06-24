using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces
{
    public interface IEvidencesClient
    {
        Task<BaseResponseWithValue<Guid>> CreateEvidenceImage(IFormFile image, string accessToken, CancellationToken cancellationToken);
        Task<BaseResponse> CreateEvidenceAsync(CreateEvidenceViewModel evidenceViewModel, string accessToken, CancellationToken cancellationToken);
    }
}
