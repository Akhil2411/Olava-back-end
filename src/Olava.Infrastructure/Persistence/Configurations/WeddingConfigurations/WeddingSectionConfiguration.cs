using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olava.Domain.Entities.Weddings;

namespace Olava.Infrastructure.Persistence.Configurations.WeddingConfigurations;

internal sealed class WeddingSectionConfiguration : IEntityTypeConfiguration<WeddingSection>
{
    public void Configure(EntityTypeBuilder<WeddingSection> b)
    {
        b.Property(x => x.SectionKey).HasMaxLength(64);
        b.Property(x => x.LayoutVariant).HasMaxLength(64);
        b.Property(x => x.ContentJson).HasColumnType("jsonb");
        b.HasOne<Wedding>().WithMany().HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.WeddingId, x.SectionKey }).IsUnique();
    }
}