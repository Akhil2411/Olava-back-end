using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olava.Domain.Entities.Templates;
using Olava.Domain.Entities.Weddings;

namespace Olava.Infrastructure.Persistence.Configurations.WeddingConfigurations;

internal sealed class WeddingConfiguration : IEntityTypeConfiguration<Wedding>
{
    public void Configure(EntityTypeBuilder<Wedding> b)
    {
        b.Property(x => x.Slug).HasMaxLength(63);          // max DNS label length
        b.Property(x => x.ShortCode).HasMaxLength(16);
        b.Property(x => x.Title).HasMaxLength(200);
        b.Property(x => x.PartnerOneName).HasMaxLength(100);
        b.Property(x => x.PartnerTwoName).HasMaxLength(100);
        b.Property(x => x.TimeZoneId).HasMaxLength(64);
        b.Property(x => x.PreferredLanguage).HasMaxLength(10);
        b.Property(x => x.ContactEmail).HasMaxLength(256);
        b.Property(x => x.ContactPhone).HasMaxLength(32);
        b.Property(x => x.ThemeJson).HasColumnType("jsonb");

        b.HasIndex(x => x.Slug).IsUnique();
        b.HasIndex(x => x.ShortCode).IsUnique();
        b.HasIndex(x => new { x.TenantId, x.Status });

        b.HasOne<TemplateVersion>().WithMany().HasForeignKey(x => x.TemplateVersionId).OnDelete(DeleteBehavior.Restrict);
    }
}
