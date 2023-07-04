using PoliceDepartment.EvidenceManager.SharedKernel.Case;
using PoliceDepartment.EvidenceManager.SharedKernel.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.Officer;

namespace PoliceDepartment.EvidenceManager.SharedKernel.Database
{
    public interface IUnitOfWork : IDisposable
    {
        ICaseRepository Case { get; }
        IEvidenceRepository Evidence { get; }
        IOfficerRepository Officer { get; }

        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task<bool> CommmitAsync(CancellationToken cancellationToken);
    }
}
