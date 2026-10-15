using System.ComponentModel.DataAnnotations;

namespace Olava.Application.Auth;

public sealed record ForgotPasswordRequest
{
    [Required, EmailAddress, StringLength(256)] public string Email { get; init; } = string.Empty;
}