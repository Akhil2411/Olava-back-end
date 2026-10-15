namespace Olava.Domain.Common;

/// <summary>
/// Tenant entity
/// </summary>
public abstract class TenantEntity : BaseEntity, ITenantOwned
{
    public Guid TenantId { get; set; }
}