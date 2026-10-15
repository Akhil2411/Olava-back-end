using Olava.Domain.Common;

namespace Olava.Infrastructure.Identity;

/// <summary>Only a SHA-256 hash is stored, so a DB leak can't be replayed.</summary>
public sealed class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }                 // tenant context this token was issued for
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public string? CreatedByIp { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? ReplacedByHash { get; set; }
    public string? RevokedReason { get; set; }
}
