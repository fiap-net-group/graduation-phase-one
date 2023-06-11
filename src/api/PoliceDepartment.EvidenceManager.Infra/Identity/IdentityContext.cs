using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PoliceDepartment.EvidenceManager.Infra.Identity
{
    public class IdentityContext : IdentityDbContext<IdentityUser>, IIdentityContext
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {

        }

        public async Task<bool> AnyPendingMigrationsAsync()
        {
            try
            {
                var migrations = await Database.GetPendingMigrationsAsync();

                return migrations.Any();
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task MigrateAsync()
        {
            await Database.MigrateAsync();
        }

        public async Task TestConnectionAsync()
        {
            _ = await Database.ExecuteSqlRawAsync("SELECT 1");
        }
    }
}
