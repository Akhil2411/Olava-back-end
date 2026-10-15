using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olava.Domain.Entities.Guests;
using Olava.Domain.Entities.Weddings;

namespace Olava.Infrastructure.Persistence.Configurations.OperationsConfigurations;

internal sealed class RsvpConfiguration : IEntityTypeConfiguration<Rsvp>
{
    public void Configure(EntityTypeBuilder<Rsvp> b)
    {
        b.Property(x => x.Message).HasMaxLength(1000);
        b.HasOne(x => x.Guest).WithOne(g => g.Rsvp).HasForeignKey<Rsvp>(x => x.GuestId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<Wedding>().WithMany().HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.WeddingId);
    }
}
