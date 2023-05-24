using Microsoft.EntityFrameworkCore;
using PoliceDepartment.EvidenceManager.Domain.Officer;
using PoliceDepartment.EvidenceManager.Infra.Database.Mappings;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Repositories
{
    [ExcludeFromCodeCoverage]
    public class OfficerRepository : IOfficerRepository
    {
        private readonly IAppDatabaseContext _context;

        public OfficerRepository(IAppDatabaseContext context)
        {
            _context = context;
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
