using System;
using System.Collections.Generic;
using System.Text;

namespace Olava.Application.Abstractions;

/// <summary>Who is calling? Resolved from the JWT per request.</summary>
public interface ICurrentUser
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
}