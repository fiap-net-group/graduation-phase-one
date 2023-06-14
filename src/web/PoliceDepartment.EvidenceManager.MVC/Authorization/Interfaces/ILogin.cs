using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces
{
    public interface ILogin
    {
        Task<BaseResponse> RunAsync(string username, string password, CancellationToken cancellationToken);
    }
}
