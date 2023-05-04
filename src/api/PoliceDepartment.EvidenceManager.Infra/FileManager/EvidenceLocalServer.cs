using PoliceDepartment.EvidenceManager.Domain.Evidence;

namespace PoliceDepartment.EvidenceManager.Infra.FileManager
{
    public class EvidenceLocalServer : IEvidenceFileServer
    {
        public Task<EvidenceImage> GetBy(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> Save(EvidenceImage image)
        {
            throw new NotImplementedException();
        }
    }
}
