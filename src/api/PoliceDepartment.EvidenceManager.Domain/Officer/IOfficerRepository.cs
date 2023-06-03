namespace PoliceDepartment.EvidenceManager.Domain.Officer
{
    public interface IOfficerRepository : IDisposable
    {
        public Task CreateAsync(OfficerEntity officer, CancellationToken cancellationToken);
    }
}
