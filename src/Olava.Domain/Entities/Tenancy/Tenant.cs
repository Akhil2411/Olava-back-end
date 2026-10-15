using Olava.Domain.Common;
using Olava.Domain.Enums;

namespace Olava.Domain.Entities.Tenancy;

/// <summary>A workspace. Every host gets an Individual one; agencies get an Agency one later.</summary>
public sealed class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public TenantType Type { get; set; }
    public TenantStatus Status { get; set; } = TenantStatus.Active;
    public string? PlanCode { get; set; }
    public ICollection<TenantMember> Members { get; } = [];
}
