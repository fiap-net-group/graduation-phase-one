namespace PoliceDepartment.EvidenceManager.Domain.Case
{
    public interface ICaseRepository : IDisposable
    {
        Task<IEnumerable<CaseEntity>> GetByOfficerId(Guid officerId, CancellationToken cancellationToken);
        Task<CaseEntity> GetId(Guid id, CancellationToken cancellationToken);
    }
}
