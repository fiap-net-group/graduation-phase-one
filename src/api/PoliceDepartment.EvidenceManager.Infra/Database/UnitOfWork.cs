using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.Domain.Officer;
using PoliceDepartment.EvidenceManager.Infra.Database.Mappings;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.Infra.Database
{
    [ExcludeFromCodeCoverage]
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IAppDatabaseContext _context;
        public ICaseRepository Case { get; }
        public IEvidenceRepository Evidence { get; }
        public IOfficerRepository Officer { get; }

        public UnitOfWork(IAppDatabaseContext context,
                          ICaseRepository caseRepository, 
                          IEvidenceRepository evidenceRepository, 
                          IOfficerRepository officerRepository)
        {
            _context = context;
            Case = caseRepository;
            Evidence = evidenceRepository;
            Officer = officerRepository;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
