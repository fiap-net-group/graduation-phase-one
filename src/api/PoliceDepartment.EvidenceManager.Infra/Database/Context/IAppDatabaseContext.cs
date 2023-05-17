using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.Domain.Officer;
using PoliceDepartment.EvidenceManager.Domain.Database;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Mappings
{
    public interface IAppDatabaseContext : IDatabaseContext
    {
        public DbSet<CaseEntity> Cases { get; }
        public DbSet<EvidenceEntity> Evidences { get; }
        public DbSet<OfficerEntity> Officers { get; }

        Task<bool> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
