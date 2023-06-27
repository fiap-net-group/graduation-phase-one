using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces
{
    public interface IDeleteEvidence
    {
        Task<BaseResponse> RunAsync(Guid evidenceId, CancellationToken cancellationToken);
    }
}
