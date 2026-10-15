using System.Buffers.Text;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Olava.Application.Abstractions;
using Olava.Application.Auth;
using Olava.Application.Auth.Contracts;
using Olava.Application.Common;
using Olava.Domain.Entities.Tenancy;
using Olava.Domain.Enums;
using Olava.Infrastructure.Identity.Contracts;
using Olava.Infrastructure.Options;
using Olava.Infrastructure.Persistence;

namespace Olava.Infrastructure.Identity.Services;

internal sealed class AuthService(
    UserManager<ApplicationUser> users,
    SignInManager<ApplicationUser> signIn,
    AppDbContext db,
    ITokenService tokens,
    IEmailService email,
    IOptions<AppOptions> appOptions,
    IOptions<JwtOptions> jwtOptions,
    IOptions<IdentityOptions> identityOptions,
    TimeProvider clock) : IAuthService
{
    private static readonly Error InvalidCredentials =
        Error.Unauthorized("auth.invalid_credentials", "Email or password is incorrect.");
    private static readonly Error InvalidRefresh =
        Error.Unauthorized("auth.invalid_refresh_token", "Refresh token is invalid or expired.");
    private static readonly Error InvalidLink =
        Error.Validation("auth.invalid_link", "This link is invalid or has expired.");

    // ───────────────────────── Register ─────────────────────────
    public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var address = request.Email.Trim();
        if (await users.FindByEmailAsync(address) is not null)
            return Error.Conflict("auth.email_taken", "An account with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = address,
            Email = address,
            FullName = request.FullName.Trim(),
            CreatedAt = clock.GetUtcNow()
        };
        var tenant = new Tenant
        {
            Name = string.IsNullOrWhiteSpace(request.WorkspaceName)
                ? $"{user.FullName}'s workspace"
                : request.WorkspaceName.Trim(),
            Type = TenantType.Individual
        };

        // User + Tenant + Membership must succeed or fail TOGETHER → one transaction.
        var strategy = db.Database.CreateExecutionStrategy();
        var identityResult = await strategy.ExecuteAsync(async () =>
        {
            db.ChangeTracker.Clear(); // safe if the retry strategy re-runs this block
            await using var tx = await db.Database.BeginTransactionAsync(ct);

            var result = await users.CreateAsync(user, request.Password);
            if (!result.Succeeded) return result;

            db.Tenants.Add(tenant);
            db.TenantMembers.Add(new TenantMember { TenantId = tenant.Id, UserId = user.Id, Role = TenantRole.Owner });
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return result;
        });

        if (!identityResult.Succeeded)
            return Error.Validation("auth.registration_failed", Describe(identityResult));

        await SendConfirmationEmailAsync(user, ct);
        return new RegisterResponse(user.Id, address, identityOptions.Value.SignIn.RequireConfirmedEmail);
    }

    // ───────────────────────── Login ─────────────────────────
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken ct)
    {
        var user = await users.FindByEmailAsync(request.Email.Trim());
        if (user is null || !user.IsActive) return InvalidCredentials;

        // Checks password + lockout + (optionally) confirmed email; counts failures.
        var result = await signIn.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (result.IsLockedOut)
            return Error.Forbidden("auth.locked_out", "Too many failed attempts. Try again later.");
        if (result.IsNotAllowed)
            return Error.Forbidden("auth.email_not_confirmed", "Please confirm your email first.");
        if (!result.Succeeded) return InvalidCredentials;

        user.LastLoginAt = clock.GetUtcNow();
        await users.UpdateAsync(user);

        return await IssueTokensAsync(user, preferredTenantId: null, ipAddress, rotating: null, ct);
    }

    // ───────────────────────── Refresh (rotation + reuse detection) ─────────────────────────
    public async Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request, string? ipAddress, CancellationToken ct)
    {
        var hash = tokens.Hash(request.RefreshToken);
        var stored = await db.RefreshTokens.SingleOrDefaultAsync(t => t.TokenHash == hash, ct);
        if (stored is null) return InvalidRefresh;

        if (stored.RevokedAt is not null)
        {
            // An already-used token came back → likely stolen. Kill every session for this user.
            await RevokeAllAsync(stored.UserId, "reuse_detected", ct);
            return InvalidRefresh;
        }

        if (stored.ExpiresAt <= clock.GetUtcNow()) return InvalidRefresh;

        var user = await users.FindByIdAsync(stored.UserId.ToString());
        if (user is null || !user.IsActive || await users.IsLockedOutAsync(user)) return InvalidRefresh;

        return await IssueTokensAsync(user, stored.TenantId, ipAddress, rotating: stored, ct);
    }

    // ───────────────────────── Logout ─────────────────────────
    public async Task<Result> LogoutAsync(RefreshRequest request, CancellationToken ct)
    {
        var hash = tokens.Hash(request.RefreshToken);
        var now = (DateTimeOffset?)clock.GetUtcNow();
        await db.RefreshTokens
            .Where(t => t.TokenHash == hash && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.RevokedAt, now)
                .SetProperty(t => t.RevokedReason, "logout"), ct);
        return Result.Success();
    }

    // ───────────────────────── Email confirmation ─────────────────────────
    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(request.UserId.ToString());
        if (user is null || !TryDecodeToken(request.Token, out var token)) return InvalidLink;

        var result = await users.ConfirmEmailAsync(user, token);
        return result.Succeeded ? Result.Success() : InvalidLink;
    }

    // ───────────────────────── Forgot / reset password ─────────────────────────
    public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct)
    {
        var user = await users.FindByEmailAsync(request.Email.Trim());

        // Always succeed: never reveal whether an email is registered.
        if (user is null || !user.IsActive) return Result.Success();

        var token = await users.GeneratePasswordResetTokenAsync(user);
        var link = BuildLink("reset-password", new Dictionary<string, string?>
        {
            ["email"] = user.Email,
            ["token"] = EncodeToken(token)
        });

        await email.SendAsync(user.Email!, "Reset your password",
            $"<p>Hi {WebUtility.HtmlEncode(user.FullName)},</p>" +
            $"<p><a href=\"{WebUtility.HtmlEncode(link)}\">Reset your password</a></p>" +
            "<p>If you didn't request this, you can ignore this email.</p>", ct);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct)
    {
        var user = await users.FindByEmailAsync(request.Email.Trim());
        if (user is null || !TryDecodeToken(request.Token, out var token)) return InvalidLink;

        var result = await users.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded) return Error.Validation("auth.reset_failed", Describe(result));

        await RevokeAllAsync(user.Id, "password_reset", ct); // sign out everywhere
        return Result.Success();
    }

    // ───────────────────────── Helpers ─────────────────────────
    private async Task<Result<AuthResponse>> IssueTokensAsync(
        ApplicationUser user, Guid? preferredTenantId, string? ip, RefreshToken? rotating, CancellationToken ct)
    {
        // Membership lookup happens BEFORE a tenant is known → bypass only the tenant filter.
        var membership = await db.TenantMembers
            .IgnoreQueryFilters([AppDbContext.TenantFilter])
            .AsNoTracking()
            .Where(m => m.UserId == user.Id && (preferredTenantId == null || m.TenantId == preferredTenantId))
            .OrderBy(m => m.CreatedAt)
            .Select(m => new { m.TenantId, m.Role, TenantStatus = m.Tenant!.Status })
            .FirstOrDefaultAsync(ct);

        if (membership is null)
            return Error.Forbidden("auth.no_workspace", "This account has no active workspace.");
        if (membership.TenantStatus != TenantStatus.Active)
            return Error.Forbidden("auth.workspace_suspended", "This workspace is suspended.");

        var roles = await users.GetRolesAsync(user);
        var access = tokens.CreateAccessToken(user, roles, membership.TenantId, membership.Role);

        var (rawRefresh, refreshHash) = tokens.GenerateRefreshToken();
        var now = clock.GetUtcNow();
        var refreshExpires = now.AddDays(jwtOptions.Value.RefreshTokenDays);

        if (rotating is not null)
        {
            rotating.RevokedAt = now;
            rotating.ReplacedByHash = refreshHash;
            rotating.RevokedReason = "rotated";
        }

        db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TenantId = membership.TenantId,
            TokenHash = refreshHash,
            ExpiresAt = refreshExpires,
            CreatedByIp = ip
        });
        await db.SaveChangesAsync(ct);

        return new AuthResponse(
            access.Token, access.ExpiresAt, rawRefresh, refreshExpires,
            new UserSummary(user.Id, user.Email!, user.FullName, membership.TenantId,
                membership.Role.ToString(), [.. roles]));
    }

    private async Task RevokeAllAsync(Guid userId, string reason, CancellationToken ct)
    {
        var now = (DateTimeOffset?)clock.GetUtcNow();
        await db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.RevokedAt, now)
                .SetProperty(t => t.RevokedReason, reason), ct);
    }

    private async Task SendConfirmationEmailAsync(ApplicationUser user, CancellationToken ct)
    {
        var token = await users.GenerateEmailConfirmationTokenAsync(user);
        var link = BuildLink("confirm-email", new Dictionary<string, string?>
        {
            ["userId"] = user.Id.ToString(),
            ["token"] = EncodeToken(token)
        });

        await email.SendAsync(user.Email!, "Confirm your email",
            $"<p>Welcome, {WebUtility.HtmlEncode(user.FullName)}!</p>" +
            $"<p><a href=\"{WebUtility.HtmlEncode(link)}\">Confirm your email</a></p>", ct);
    }

    private string BuildLink(string path, Dictionary<string, string?> query) =>
        QueryHelpers.AddQueryString($"{appOptions.Value.FrontendBaseUrl.TrimEnd('/')}/{path}", query);

    // Identity tokens contain characters that break URLs → Base64Url wrap them.
    private static string EncodeToken(string token) => Base64Url.EncodeToString(Encoding.UTF8.GetBytes(token));

    private static bool TryDecodeToken(string encoded, out string token)
    {
        try
        {
            token = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(encoded));
            return true;
        }
        catch (FormatException)
        {
            token = string.Empty;
            return false;
        }
    }

    private static string Describe(IdentityResult result) =>
        string.Join(" ", result.Errors.Select(e => e.Description));
}