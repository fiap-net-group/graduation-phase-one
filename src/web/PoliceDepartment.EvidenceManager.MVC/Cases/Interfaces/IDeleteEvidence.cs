using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces
{
    public interface IDeleteEvidence
    {
        Task<BaseResponse> RunAsync(Guid id, Guid imageId, CancellationToken cancellationToken);
    }
}
