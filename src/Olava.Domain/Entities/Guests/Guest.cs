using Olava.Domain.Common;

namespace Olava.Domain.Entities.Guests;

public sealed class Guest : TenantEntity
{
    public Guid WeddingId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }          // private data: never expose publicly (GST-07)
    public string? Phone { get; set; }
    public string? GroupName { get; set; }
    public int MaxPlusOnes { get; set; }
    public string? InviteToken { get; set; }    // opt-in unique link (GST-06)
    public string? Notes { get; set; }
    public Rsvp? Rsvp { get; set; }
}
