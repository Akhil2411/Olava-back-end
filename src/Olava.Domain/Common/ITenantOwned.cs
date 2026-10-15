namespace Olava.Domain.Common;

/// <summary>Marker: this row belongs to exactly one tenant and is auto-filtered by TenantId.</summary>
public interface ITenantOwned
{
    Guid TenantId { get; set; }
}