using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Olava.Application.Abstractions;
using Olava.Application.Auth.Contracts;
using Olava.Infrastructure.Email;
using Olava.Infrastructure.Identity;
using Olava.Infrastructure.Identity.Contracts;
using Olava.Infrastructure.Identity.Services;
using Olava.Infrastructure.Options;
using Olava.Infrastructure.Persistence;
using Olava.Infrastructure.Persistence.Interceptors;

namespace Olava.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:Default is not configured.");

        services.AddSingleton(TimeProvider.System);

        // Fail fast at startup if config is missing/invalid (e.g. no JWT signing key).
        services.AddOptions<JwtOptions>().Bind(config.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<AppOptions>().Bind(config.GetSection(AppOptions.SectionName))
            .ValidateDataAnnotations().ValidateOnStart();

        services.AddScoped<AuditingSaveChangesInterceptor>();
        services.AddDbContext<AppDbContext>((sp, options) => options
            .UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure())
            .AddInterceptors(sp.GetRequiredService<AuditingSaveChangesInterceptor>()));

        services.AddIdentityCore<ApplicationUser>(o =>
        {
            o.User.RequireUniqueEmail = true;

            // NIST-style: length over complexity rules.
            o.Password.RequiredLength = 10;
            o.Password.RequireDigit = false;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;

            o.Lockout.AllowedForNewUsers = true;
            o.Lockout.MaxFailedAccessAttempts = 5;
            o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            o.SignIn.RequireConfirmedEmail = config.GetValue<bool>("Identity:RequireConfirmedEmail");
        })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        // Reset / confirmation links expire after 2 hours.
        services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(2));

        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IEmailService, LoggingEmailService>();

        return services;
    }
}