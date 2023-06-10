using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.Domain.Officer;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Repositories
{
    [ExcludeFromCodeCoverage]
    public class OfficerRepository : IOfficerRepository
    {
        private readonly ILoggerManager _logger;
        private readonly SqlServerContext _context;

        public OfficerRepository(ILoggerManager logger, SqlServerContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task CreateAsync(OfficerEntity officer, CancellationToken cancellationToken)
        {
            await _context.Officers.AddAsync(officer, cancellationToken);
            var success = await _context.SaveChangesAsync(cancellationToken);

            if (!success)
            {
                _logger.LogError("An error ocurred at the database");
                throw new InfrastructureException("Error on create officer");
            }
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
