using System.ComponentModel.DataAnnotations;

namespace Olava.Application.Auth;

public sealed record ConfirmEmailRequest
{
    [Required] public Guid UserId { get; init; }
    [Required, StringLength(2048)] public string Token { get; init; } = string.Empty;
}