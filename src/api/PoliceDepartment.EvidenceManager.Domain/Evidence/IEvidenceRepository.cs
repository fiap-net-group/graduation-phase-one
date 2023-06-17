namespace PoliceDepartment.EvidenceManager.SharedKernel.Evidence
{
    public interface IEvidenceRepository : IDisposable
    {
        Task<EvidenceEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task CreateAsync(EvidenceEntity evidence, CancellationToken cancellationToken);
        Task<IEnumerable<EvidenceEntity>> GetPaginatedAsync(int page, int rows);
        Task<IEnumerable<EvidenceEntity>> GetByCaseIdAsync(Guid caseId, CancellationToken cancellationToken);
        Task DeleteByIdAsync(Guid id);
        Task DeleteByCaseAsync(Guid caseId, CancellationToken cancellationToken);
        Task UpdateAsync(EvidenceEntity evidence);
    }
}
