using System.ComponentModel.DataAnnotations;

namespace Olava.Application.Auth;

public sealed record ResetPasswordRequest
{
    [Required, EmailAddress, StringLength(256)] public string Email { get; init; } = string.Empty;
    [Required, StringLength(2048)] public string Token { get; init; } = string.Empty;
    [Required, StringLength(128, MinimumLength = 10)] public string NewPassword { get; init; } = string.Empty;
}