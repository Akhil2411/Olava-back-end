using System;
using System.Collections.Generic;
using System.Text;

namespace Olava.Application.Auth;

public sealed record RegisterResponse(Guid UserId, string Email, bool EmailConfirmationRequired);