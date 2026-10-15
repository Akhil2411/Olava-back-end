namespace Olava.Application.Auth;

public static class AppRoles
{
    public const string PlatformAdmin = "PlatformAdmin";
    public const string Support = "Support";
    public const string TemplateCreator = "TemplateCreator";
    public static readonly string[] All = [PlatformAdmin, Support, TemplateCreator];
}