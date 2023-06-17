namespace PoliceDepartment.EvidenceManager.SharedKernel.Database
{
    public interface IDatabaseContext : IDisposable
    {
        Task<bool> AnyPendingMigrationsAsync();
        Task MigrateAsync();
        Task TestConnectionAsync();
    }
}
