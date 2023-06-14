using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces
{
    public interface ILogout
    {
        Task<BaseResponse> RunAsync(CancellationToken cancellationToken);
    }
}
