namespace PoliceDepartment.EvidenceManager.Domain.Evidence
{
    public interface IEvidenceRepository : IDisposable
    {
        Task Create(EvidenceEntity evidence);
        Task<EvidenceEntity> GetBy(Guid id);
        Task<IEnumerable<EvidenceEntity>> GetPaginated(int page, int rows);
        Task DeleteBy(Guid id);
        Task Update(EvidenceEntity evidence);
    }
}
