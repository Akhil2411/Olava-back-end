using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olava.Domain.Entities.Guests;
using Olava.Domain.Entities.Weddings;

namespace Olava.Infrastructure.Persistence.Configurations.OperationsConfigurations;

internal sealed class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> b)
    {
        b.Property(x => x.FullName).HasMaxLength(200);
        b.Property(x => x.Email).HasMaxLength(256);
        b.Property(x => x.Phone).HasMaxLength(32);
        b.Property(x => x.GroupName).HasMaxLength(100);
        b.Property(x => x.InviteToken).HasMaxLength(64);
        b.Property(x => x.Notes).HasMaxLength(1000);

        b.HasOne<Wedding>().WithMany().HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.WeddingId);
        b.HasIndex(x => x.InviteToken).IsUnique();   // Postgres allows many NULLs in a unique index
    }
}