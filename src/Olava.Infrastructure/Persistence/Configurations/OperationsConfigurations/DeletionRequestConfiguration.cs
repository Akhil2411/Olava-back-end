using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olava.Domain.Entities.Operations;

namespace Olava.Infrastructure.Persistence.Configurations.OperationsConfigurations;

internal sealed class DeletionRequestConfiguration : IEntityTypeConfiguration<DeletionRequest>
{
    public void Configure(EntityTypeBuilder<DeletionRequest> b)
    {
        b.Property(x => x.Notes).HasMaxLength(1000);
        b.HasIndex(x => new { x.Status, x.ScheduledFor });
    }
}
