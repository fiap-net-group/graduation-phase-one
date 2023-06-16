using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces
{
    public interface IAuthorizationClient
    {
        Task<BaseResponseWithValue<AccessTokenViewModel>> SignInAsync(string username, string password, CancellationToken cancellationToken);
        Task<BaseResponse> SignOutAsync(string accessToken, CancellationToken cancellationToken);
    }
}
