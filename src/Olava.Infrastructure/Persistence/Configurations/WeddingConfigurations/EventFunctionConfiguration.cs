using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olava.Domain.Entities.Operations;
using Olava.Domain.Entities.Weddings;

namespace Olava.Infrastructure.Persistence.Configurations.WeddingConfigurations;

internal sealed class EventFunctionConfiguration : IEntityTypeConfiguration<EventFunction>
{
    public void Configure(EntityTypeBuilder<EventFunction> b)
    {
        b.Property(x => x.Name).HasMaxLength(200);
        b.Property(x => x.EventType).HasMaxLength(64);
        b.Property(x => x.Description).HasMaxLength(2000);
        b.Property(x => x.DressCode).HasMaxLength(200);
        b.Property(x => x.Notes).HasMaxLength(2000);

        b.HasOne<Wedding>().WithMany().HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<Venue>().WithMany().HasForeignKey(x => x.VenueId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne<MediaAsset>().WithMany().HasForeignKey(x => x.CoverMediaAssetId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => new { x.WeddingId, x.SortOrder });
    }
}
