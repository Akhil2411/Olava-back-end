using Olava.Application.Abstractions;
using Olava.Application.Auth;
using System.Security.Claims;

namespace Olava.Api.Security;


/// <summary>Reads identity + tenant from the validated JWT of the current HTTP request.</summary>
internal sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser, ITenantProvider
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public Guid? UserId => ParseGuid(AppClaimTypes.Subject);
    public Guid? TenantId => ParseGuid(AppClaimTypes.TenantId);

    private Guid? ParseGuid(string claimType) =>
        Guid.TryParse(Principal?.FindFirstValue(claimType), out var id) ? id : null;
}
