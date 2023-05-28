using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.Domain.Officer;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Repositories
{
    [ExcludeFromCodeCoverage]
    public class OfficerRepository : IOfficerRepository
    {
        private readonly ILoggerManager _logger;
        public SqlServerContext _DbContext { get; set; }

        public OfficerRepository(ILoggerManager logger,SqlServerContext dbContext)
        {
            _logger = logger;
            _DbContext = dbContext;
        }
        public async Task CreateAsync(OfficerEntity officer, CancellationToken cancellationToken)
        {            
            await _DbContext.Officers.AddAsync(officer, cancellationToken);
            var success = await _DbContext.SaveChangesAsync(cancellationToken);

            if(success == 0){
                _logger.LogError("An error ocurred at the database");
                throw new Exception("Error on create officer");
            }            
        }

        public void Dispose()
        {

        }
    }
}
