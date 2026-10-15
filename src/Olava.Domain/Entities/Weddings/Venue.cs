using Olava.Domain.Common;

namespace Olava.Domain.Entities.Weddings;

public sealed class Venue : TenantEntity
{
    public Guid WeddingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? MapUrl { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
