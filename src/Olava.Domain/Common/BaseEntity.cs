namespace Olava.Domain.Common;

/// <summary>
/// Base entity to be used for all other entities
/// </summary>
public abstract class BaseEntity
{
    // UUIDv7: time-ordered GUIDs → far better Postgres index locality than random GUIDs
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}


