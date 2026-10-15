using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Olava.Infrastructure.Persistence.Configurations.TenancyConfigurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> b) => b.Property(x => x.FullName).HasMaxLength(120);
}