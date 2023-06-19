using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PoliceDepartment.EvidenceManager.SharedKernel.Case;
using System.Diagnostics.CodeAnalysis;
using PoliceDepartment.EvidenceManager.SharedKernel.Evidence;

namespace PoliceDepartment.EvidenceManager.Infra.Database.Mappings
{
    [ExcludeFromCodeCoverage]
    public class EvidenceMapping : IEntityTypeConfiguration<EvidenceEntity>
    {
        public void Configure(EntityTypeBuilder<EvidenceEntity> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).IsRequired();

            builder.Property(e => e.CreatedAt).IsRequired();

            builder.Property(e => e.UpdatedAt).IsRequired();

            builder.Property(e => e.Name).IsRequired();

            builder.Property(e => e.Description).IsRequired();

            builder.HasOne(e => e.Case)
                   .WithMany(c => c.Evidences)
                   .HasForeignKey(e => e.CaseId)
                   .OnDelete(DeleteBehavior.ClientSetNull);

            builder.ToTable("Evidences");
        }
    }
}
