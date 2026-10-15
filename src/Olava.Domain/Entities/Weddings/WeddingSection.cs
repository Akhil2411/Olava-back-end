using Olava.Domain.Common;

namespace Olava.Domain.Entities.Weddings;

/// <summary>Per-wedding section config: enabled, order, content (FR-TPL-02/03/04).</summary>
public sealed class WeddingSection : TenantEntity
{
    public Guid WeddingId { get; set; }
    public string SectionKey { get; set; } = string.Empty;  // hero, story, events, gallery, rsvp, faq...
    public bool IsEnabled { get; set; } = true;
    public int SortOrder { get; set; }
    public string? LayoutVariant { get; set; }
    public string ContentJson { get; set; } = "{}";
}
