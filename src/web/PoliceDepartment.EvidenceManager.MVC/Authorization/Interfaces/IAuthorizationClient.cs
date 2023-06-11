using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces
{
    public interface IAuthorizationClient
    {
        Task<BaseResponseWithValue<AccessTokenViewModel>> AuthorizeAsync(string username, string password, CancellationToken cancellationToken);
    }
}
