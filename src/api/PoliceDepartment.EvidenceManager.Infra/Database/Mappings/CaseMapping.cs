using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PoliceDepartment.EvidenceManager.Domain.Case;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Mappings
{
    [ExcludeFromCodeCoverage]
    public class CaseMapping : IEntityTypeConfiguration<CaseEntity>
    {
        public void Configure(EntityTypeBuilder<CaseEntity> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).IsRequired();

            builder.Property(c => c.CreatedAt).IsRequired();

            builder.Property(c => c.UpdatedAt).IsRequired();

            builder.Property(c => c.Name).IsRequired();

            builder.Property(c => c.Description).IsRequired();

            builder.HasOne(c => c.Officer)
                   .WithMany(o => o.Cases)
                   .HasForeignKey(c => c.OfficerId)
                   .OnDelete(DeleteBehavior.ClientSetNull);

            builder.ToTable("Cases");
        }
    }
}
