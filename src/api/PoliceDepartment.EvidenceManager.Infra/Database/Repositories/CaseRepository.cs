using Microsoft.EntityFrameworkCore;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Infra.Database.Mappings;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Repositories
{
    [ExcludeFromCodeCoverage]
    public class CaseRepository : ICaseRepository
    {
        private readonly IAppDatabaseContext _context;

        public CaseRepository(IAppDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CaseEntity>> GetByOfficerId(Guid officerId, CancellationToken cancellationToken)
        {
            var cases = await _context.Cases.Where(c => c.OfficerId == officerId)
                                            .Include(c => c.Officer)
                                            .Include(c => c.Evidences)
                                            .ToListAsync(cancellationToken);

            return cases is null ? Enumerable.Empty<CaseEntity>() : cases;
        }

        public async Task<CaseEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _context.Cases.Where(c => c.Id == id)
                                             .Include(c => c.Officer)
                                             .Include(c => c.Evidences)
                                             .FirstOrDefaultAsync(cancellationToken);

            return entity ?? new CaseEntity();
        }

        public async Task UpdateAsync(CaseEntity entity, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                entity.UpdatedAt = DateTime.Now;
                _context.Cases.Update(entity);
            }, cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
        }
    }
}
