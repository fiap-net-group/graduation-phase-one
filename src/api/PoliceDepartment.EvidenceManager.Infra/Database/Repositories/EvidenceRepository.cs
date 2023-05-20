using PoliceDepartment.EvidenceManager.Domain.Evidence;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Repositories
{
    [ExcludeFromCodeCoverage]
    public class EvidenceRepository : IEvidenceRepository
    {
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

        }
    }
}
