using System.ComponentModel.DataAnnotations;

namespace Olava.Application.Auth;

public sealed record RefreshRequest
{
    [Required, StringLength(512)] public string RefreshToken { get; init; } = string.Empty;
}