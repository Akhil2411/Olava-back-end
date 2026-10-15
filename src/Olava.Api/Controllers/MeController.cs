using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Olava.Api.Security;
using Olava.Application.Auth;
using System.Security.Claims;

namespace Olava.Api.Controllers;

[Authorize(Policy = Policies.TenantMember)]
[Route("api/v1/me")]
public sealed class MeController : ApiControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        UserId = User.FindFirstValue(AppClaimTypes.Subject),
        Email = User.FindFirstValue(AppClaimTypes.Email),
        Name = User.FindFirstValue(AppClaimTypes.Name),
        TenantId = User.FindFirstValue(AppClaimTypes.TenantId),
        TenantRole = User.FindFirstValue(AppClaimTypes.TenantRole),
        PlatformRoles = User.FindAll(AppClaimTypes.Role).Select(c => c.Value)
    });
}