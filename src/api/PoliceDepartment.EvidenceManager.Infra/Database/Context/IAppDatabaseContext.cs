using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using PoliceDepartment.EvidenceManager.SharedKernel.Case;
using PoliceDepartment.EvidenceManager.SharedKernel.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.Officer;
using PoliceDepartment.EvidenceManager.SharedKernel.Database;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Mappings
{
    public interface IAppDatabaseContext : IDatabaseContext
    {
        public DbSet<CaseEntity> Cases { get; }
        public DbSet<EvidenceEntity> Evidences { get; }
        public DbSet<OfficerEntity> Officers { get; }

        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
        Task<bool> CommitAsync(IDbContextTransaction transaction, CancellationToken cancellationToken);
    }
}
