using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Olava.Application.Abstractions;
using Olava.Domain.Common;
using Olava.Domain.Entities.Guests;
using Olava.Domain.Entities.Operations;
using Olava.Domain.Entities.Templates;
using Olava.Domain.Entities.Tenancy;
using Olava.Domain.Entities.Weddings;
using Olava.Domain.Enums;
using Olava.Infrastructure.Identity;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Olava.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public const string TenantFilter = "Tenant";

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantMember> TenantMembers => Set<TenantMember>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Wedding> Weddings => Set<Wedding>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<EventFunction> EventFunctions => Set<EventFunction>();
    public DbSet<WeddingSection> WeddingSections => Set<WeddingSection>();

    public DbSet<Template> Templates => Set<Template>();
    public DbSet<TemplateVersion> TemplateVersions => Set<TemplateVersion>();

    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Rsvp> Rsvps => Set<Rsvp>();

    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Entitlement> Entitlements => Set<Entitlement>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<DeletionRequest> DeletionRequests => Set<DeletionRequest>();

    // Evaluated per query, per DbContext instance (i.e. per request).
    private Guid? CurrentTenantId => tenantProvider.TenantId;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // maps the Identity tables

        builder.Entity<ApplicationUser>().ToTable("users");
        builder.Entity<ApplicationRole>().ToTable("roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        ApplyTenantRules(builder);
        ApplySnakeCaseNaming(builder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder b)
    {
        // Store enums as readable strings instead of magic integers.
        b.Properties<TenantType>().HaveConversion<string>().HaveMaxLength(32);
        b.Properties<TenantStatus>().HaveConversion<string>().HaveMaxLength(32);
        b.Properties<TenantRole>().HaveConversion<string>().HaveMaxLength(32);
        b.Properties<WeddingStatus>().HaveConversion<string>().HaveMaxLength(32);
        b.Properties<TemplateStatus>().HaveConversion<string>().HaveMaxLength(32);
        b.Properties<RsvpStatus>().HaveConversion<string>().HaveMaxLength(32);
        b.Properties<MediaPurpose>().HaveConversion<string>().HaveMaxLength(32);
        b.Properties<OrderStatus>().HaveConversion<string>().HaveMaxLength(32);
        b.Properties<DeletionRequestStatus>().HaveConversion<string>().HaveMaxLength(32);
    }

    // ---------- Multi-tenancy: global filter + FK + index for every ITenantOwned entity ----------
    private static readonly MethodInfo ConfigureTenantOwnedMethod =
        typeof(AppDbContext).GetMethod(nameof(ConfigureTenantOwned), BindingFlags.Instance | BindingFlags.NonPublic)!;

    private void ApplyTenantRules(ModelBuilder builder)
    {
        var tenantOwnedTypes = builder.Model.GetEntityTypes()
            .Where(t => typeof(ITenantOwned).IsAssignableFrom(t.ClrType))
            .Select(t => t.ClrType)
            .ToList();

        foreach (var clrType in tenantOwnedTypes)
            ConfigureTenantOwnedMethod.MakeGenericMethod(clrType).Invoke(this, [builder]);
    }

    private void ConfigureTenantOwned<TEntity>(ModelBuilder builder) where TEntity : class, ITenantOwned
    {
        var entity = builder.Entity<TEntity>();

        // EF Core 10 "named" filter: fail-closed (no tenant in context → no rows).
        entity.HasQueryFilter(TenantFilter, e => e.TenantId == CurrentTenantId);

        entity.HasIndex(e => e.TenantId);

        if (typeof(TEntity) != typeof(TenantMember)) // TenantMember↔Tenant is configured explicitly
            entity.HasOne<Tenant>().WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
    }

    // ---------- Postgres-friendly snake_case without extra packages ----------
    private static void ApplySnakeCaseNaming(ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName is null) continue;

            entity.SetTableName(ToSnake(tableName));
            var table = StoreObjectIdentifier.Table(entity.GetTableName()!, entity.GetSchema());

            foreach (var property in entity.GetProperties())
                property.SetColumnName(ToSnake(property.GetColumnName(table) ?? property.Name));

            foreach (var key in entity.GetKeys())
                key.SetName(ToSnake(key.GetName()!));

            foreach (var fk in entity.GetForeignKeys())
                fk.SetConstraintName(ToSnake(fk.GetConstraintName()!));

            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(ToSnake(index.GetDatabaseName()!));
        }
    }

    private static string ToSnake(string name) =>
        Regex.Replace(name, @"([a-z0-9])([A-Z])", "$1_$2").ToLowerInvariant();
}