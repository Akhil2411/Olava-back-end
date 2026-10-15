using Olava.Domain.Common;

namespace Olava.Domain.Entities.Weddings;

/// <summary>A function: mehendi, haldi, nikah, reception... (EVT-01/02)</summary>
public sealed class EventFunction : TenantEntity
{
    public Guid WeddingId { get; set; }
    public Guid? VenueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? EventType { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
    public string? DressCode { get; set; }
    public string? Notes { get; set; }
    public Guid? CoverMediaAssetId { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}