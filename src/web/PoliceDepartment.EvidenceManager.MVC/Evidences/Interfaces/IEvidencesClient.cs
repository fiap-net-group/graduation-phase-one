using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces
{
    public interface IEvidencesClient
    {
        Task<BaseResponseWithValue<string>> CreateEvidenceImageAsync(IFormFile image, string accessToken, CancellationToken cancellationToken);
        Task<BaseResponse> EditAsync(Guid id, EvidenceViewModel evidenceViewModel, string accessToken, CancellationToken cancellationToken);
        Task<BaseResponse> DeleteEvidenceAsync(Guid id, string accessToken, CancellationToken cancellationToken);
        Task<BaseResponseWithValue<string>> DeleteEvidenceImageAsync(string imageId, string accessToken, CancellationToken cancellationToken);
        Task<BaseResponse> CreateEvidenceAsync(CreateEvidenceViewModel evidenceViewModel, string accessToken, CancellationToken cancellationToken);
        Task<BaseResponseWithValue<EvidenceViewModel>> GetEvidenceByIdAsync(Guid id, string accessToken, CancellationToken cancellationToken);
        Task<BaseResponseWithValue<string>> GetEvidenceImageAsync(string imageId, string accessToken, CancellationToken cancellationToken);
    }
}
