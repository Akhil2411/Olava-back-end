using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Olava.Application.Abstractions;
using Olava.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Olava.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Runs on every SaveChanges: stamps timestamps and enforces tenant rules on writes.
/// </summary>
internal sealed class AuditingSaveChangesInterceptor(ITenantProvider tenant, TimeProvider clock)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Apply(DbContext? context)
    {
        if (context is null) return;
        var now = clock.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is BaseEntity auditable)
            {
                if (entry.State == EntityState.Added) auditable.CreatedAt = now;
                else if (entry.State == EntityState.Modified) auditable.UpdatedAt = now;
            }

            if (entry.Entity is not ITenantOwned owned) continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    if (owned.TenantId == Guid.Empty)
                    {
                        owned.TenantId = tenant.TenantId
                            ?? throw new InvalidOperationException("No tenant in context; cannot create tenant-owned data.");
                    }
                    else if (tenant.TenantId is { } current && owned.TenantId != current)
                    {
                        throw new InvalidOperationException("Cross-tenant write blocked.");
                    }
                    break;

                case EntityState.Modified when entry.Property(nameof(ITenantOwned.TenantId)).IsModified:
                    throw new InvalidOperationException("TenantId cannot be changed.");
            }
        }
    }
}