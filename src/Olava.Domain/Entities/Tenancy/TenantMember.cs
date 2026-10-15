using Olava.Domain.Common;
using Olava.Domain.Enums;

namespace Olava.Domain.Entities.Tenancy;

/// <summary>Links a user to a tenant with a role. A user can belong to many tenants.</summary>
public sealed class TenantMember : TenantEntity
{
    public Guid UserId { get; set; }
    public TenantRole Role { get; set; }
    public Tenant? Tenant { get; set; }
}