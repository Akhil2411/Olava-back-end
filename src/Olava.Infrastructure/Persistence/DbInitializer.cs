using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Olava.Application.Auth;
using Olava.Infrastructure.Identity;

namespace Olava.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitialiseDatabaseAsync(this IServiceProvider services, bool applyMigrations)
    {
        using var scope = services.CreateScope();

        if (applyMigrations)
            await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();

        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        foreach (var name in AppRoles.All)
        {
            if (!await roles.RoleExistsAsync(name))
                await roles.CreateAsync(new ApplicationRole(name));
        }
    }
}