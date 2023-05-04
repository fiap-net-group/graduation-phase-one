using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.Domain.Officer;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Mappings
{
    public interface IDatabaseContext : IDisposable
    {
        public DbSet<CaseEntity> Cases { get; }
        public DbSet<EvidenceEntity> Evidences { get; }
        public DbSet<OfficerEntity> Officers { get; }

        Task<bool> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<bool> AnyPendingMigrationsAsync();
        Task MigrateAsync();
    }
}
