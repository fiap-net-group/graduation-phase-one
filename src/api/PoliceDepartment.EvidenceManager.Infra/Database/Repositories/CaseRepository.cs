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
            var cases = await _context.Cases.Where(c => c.OfficerId == officerId).ToListAsync(cancellationToken);

            return cases is null ? Enumerable.Empty<CaseEntity>() : cases;
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
