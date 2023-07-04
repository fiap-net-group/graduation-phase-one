using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces
{
    public interface IDeleteCase
    {
        Task<BaseResponse> RunAsync(Guid id, CancellationToken cancellationToken);
    }
}
