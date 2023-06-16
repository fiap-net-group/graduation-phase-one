using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using PoliceDepartment.EvidenceManager.Domain.Officer;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Mappings
{
    [ExcludeFromCodeCoverage]
    public class OfficerMapping : IEntityTypeConfiguration<OfficerEntity>
    {
        public void Configure(EntityTypeBuilder<OfficerEntity> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id).IsRequired();

            builder.Property(o => o.Name).IsRequired();

            builder.ToTable("Officers");
        }
    }
}
