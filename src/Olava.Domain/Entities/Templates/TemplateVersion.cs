using Olava.Domain.Common;

namespace Olava.Domain.Entities.Templates;

/// <summary>Immutable once published. Defines sections, tokens, slots (FR-TPL-09).</summary>
public sealed class TemplateVersion : BaseEntity
{
    public Guid TemplateId { get; set; }
    public int VersionNumber { get; set; }
    public string DefinitionJson { get; set; } = "{}";
    public bool IsPublished { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public Template? Template { get; set; }
}