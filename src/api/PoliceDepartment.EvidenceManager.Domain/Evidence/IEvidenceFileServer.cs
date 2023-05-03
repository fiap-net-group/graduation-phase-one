namespace PoliceDepartment.EvidenceManager.Domain.Evidence
{
    public interface IEvidenceFileServer
    {
        public Task<Guid> Save(EvidenceImage image);
        public Task<EvidenceImage> GetBy(Guid id);
    }
}
