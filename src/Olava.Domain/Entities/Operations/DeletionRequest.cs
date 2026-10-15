using Olava.Domain.Common;
using Olava.Domain.Enums;

namespace Olava.Domain.Entities.Operations;

/// <summary>ACC-06: host asks to delete event + personal data.</summary>
public sealed class DeletionRequest : TenantEntity
{
    public Guid? WeddingId { get; set; }          // no FK on purpose: the wedding will be deleted
    public Guid RequestedByUserId { get; set; }
    public DeletionRequestStatus Status { get; set; } = DeletionRequestStatus.Requested;
    public DateTimeOffset ScheduledFor { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string? Notes { get; set; }
}