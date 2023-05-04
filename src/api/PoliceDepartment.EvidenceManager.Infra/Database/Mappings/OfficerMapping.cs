using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using PoliceDepartment.EvidenceManager.Domain.Officer;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Mappings
{
    [ExcludeFromCodeCoverage]
    public class OfficerMapping : IEntityTypeConfiguration<OfficerEntity>
    {
        private object p;

        public void Configure(EntityTypeBuilder<OfficerEntity> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id).IsRequired();

            builder.HasMany(o => o.Evidences)
                   .WithOne(e => e.Officer)
                   .HasForeignKey(e => e.OfficerId);

            builder.HasMany(o => o.Cases)
                   .WithOne(c => c.Officer)
                   .HasForeignKey(c => c.OfficerId);

            builder.ToTable("Officers");
        }
    }
}
