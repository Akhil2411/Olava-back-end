using Olava.Domain.Common;

namespace Olava.Domain.Entities.Operations;

/// <summary>What a tenant/wedding is allowed to use (templates, AI regenerations, renders...).</summary>
public sealed class Entitlement : TenantEntity
{
    public Guid? WeddingId { get; set; }
    public Guid? OrderId { get; set; }
    public string Code { get; set; } = string.Empty;
    public int? QuotaLimit { get; set; }
    public int QuotaUsed { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}