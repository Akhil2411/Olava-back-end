using System.ComponentModel.DataAnnotations;

namespace Olava.Infrastructure.Options;

public sealed class AppOptions
{
    public const string SectionName = "App";

    [Required, Url] public string FrontendBaseUrl { get; set; } = string.Empty;
}

