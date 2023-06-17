namespace PoliceDepartment.EvidenceManager.SharedKernel.Evidence
{
    public interface IEvidenceFileServer
    {
        public Task<Guid> Save(EvidenceImage image);
        public Task<EvidenceImage> GetBy(Guid id);
    }
}
