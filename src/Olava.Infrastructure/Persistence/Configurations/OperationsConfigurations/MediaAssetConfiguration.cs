using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olava.Domain.Entities.Operations;
using Olava.Domain.Entities.Weddings;

namespace Olava.Infrastructure.Persistence.Configurations.OperationsConfigurations;

internal sealed class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> b)
    {
        b.Property(x => x.StorageKey).HasMaxLength(500);
        b.Property(x => x.ContentType).HasMaxLength(100);
        b.Property(x => x.Caption).HasMaxLength(500);
        b.HasOne<Wedding>().WithMany().HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.WeddingId, x.Purpose, x.SortOrder });
    }
}
