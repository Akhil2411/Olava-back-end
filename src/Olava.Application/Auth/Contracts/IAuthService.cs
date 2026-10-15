using Olava.Application.Common;

namespace Olava.Application.Auth.Contracts;

public interface IAuthService
{
    Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken ct);
    Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request, string? ipAddress, CancellationToken ct);
    Task<Result> LogoutAsync(RefreshRequest request, CancellationToken ct);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken ct);
    Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct);
}