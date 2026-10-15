using Olava.Domain.Common;
using Olava.Domain.Enums;

namespace Olava.Domain.Entities.Weddings;

public sealed class Wedding : TenantEntity
{
    public string Slug { get; set; } = string.Empty;        // subdomain: priya-arjun.yourapp.com (editable)
    public string ShortCode { get; set; } = string.Empty;   // stable QR/redirect id (NEVER changes)
    public WeddingStatus Status { get; set; } = WeddingStatus.Draft;

    public string Title { get; set; } = string.Empty;
    public string PartnerOneName { get; set; } = string.Empty;
    public string PartnerTwoName { get; set; } = string.Empty;
    public DateOnly? WeddingDate { get; set; }
    public string TimeZoneId { get; set; } = "Asia/Kolkata";
    public string PreferredLanguage { get; set; } = "en";
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public Guid? TemplateVersionId { get; set; }            // pinned version, so template updates never break live sites
    public string ThemeJson { get; set; } = "{}";           // approved theme overrides (colors, fonts)
    public DateTimeOffset? PublishedAt { get; set; }
}
