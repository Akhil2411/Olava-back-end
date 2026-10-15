namespace Olava.Infrastructure.Identity;

internal sealed record AccessToken(string Token, DateTimeOffset ExpiresAt);

