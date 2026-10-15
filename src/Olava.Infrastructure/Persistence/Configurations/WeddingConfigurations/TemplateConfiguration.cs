using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olava.Domain.Entities.Templates;

namespace Olava.Infrastructure.Persistence.Configurations.WeddingConfigurations;

internal sealed class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> b)
    {
        b.Property(x => x.Key).HasMaxLength(64);
        b.Property(x => x.Name).HasMaxLength(200);
        b.Property(x => x.Description).HasMaxLength(1000);
        b.Property(x => x.Category).HasMaxLength(64);
        b.Property(x => x.PreviewImageUrl).HasMaxLength(2048);
        b.HasIndex(x => x.Key).IsUnique();
    }
}