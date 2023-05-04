using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PoliceDepartment.EvidenceManager.Domain.Officer;

namespace PoliceDepartment.EvidenceManager.Infra.Identity
{
    public class IdentityContext : IdentityDbContext<OfficerEntity>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }
    }
}
