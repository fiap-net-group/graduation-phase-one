using PoliceDepartment.EvidenceManager.Domain.Officer;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Repositories
{
    [ExcludeFromCodeCoverage]
    public class OfficerRepository : IOfficerRepository
    {
        public SqlServerContext _DbContext { get; set; }

        public OfficerRepository(SqlServerContext dbContext)
        {
            _DbContext = dbContext;
        }
        public async Task CreateAsync(OfficerEntity officer, CancellationToken cancellationToken)
        {
            try
            {
                await _DbContext.Officers.AddAsync(officer, cancellationToken);
                await _DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {

        }
    }
}
