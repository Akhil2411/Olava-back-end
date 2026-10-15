namespace Olava.Application.Auth;

public static class AppClaimTypes
{
    public const string Subject = "sub";
    public const string Email = "email";
    public const string Name = "name";
    public const string Role = "role";                // platform roles
    public const string TenantId = "tenant_id";
    public const string TenantRole = "tenant_role";   // Owner/Admin/Editor/Viewer
    public const string TokenId = "jti";
}