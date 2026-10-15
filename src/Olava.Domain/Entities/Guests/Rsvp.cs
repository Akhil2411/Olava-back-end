using Olava.Domain.Common;
using Olava.Domain.Enums;

namespace Olava.Domain.Entities.Guests;

public sealed class Rsvp : TenantEntity
{
    public Guid WeddingId { get; set; }
    public Guid GuestId { get; set; }
    public RsvpStatus Status { get; set; }
    public int PlusOneCount { get; set; }
    public string? Message { get; set; }
    public DateTimeOffset RespondedAt { get; set; }
    public Guest? Guest { get; set; }
}