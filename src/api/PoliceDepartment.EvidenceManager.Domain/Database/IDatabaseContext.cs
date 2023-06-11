namespace PoliceDepartment.EvidenceManager.Domain.Database
{
    public interface IDatabaseContext : IDisposable
    {
        Task<bool> AnyPendingMigrationsAsync();
        Task MigrateAsync();
        Task TestConnectionAsync();
    }
}
