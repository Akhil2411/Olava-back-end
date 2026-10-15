using System.ComponentModel.DataAnnotations;

namespace Olava.Application.Auth;

public sealed record RegisterRequest
{
    [Required, EmailAddress, StringLength(256)] public string Email { get; init; } = string.Empty;
    [Required, StringLength(128, MinimumLength = 10)] public string Password { get; init; } = string.Empty;
    [Required, StringLength(120, MinimumLength = 2)] public string FullName { get; init; } = string.Empty;
    [StringLength(120)] public string? WorkspaceName { get; init; }
}