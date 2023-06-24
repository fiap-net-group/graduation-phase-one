using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces
{
    public interface IEvidencesClient
    {
        Task<BaseResponseWithValue<string>> CreateEvidenceImage(IFormFile image, string accessToken, CancellationToken cancellationToken);
        Task<BaseResponseWithValue<string>> DeleteEvidenceImage(string imageId, string accessToken, CancellationToken cancellationToken);
        Task<BaseResponse> CreateEvidenceAsync(CreateEvidenceViewModel evidenceViewModel, string accessToken, CancellationToken cancellationToken);
    }
}
