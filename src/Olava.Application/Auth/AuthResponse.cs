namespace Olava.Application.Auth;

public sealed record AuthResponse(
    string AccessToken, DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken, DateTimeOffset RefreshTokenExpiresAt,
    UserSummary User);