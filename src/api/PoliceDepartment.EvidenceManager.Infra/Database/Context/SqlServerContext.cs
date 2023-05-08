using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.Domain.Officer;
using PoliceDepartment.EvidenceManager.Infra.Database.Mappings;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.Infra.Database
{
    [ExcludeFromCodeCoverage]
    public class SqlServerContext : DbContext, IDatabaseContext
    {
        public DbSet<CaseEntity> Cases { get; set; }
        public DbSet<EvidenceEntity> Evidences { get; set; }
        public DbSet<OfficerEntity> Officers { get; set; }

        public SqlServerContext(DbContextOptions<SqlServerContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }

        public async Task<bool> AnyPendingMigrationsAsync()
        {
            try
            {
                var migrations = await Database.GetPendingMigrationsAsync();

                return migrations.Any();
            }
            catch
            {
                return false;
            }
        }

        public async Task MigrateAsync()
        {
            await Database.MigrateAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await Database.BeginTransactionAsync();
        }
    }
}
