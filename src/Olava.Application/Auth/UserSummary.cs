namespace Olava.Application.Auth;

public sealed record UserSummary(
    Guid Id, string Email, string FullName,
    Guid TenantId, string TenantRole, IReadOnlyList<string> PlatformRoles);