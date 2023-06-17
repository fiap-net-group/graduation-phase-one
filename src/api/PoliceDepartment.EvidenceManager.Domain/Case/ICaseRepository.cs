namespace PoliceDepartment.EvidenceManager.SharedKernel.Case
{
    public interface ICaseRepository : IDisposable
    {
        Task UpdateAsync(CaseEntity entity, CancellationToken cancellationToken);
        Task<IEnumerable<CaseEntity>> GetByOfficerId(Guid officerId, CancellationToken cancellationToken);
        Task<CaseEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task DeleteAsync(CaseEntity entity, CancellationToken cancellationToken);
        Task AddAsync(CaseEntity entity, CancellationToken cancellationToken);
    }
}
