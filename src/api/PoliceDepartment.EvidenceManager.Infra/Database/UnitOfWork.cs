using Microsoft.EntityFrameworkCore.Storage;
using PoliceDepartment.EvidenceManager.SharedKernel.Case;
using PoliceDepartment.EvidenceManager.SharedKernel.Database;
using PoliceDepartment.EvidenceManager.SharedKernel.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.Officer;
using PoliceDepartment.EvidenceManager.Infra.Database.Mappings;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.Infra.Database
{
    [ExcludeFromCodeCoverage]
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IAppDatabaseContext _context;
        private IDbContextTransaction _transaction;

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

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is not null)
                throw new InvalidOperationException("A transaction is already opened");

            _transaction = await _context.BeginTransactionAsync(cancellationToken);
        }

        public async Task<bool> CommmitAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null)
                throw new InvalidOperationException("There is no transaction opened");

            var response = await _context.CommitAsync(_transaction, cancellationToken);

            await _transaction.DisposeAsync();

            return response;
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
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
                _context?.Dispose();
                _transaction?.Dispose();
            }
        }
    }
}
