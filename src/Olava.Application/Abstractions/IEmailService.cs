using System;
using System.Collections.Generic;
using System.Text;

namespace Olava.Application.Abstractions;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
}
