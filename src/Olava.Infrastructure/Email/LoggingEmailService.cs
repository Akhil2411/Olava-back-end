using Microsoft.Extensions.Logging;
using Olava.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Olava.Infrastructure.Email;

/// <summary>DEV ONLY: prints emails to the console. Swap for SES/SendGrid/Resend in production.</summary>
internal sealed class LoggingEmailService(ILogger<LoggingEmailService> logger) : IEmailService
{
    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        logger.LogInformation("DEV EMAIL → {To} | {Subject}{NewLine}{Body}", to, subject, Environment.NewLine, htmlBody);
        return Task.CompletedTask;
    }
}