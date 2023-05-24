using Microsoft.EntityFrameworkCore;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.Infra.Database.Mappings;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Repositories
{
    [ExcludeFromCodeCoverage]
    public class EvidenceRepository : IEvidenceRepository
    {
        private readonly IAppDatabaseContext _context;

        public EvidenceRepository(IAppDatabaseContext context)
        {
            _context = context;
        }

        public Task Create(EvidenceEntity evidence)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBy(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<EvidenceEntity> GetBy(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EvidenceEntity>> GetPaginated(int page, int rows)
        {
            throw new NotImplementedException();
        }

        public Task Update(EvidenceEntity evidence)
        {
            throw new NotImplementedException();
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
