using Olava.Domain.Common;

namespace Olava.Domain.Entities.Operations;

/// <summary>Append-only security/audit trail. TenantId is nullable (platform-level events).</summary>
public sealed class AuditEvent : BaseEntity
{
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string MetadataJson { get; set; } = "{}";
    public string? IpAddress { get; set; }
}