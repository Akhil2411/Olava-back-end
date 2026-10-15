using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olava.Domain.Entities.Operations;
using Olava.Domain.Entities.Weddings;

namespace Olava.Infrastructure.Persistence.Configurations.OperationsConfigurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.Property(x => x.OrderNumber).HasMaxLength(32);
        b.Property(x => x.ProductCode).HasMaxLength(64);
        b.Property(x => x.CurrencyCode).HasMaxLength(3);
        b.Property(x => x.Provider).HasMaxLength(32);
        b.Property(x => x.ProviderReference).HasMaxLength(128);
        b.HasOne<Wedding>().WithMany().HasForeignKey(x => x.WeddingId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => x.OrderNumber).IsUnique();
    }
}