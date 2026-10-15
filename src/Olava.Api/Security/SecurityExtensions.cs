using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Olava.Application.Auth;
using Olava.Domain.Enums;
using Olava.Infrastructure.Options;
using System.Text;
using System.Threading.RateLimiting;

namespace Olava.Api.Security;

public static class Policies
{
    public const string TenantMember = nameof(TenantMember);
    public const string TenantAdmin = nameof(TenantAdmin);
    public const string PlatformStaff = nameof(PlatformStaff);
}

internal static class SecurityExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        // Configure the bearer options from our validated JwtOptions (single source of truth).
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;
                bearer.MapInboundClaims = false; // keep claim names exactly as issued ("sub", "role"...)
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = AppClaimTypes.Name,
                    RoleClaimType = AppClaimTypes.Role
                };
            });

        return services;
    }

    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            // Secure by default: every endpoint requires login unless it says .AllowAnonymous()
            .AddPolicy(Policies.TenantMember, p => p
                .RequireAuthenticatedUser()
                .RequireClaim(AppClaimTypes.TenantId))
            .AddPolicy(Policies.TenantAdmin, p => p
                .RequireAuthenticatedUser()
                .RequireClaim(AppClaimTypes.TenantRole, nameof(TenantRole.Admin), nameof(TenantRole.Owner)))
            .AddPolicy(Policies.PlatformStaff, p => p
                .RequireRole(AppRoles.PlatformAdmin, AppRoles.Support));

        return services;
    }

    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration config)
    {
        var origins = config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(o => o.AddDefaultPolicy(p => p
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()));
        return services;
    }

    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(o =>
        {
            o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            o.AddPolicy("auth", ctx => RateLimitPartition.GetFixedWindowLimiter(
                ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 20,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));
        });
        return services;
    }
}