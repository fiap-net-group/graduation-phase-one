namespace PoliceDepartment.EvidenceManager.Domain.Officer
{
    public interface IOfficerRepository : IDisposable
    {
        public Task CreateAsync(OfficerEntity officer, CancellationToken cancellationToken);
        Task<OfficerEntity> GetByEmailAsync(string username, CancellationToken cancellationToken);
    }
}
