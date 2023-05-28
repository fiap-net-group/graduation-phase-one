namespace PoliceDepartment.EvidenceManager.Domain.Evidence
{
    public interface IEvidenceRepository : IDisposable
    {
        Task CreateAsync(EvidenceEntity evidence);
        Task<EvidenceEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<EvidenceEntity>> GetPaginatedAsync(int page, int rows);
        Task DeleteByIdAsync(Guid id);
        Task DeleteByCaseAsync(Guid caseId, CancellationToken cancellationToken);
        Task UpdateAsync(EvidenceEntity evidence);
    }
}
