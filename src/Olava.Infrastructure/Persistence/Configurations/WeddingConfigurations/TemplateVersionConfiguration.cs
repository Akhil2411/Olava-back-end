using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olava.Domain.Entities.Templates;

namespace Olava.Infrastructure.Persistence.Configurations.WeddingConfigurations;

internal sealed class TemplateVersionConfiguration : IEntityTypeConfiguration<TemplateVersion>
{
    public void Configure(EntityTypeBuilder<TemplateVersion> b)
    {
        b.Property(x => x.DefinitionJson).HasColumnType("jsonb");
        b.HasOne(x => x.Template).WithMany(t => t.Versions).HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.TemplateId, x.VersionNumber }).IsUnique();
    }
}