using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.Domain.Officer;

namespace PoliceDepartment.EvidenceManager.Domain.Database
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
