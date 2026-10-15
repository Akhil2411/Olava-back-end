using System;
using System.Collections.Generic;
using System.Text;

namespace Olava.Application.Abstractions;

/// <summary>Which tenant is this request acting in? Drives the global query filter.</summary>
public interface ITenantProvider
{
    Guid? TenantId { get; }
}