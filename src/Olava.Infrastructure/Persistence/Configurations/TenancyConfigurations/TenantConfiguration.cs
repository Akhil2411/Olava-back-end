using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olava.Domain.Entities.Tenancy;

namespace Olava.Infrastructure.Persistence.Configurations.TenancyConfigurations;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> b)
    {
        b.Property(x => x.Name).HasMaxLength(200);
        b.Property(x => x.PlanCode).HasMaxLength(64);
    }
}
