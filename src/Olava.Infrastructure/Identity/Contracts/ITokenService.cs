using Olava.Domain.Enums;

namespace Olava.Infrastructure.Identity.Contracts;

internal interface ITokenService
{
    AccessToken CreateAccessToken(ApplicationUser user, IEnumerable<string> roles, Guid tenantId, TenantRole tenantRole);
    (string Raw, string Hash) GenerateRefreshToken();
    string Hash(string rawToken);
}